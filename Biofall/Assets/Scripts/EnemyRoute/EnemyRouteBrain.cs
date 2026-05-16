/*
    PURPOSE:
    Controls the enemy's decision-making and state changes.

    This script decides what the enemy should be doing:
    - Idle at the start
    - Travel to a selected room
    - Wander inside that room
    - Investigate noises
    - Chase the player
    - Search the player's last known location after losing them

    PSEUDOCODE:
    Start:
        Get required components.
        Initialize room memory.
        Begin in IDLE.
        After a short delay, choose a room route.

    Update:
        If there is no player target, do nothing.
        Check if the player is moving close enough to be heard.
        If currently chasing, check if the player has escaped.
        Save the player's current position for next frame movement comparison.

    Room Selection:
        For each room:
            Check distance.
            Check memory value.
            Check activity interest.
            Calculate final weight.
        Pick a room based on weighted chance.

    Noise:
        If a noise is heard:
            Store the noise position.
            Boost nearby room interest.
            Interrupt current route.
            Investigate the noise.
            After investigation, choose a new room.

    Chase:
        If player is close and moving, chase.
        If player gets too far, go to their last known position.
        Search briefly.
        Then resume room routing.

    NOTES:
    - This is the "brain" script. It should not directly handle low-level movement.
    - Movement commands are passed to EnemyRouteMovement.
    - Room data comes from RoomPatrolPoint.
    - Noise events come from NoiseSensor.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyRouteReferences))]
[RequireComponent(typeof(EnemyRouteMovement))]
[RequireComponent(typeof(NoiseSensor))]
public class EnemyRouteBrain : MonoBehaviour
{
    public enum EnemyState
    {
        IDLE,
        TRAVELING,
        ROOM_WANDER,
        INVESTIGATING_NOISE,
        CHASING,
        SEARCHING_LAST_KNOWN,
        ATTACKING
    }

    [Header("State")]
    [SerializeField] private EnemyState currentState;

    [Header("Room Selection")]
    [SerializeField] private float roomSearchRadius = 100f;
    [SerializeField] private float distanceWeightPower = 1.25f;
    [SerializeField] private float visitedPenalty = 0.15f;
    [SerializeField] private float memoryRecoveryAmount = 0.15f;
    [SerializeField] private float activityDecayAmount = 0.1f;

    [Header("Player Detection")]
    [SerializeField] private float hearingRange = 8f;
    [SerializeField] private float chaseRange = 12f;
    [SerializeField] private float losePlayerRange = 18f;
    [SerializeField] private float playerMoveNoiseThreshold = 0.1f;
    [SerializeField] private float closeProximityRange = 2f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackHitboxDelay = 0.35f;
    [SerializeField] private float attackHitboxActiveTime = 0.25f;

    private bool isAttacking;

    [Header("Timing")]
    [SerializeField] private float idleStartTime = 1f;
    [SerializeField] private float roomWanderWaitTime = 1.5f;
    [SerializeField] private int roomWanderSteps = 2;
    [SerializeField] private float investigateWaitTime = 2f;
    [SerializeField] private float searchWaitTime = 3f;

    [Header("Noise Activity")]
    [SerializeField] private float noiseRoomBoost = 2f;
    [SerializeField] private float noiseBoostRadius = 25f;

    private EnemyRouteReferences enemyRef;
    private EnemyRouteMovement movement;
    private NoiseSensor noiseSensor;

    private Coroutine activeRoutine;

    private Vector3 lastKnownPlayerPosition;
    private Vector3 lastPlayerPosition;

    private RoomPatrolPoint currentRoom;
    private RoomPatrolPoint lastRoom;

    private Dictionary<RoomPatrolPoint, float> roomMemory = new Dictionary<RoomPatrolPoint, float>();

    private void Awake()
    {
        enemyRef = GetComponent<EnemyRouteReferences>();
        movement = GetComponent<EnemyRouteMovement>();
        noiseSensor = GetComponent<NoiseSensor>();
    }

    private void OnEnable()
    {
        noiseSensor.OnNoiseHeard += HearNoise;
    }

    private void OnDisable()
    {
        noiseSensor.OnNoiseHeard -= HearNoise;
    }

    private void Start()
    {
        InitializeRoomMemory();

        if (enemyRef.Target != null)
        {
            lastPlayerPosition = enemyRef.Target.position;
        }

        ChangeState(EnemyState.IDLE);
        activeRoutine = StartCoroutine(StartAfterIdle());
    }

    private void Update()
    {
        if (enemyRef.Target == null)
        {
            return;
        }

        CheckPlayerMovementNoise();

        if (currentState == EnemyState.CHASING)
        {
            HandleChase();
        }

        lastPlayerPosition = enemyRef.Target.position;
    }

    private void InitializeRoomMemory()
    {
        roomMemory.Clear();

        foreach (RoomPatrolPoint room in enemyRef.RoomPoints)
        {
            if (room != null && !roomMemory.ContainsKey(room))
            {
                roomMemory.Add(room, 1f);
            }
        }
    }

    private void ChangeState(EnemyState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;
        Debug.Log($"{gameObject.name} changed state to {currentState}");

        switch (currentState)
        {
            case EnemyState.IDLE:
                movement.StopChase();
                movement.StopMovement(true);
                break;

            case EnemyState.TRAVELING:
                movement.StopChase();
                movement.StopMovement(false);
                break;

            case EnemyState.ROOM_WANDER:
                movement.StopChase();
                movement.StopMovement(false);
                break;

            case EnemyState.INVESTIGATING_NOISE:
                movement.StopChase();
                movement.MoveToInvestigatePoint(lastKnownPlayerPosition);
                break;

            case EnemyState.CHASING:
                StopActiveRoutine();
                movement.StartChase();
                break;

            case EnemyState.SEARCHING_LAST_KNOWN:
                movement.StopChase();
                movement.MoveToInvestigatePoint(lastKnownPlayerPosition);
                break;

            case EnemyState.ATTACKING:
                movement.StopChase();
                movement.StopMovement(true);
                movement.TriggerAttackAnimation();
                break;
        }
    }

    private IEnumerator StartAfterIdle()
    {
        yield return new WaitForSeconds(idleStartTime);
        PickNewRoomRoute();
    }

    private void PickNewRoomRoute()
    {
        RoomPatrolPoint selectedRoom = PickWeightedRoom();

        if (selectedRoom == null)
        {
            Debug.LogWarning($"{gameObject.name} could not find a valid room.");
            return;
        }

        UpdateRoomMemory(selectedRoom);

        lastRoom = currentRoom;
        currentRoom = selectedRoom;

        StopActiveRoutine();
        activeRoutine = StartCoroutine(FollowRouteToRoom(selectedRoom));
    }

    private IEnumerator AttackThenChase()
    {
        isAttacking = true;

        movement.StopMovement(true);
        movement.TriggerAttackAnimation();

        yield return new WaitForSeconds(attackHitboxDelay);

        movement.SetAttackCollider(true);

        yield return new WaitForSeconds(attackHitboxActiveTime);

        movement.SetAttackCollider(false);

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;

        if (enemyRef.Target != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, enemyRef.Target.position);

            if (distanceToPlayer <= losePlayerRange)
            {
                ChangeState(EnemyState.CHASING);
            }
            else
            {
                ChangeState(EnemyState.SEARCHING_LAST_KNOWN);

                StopActiveRoutine();
                activeRoutine = StartCoroutine(SearchLastKnownThenRoute());
            }
        }
    }

    private IEnumerator FollowRouteToRoom(RoomPatrolPoint room)
    {
        ChangeState(EnemyState.TRAVELING);

        if (room.routePoints != null)
        {
            foreach (Transform routePoint in room.routePoints)
            {
                if (routePoint == null)
                {
                    continue;
                }

                movement.MoveToTravelPoint(routePoint.position);

                while (!movement.ReachedDestination())
                {
                    yield return null;
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        movement.MoveToTravelPoint(room.transform.position);

        while (!movement.ReachedDestination())
        {
            yield return null;
        }

        activeRoutine = StartCoroutine(WanderRoomThenPickNewRoute(room));
    }

    private IEnumerator WanderRoomThenPickNewRoute(RoomPatrolPoint room)
    {
        ChangeState(EnemyState.ROOM_WANDER);

        if (room.wanderPoints != null && room.wanderPoints.Length > 0)
        {
            for (int i = 0; i < roomWanderSteps; i++)
            {
                Transform wanderPoint = room.wanderPoints[Random.Range(0, room.wanderPoints.Length)];

                if (wanderPoint == null)
                {
                    continue;
                }

                movement.MoveToWanderPoint(wanderPoint.position);

                while (!movement.ReachedDestination())
                {
                    yield return null;
                }

                movement.StopMovement(true);
                yield return new WaitForSeconds(roomWanderWaitTime);
                movement.StopMovement(false);
            }
        }
        else
        {
            movement.StopMovement(true);
            yield return new WaitForSeconds(roomWanderWaitTime);
            movement.StopMovement(false);
        }

        DecayRoomActivity();
        PickNewRoomRoute();
    }

    private void CheckPlayerMovementNoise()
    {
        if (currentState == EnemyState.CHASING)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, enemyRef.Target.position);
        float playerMovementAmount = Vector3.Distance(enemyRef.Target.position, lastPlayerPosition);

        bool playerMoved = playerMovementAmount > playerMoveNoiseThreshold;
        bool playerInHearingRange = distanceToPlayer <= hearingRange;
        bool playerTooClose = distanceToPlayer <= closeProximityRange;

        if ((playerMoved && playerInHearingRange) || playerTooClose)
        {
            lastKnownPlayerPosition = enemyRef.Target.position;
            ChangeState(EnemyState.CHASING);
        }
    }

    private void HandleChase()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, enemyRef.Target.position);

        if (distanceToPlayer <= attackRange && !isAttacking)
        {
            ChangeState(EnemyState.ATTACKING);

            StopActiveRoutine();
            activeRoutine = StartCoroutine(AttackThenChase());
            return;
        }

        if (distanceToPlayer <= chaseRange)
        {
            lastKnownPlayerPosition = enemyRef.Target.position;
        }

        if (distanceToPlayer > losePlayerRange)
        {
            ChangeState(EnemyState.SEARCHING_LAST_KNOWN);

            StopActiveRoutine();
            activeRoutine = StartCoroutine(SearchLastKnownThenRoute());
        }
    }

    private IEnumerator SearchLastKnownThenRoute()
    {
        while (!movement.ReachedDestination())
        {
            yield return null;
        }

        movement.StopMovement(true);
        yield return new WaitForSeconds(searchWaitTime);
        movement.StopMovement(false);

        BoostRoomsNearPosition(lastKnownPlayerPosition, noiseRoomBoost);

        PickNewRoomRoute();
    }

    private void HearNoise(NoiseData noiseData)
    {
        if (currentState == EnemyState.CHASING)
        {
            return;
        }

        lastKnownPlayerPosition = noiseData.position;

        BoostRoomsNearPosition(noiseData.position, noiseData.attractionStrength * noiseRoomBoost);

        ChangeState(EnemyState.INVESTIGATING_NOISE);

        StopActiveRoutine();
        activeRoutine = StartCoroutine(InvestigateNoiseThenRoute());
    }

    private IEnumerator InvestigateNoiseThenRoute()
    {
        while (!movement.ReachedDestination())
        {
            yield return null;
        }

        movement.StopMovement(true);
        yield return new WaitForSeconds(investigateWaitTime);
        movement.StopMovement(false);

        PickNewRoomRoute();
    }

    private RoomPatrolPoint PickWeightedRoom()
    {
        List<RoomCandidate> candidates = new List<RoomCandidate>();

        foreach (RoomPatrolPoint room in enemyRef.RoomPoints)
        {
            if (room == null || room == currentRoom)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, room.transform.position);

            if (distance > roomSearchRadius)
            {
                continue;
            }

            float distanceScore = GetDistanceScore(distance);
            float memoryScore = GetRoomMemory(room);
            float activityScore = 1f + room.activityInterest;
            float baseInterest = Mathf.Max(0.1f, room.baseInterest);

            if (room == lastRoom)
            {
                memoryScore = 0f;
            }

            float finalWeight = distanceScore * memoryScore * activityScore * baseInterest;

            if (finalWeight > 0f)
            {
                candidates.Add(new RoomCandidate(room, finalWeight));
            }
        }

        if (candidates.Count > 0)
        {
            DebugRoomPool(candidates);
            return PickWeightedCandidate(candidates);
        }

        // Backup: allow last room if it is the only usable route.
        foreach (RoomPatrolPoint room in enemyRef.RoomPoints)
        {
            if (room == null || room == currentRoom)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, room.transform.position);

            if (distance <= roomSearchRadius)
            {
                candidates.Add(new RoomCandidate(room, 1f));
            }
        }

        if (candidates.Count > 0)
        {
            DebugRoomPool(candidates);
            return PickWeightedCandidate(candidates);
        }

        return GetNearestNonCurrentRoom();
    }

    private float GetDistanceScore(float distance)
    {
        float normalizedDistance = Mathf.Clamp01(distance / roomSearchRadius);
        float closeness = 1f - normalizedDistance;

        return Mathf.Pow(closeness, distanceWeightPower);
    }

    private float GetRoomMemory(RoomPatrolPoint room)
    {
        if (!roomMemory.ContainsKey(room))
        {
            roomMemory.Add(room, 1f);
        }

        return roomMemory[room];
    }

    private void UpdateRoomMemory(RoomPatrolPoint visitedRoom)
    {
        List<RoomPatrolPoint> keys = new List<RoomPatrolPoint>(roomMemory.Keys);

        foreach (RoomPatrolPoint room in keys)
        {
            roomMemory[room] = Mathf.Clamp01(roomMemory[room] + memoryRecoveryAmount);
        }

        roomMemory[visitedRoom] = visitedPenalty;
    }

    private void BoostRoomsNearPosition(Vector3 position, float boostAmount)
    {
        foreach (RoomPatrolPoint room in enemyRef.RoomPoints)
        {
            if (room == null)
            {
                continue;
            }

            float distance = Vector3.Distance(position, room.transform.position);

            if (distance <= noiseBoostRadius)
            {
                float closeness = 1f - Mathf.Clamp01(distance / noiseBoostRadius);
                room.activityInterest += boostAmount * closeness;
            }
        }
    }

    private void DecayRoomActivity()
    {
        foreach (RoomPatrolPoint room in enemyRef.RoomPoints)
        {
            if (room == null)
            {
                continue;
            }

            room.activityInterest = Mathf.Max(0f, room.activityInterest - activityDecayAmount);
        }
    }

    private RoomPatrolPoint PickWeightedCandidate(List<RoomCandidate> candidates)
    {
        float totalWeight = 0f;

        foreach (RoomCandidate candidate in candidates)
        {
            totalWeight += candidate.weight;
        }

        float randomValue = Random.Range(0f, totalWeight);

        foreach (RoomCandidate candidate in candidates)
        {
            randomValue -= candidate.weight;

            if (randomValue <= 0f)
            {
                return candidate.room;
            }
        }

        return candidates[candidates.Count - 1].room;
    }

    private RoomPatrolPoint GetNearestNonCurrentRoom()
    {
        RoomPatrolPoint nearest = null;
        float nearestDistance = Mathf.Infinity;

        foreach (RoomPatrolPoint room in enemyRef.RoomPoints)
        {
            if (room == null || room == currentRoom)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, room.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = room;
            }
        }

        return nearest;
    }

    private void DebugRoomPool(List<RoomCandidate> candidates)
    {
        float totalWeight = 0f;

        foreach (RoomCandidate candidate in candidates)
        {
            totalWeight += candidate.weight;
        }

        string log = "Room Selection Pool:\n";

        foreach (RoomCandidate candidate in candidates)
        {
            float percentage = candidate.weight / totalWeight * 100f;
            log += $"{candidate.room.roomName}: {percentage:F1}%\n";
        }

        Debug.Log(log);
    }

    private void StopActiveRoutine()
    {
        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
            activeRoutine = null;
        }
    }

    private struct RoomCandidate
    {
        public RoomPatrolPoint room;
        public float weight;

        public RoomCandidate(RoomPatrolPoint room, float weight)
        {
            this.room = room;
            this.weight = weight;
        }
    }
}