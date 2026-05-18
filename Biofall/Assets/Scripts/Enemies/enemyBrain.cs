//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class enemyBrain : MonoBehaviour
//{
//    enemyReferences enemyRef;
//    enemyMovement movement;
//    enemyHealth health;
//    enemyAttack attack;

//    public enum EnemyState
//    {
//        IDLE,
//        ROAMING,
//        INVESTIGATING,
//        CHASING,
//        ATTACKING,
//        INCAPACITATED,
//        DEAD,

//        SEARCHING_LAST_KNOWN,
//        INVESTIGATING_NOISE
//    }

//    [Header("-----AI Stats-----")]
//    [Range(5, 100)][SerializeField] float detectionRange = 8f;
//    [Range(3, 10)][SerializeField] float investigateTime = 3f;
//    [Range(0, 90)][SerializeField] float detectionAngle = 90f;
//    [Range(5, 500)][SerializeField] float incapacitatedTimer = 5f;
//    [Range(6, 500)][SerializeField] float incapacitatedDelay = 6f;
//    [Range(0, 120)][SerializeField] float idleDelay = 1f;
//    [SerializeField] EnemyState currentState;

//    Coroutine stateRoutine;

//    float distance;
//    float distanceTimer;
//    float angleTimer;

//    bool isAttacking;

//    // ==============================================================
//    [Header("Noise Activity")]
//    [SerializeField] private float noiseRoomBoost = 2f;
//    [SerializeField] private float noiseBoostRadius = 25f;
//    NoiseSensor noiseSensor;
//    private Vector3 lastKnownNoisePosition;

//    // ==============================================================
//    void Awake()
//    {
//        movement = GetComponent<enemyMovement>();
//        enemyRef = GetComponent<enemyReferences>();
//        health = GetComponent<enemyHealth>();
//        attack = GetComponent<enemyAttack>();

//        // =============================================================
//        noiseSensor = GetComponent<NoiseSensor>();

//        // =============================================================
//    }
//    void Start()
//    {
//        //movement = GetComponent<enemyMovement>();
//        //enemyRef = GetComponent<enemyReferences>();
//        //health = GetComponent<enemyHealth>();
//        //attack = GetComponent<enemyAttack>();

//        //// =============================================================
//        //noiseSensor = GetComponent<NoiseSensor>();

//        //// =============================================================
//        StartCoroutine(StartAfterIdle());
//    }

//    void Update()
//    {
//        HandleUpdates();
//    }
//    // ===============================================================
//    private void OnEnable()
//    {
//        noiseSensor.OnNoiseHeard += HearNoise;
//    }

//    private void OnDisable()
//    {
//        noiseSensor.OnNoiseHeard -= HearNoise;
//    }

//    void HearNoise(NoiseData noiseData)
//    {
//        if (currentState == EnemyState.CHASING ||
//            currentState == EnemyState.ATTACKING ||
//            currentState == EnemyState.INCAPACITATED)
//        {
//            return;
//        }

//        lastKnownNoisePosition = noiseData.position;

//        ChangeState(EnemyState.INVESTIGATING_NOISE);
//    }

//    // ===============================================================
//    void HandleUpdates()
//    {
//        if (enemyRef.Target == null) return;
//        if (currentState == EnemyState.INCAPACITATED) return;

//        if (health.IsDead)
//        {
//            ChangeState(EnemyState.INCAPACITATED);
//            return;
//        }

//        if (PlayerFound())
//        {
//            ChangeState(EnemyState.CHASING);
//            return;
//        }

//        switch (currentState)
//        {
//            case EnemyState.ROAMING:
//                HandleRoam();
//                break;

//            case EnemyState.CHASING:
//                HandleChase();
//                break;
//        }
//    }
//    void ChangeState(EnemyState newState)
//    {
//        if (currentState == newState || isAttacking) return;

//        if(currentState == EnemyState.INVESTIGATING)
//        {
//            movement.Investigate(false);
//        }

//        if (currentState == EnemyState.ATTACKING)
//        {
//            isAttacking = false;
//            distanceTimer = 0f;
//        }

//        if (stateRoutine != null)
//        {
//            StopCoroutine(stateRoutine);
//            stateRoutine = null;
//        }

//        currentState = newState;

//        switch (newState)
//        {
//            case EnemyState.ROAMING:
//                movement.EnableAgentRotation(true);
//                movement.Stop(false);
//                movement.StopChase();
//                movement.GoToNextPoint();
//                break;

//            case EnemyState.CHASING:
//                movement.EnableAgentRotation(true);
//                movement.Chase();
//                break;

//            case EnemyState.INVESTIGATING:
//                movement.EnableAgentRotation(false);
//                movement.Stop(true);
//                movement.SetSpeed(0);
//                movement.Investigate(true);

//                stateRoutine = StartCoroutine(Investigate());
//                break;

//            case EnemyState.DEAD:
//                HandleDead();
//                break;

//            case EnemyState.INCAPACITATED:
//                stateRoutine = StartCoroutine(Incapacitated());
//                break;

//            case EnemyState.ATTACKING:
//                movement.EnableAgentRotation(false);
//                enemyRef.Agent.ResetPath();
//                enemyRef.Agent.velocity = Vector3.zero;

//                movement.Stop(true);
//                movement.SetSpeed(0);

//                attack.EnableCollider();

//                if (!isAttacking)
//                {
//                    stateRoutine = StartCoroutine(Attacking());
//                }
//                break;


//            //case EnemyState.INVESTIGATING_NOISE:
//            //    movement.EnableAgentRotation(true);
//            //    movement.Stop(false);
//            //    movement.SetSpeed(movement.RoamSpeed);
//            //    movement.MoveToNoise(lastKnownNoisePosition);
//            //    stateRoutine = StartCoroutine(InvestigateNoise());
//            //    break;
//        }
//    }
//    void HandleRoam()
//    {
//        if (ReachedDestination())
//        { 
//            ChangeState(EnemyState.INVESTIGATING);
//            return;
//        }

//        movement.SetSpeed(movement.RoamSpeed);
//        movement.SetMovement();
//    }
//    void HandleChase()
//    {
//        distanceTimer += Time.deltaTime;

//        if (distanceTimer >= 0.1f)
//        {
//            distanceTimer = 0f;

//            distance = Vector3.Distance(transform.position, enemyRef.Target.position);
//            movement.SetSpeed(movement.ChaseSpeed);
//            movement.SetMovement();

//            if (!isAttacking && distance <= attack.AttackDistance)
//            {
//                ChangeState(EnemyState.ATTACKING);
//            }
//        }
//    }
//    void HandleDead()
//    {
//        movement.Stop(true);
//        enemyRef.Agent.ResetPath();

//        movement.EnableNav(false);
//        movement.SetSpeed(0);
//        movement.ShouldUpdatePath(false);

//        health.Death(true);
//    }
//    bool ReachedDestination()
//    {
//        return !enemyRef.Agent.pathPending && enemyRef.Agent.hasPath &&
//           enemyRef.Agent.remainingDistance <= 0.5f && enemyRef.Agent.velocity.sqrMagnitude < 0.01f;
//    }
//    bool PlayerFound()
//    {
//        return (CanSeePlayer() && distance <= detectionRange);
//    }
//    bool CanSeePlayer()
//    {
//        angleTimer += Time.deltaTime;

//        if(angleTimer >= 0.1f)
//        {
//            angleTimer = 0;

//            Vector3 playerDir = (enemyRef.Target.transform.position - transform.position).normalized;
//            float angleToPlayer = Vector3.Angle(playerDir, transform.forward);

//            if (angleToPlayer <= detectionAngle)
//            {
//                if (Physics.Raycast(transform.position, playerDir, out RaycastHit hit, detectionRange))
//                {
//                    if (hit.collider.CompareTag("Player"))
//                    {
//                        movement.RotateToPlayer(transform);
//                        return true;
//                    }
//                }
//            }
//            return false;
//        }
//        return false;
//    }
//    IEnumerator Incapacitated()
//    {
//        if(currentState != EnemyState.INCAPACITATED) yield break;

//        HandleDead();

//        yield return new WaitForSeconds(incapacitatedTimer);

//        health.StandUp(true);
//        health.Death(false);

//        health.IsDead = false;
//        health.CurrentHP = 10;
//        health.IncapInvinsibility = true;

//        yield return new WaitForSeconds(incapacitatedDelay);

//        movement.EnableNav(true);
//        enemyRef.Agent.ResetPath();

//        movement.Stop(false);

//        movement.ShouldUpdatePath(true);

//        health.StandUp(false);

//        health.IncapInvinsibility = false;
//        stateRoutine = null;

//        if (PlayerFound())
//        {
//            ChangeState(EnemyState.CHASING);
//        }
//        else
//        {
//            ChangeState(EnemyState.ROAMING);
//        }
//    }
//    IEnumerator Attacking()
//    {
//        isAttacking = true;

//        while (currentState == EnemyState.ATTACKING)
//        {
//            movement.RotateToPlayer(transform);

//            attack.Attack(true);
//            attack.EnableCollider();

//            float timer = 0f;

//            while (timer < attack.AttackDelay)
//            {
//                movement.RotateToPlayer(transform);
//                if (health.IsDead)
//                {
//                    attack.Attack(false);
//                    attack.DisableCollider();

//                    isAttacking = false;

//                    ChangeState(EnemyState.INCAPACITATED);
//                    yield break;
//                }

//                timer += Time.deltaTime;
//                yield return null;
//            }

//            attack.Attack(false);
//            attack.DisableCollider();

//            float dist = Vector3.Distance(transform.position, enemyRef.Target.position);

//            if (dist > attack.AttackDistance)
//            {
//                break;
//            }
//        }

//        isAttacking = false;
//        ChangeState(EnemyState.CHASING);
//    }
//    IEnumerator Investigate()
//    {
//        if(currentState != EnemyState.INVESTIGATING) yield break;

//        if(PlayerFound())
//        {
//            ChangeState(EnemyState.CHASING);
//            yield break;
//        }
//        yield return new WaitForSeconds(investigateTime);

//        movement.Stop(false);
//        movement.SetSpeed(movement.OrigSpeed);

//        stateRoutine = null;
//        ChangeState(EnemyState.ROAMING);
//    }
//    IEnumerator StartAfterIdle()
//    {
//        currentState = EnemyState.IDLE;
//        movement.ShouldUpdatePath(false);

//        yield return new WaitForSeconds(idleDelay);

//        movement.ShouldUpdatePath(true);
//        ChangeState(EnemyState.ROAMING);
//    }

//    // ==================================================

//    //IEnumerator InvestigateNoise()
//    //{

//    //    float timeout = 6f;
//    //    float elapsed = 0f;

//    //    movement.Stop(false);

//    //    while (true)
//    //    {
//    //        if (PlayerFound())
//    //        {
//    //            ChangeState(EnemyState.CHASING);
//    //            yield break;
//    //        }

//    //        elapsed += Time.deltaTime;
//    //        if (elapsed >= timeout)
//    //            break;

//    //        if (ReachedDestination())
//    //        { 
//    //            ChangeState(EnemyState.INVESTIGATING);
//    //            yield break;
//    //        }
//    //        yield return null;
//    //    }

//    //    movement.Stop(true);

//    //    yield return new WaitForSeconds(1f);

//    //    ChangeState(EnemyState.ROAMING);
//    //}
//    IEnumerator InvestigateNoise()
//    {
//        float timeout = 6f;
//        float timer = 0f;

//        movement.Stop(false);

//        while (true)
//        {
//            if (PlayerFound())
//            {
//                ChangeState(EnemyState.CHASING);
//                yield break;
//            }

//            timer += Time.deltaTime;
//            if (timer >= timeout)
//                break;

//            if (ReachedDestination())
//            {
//                ChangeState(EnemyState.INVESTIGATING);
//                yield break;
//            }                

//            yield return null;
//        }

//        movement.Stop(true);

//        yield return new WaitForSeconds(3f);

//        ChangeState(EnemyState.ROAMING);
//    }
//}
using System.Collections;
using UnityEngine;

public class enemyBrain : MonoBehaviour
{
    enemyReferences enemyRef;
    enemyMovement movement;
    enemyHealth health;
    enemyAttack attack;

    public enum EnemyState
    {
        IDLE,
        ROAMING,
        INVESTIGATING,
        CHASING,
        ATTACKING,
        INCAPACITATED,
        INVESTIGATING_NOISE,
        DEAD
    }

    [Header("-----AI Stats-----")]
    [Range(5, 100)][SerializeField] float detectionRange = 8f;
    [Range(3, 10)][SerializeField] float investigateTime = 3f;
    [Range(0, 90)][SerializeField] float detectionAngle = 90f;
    [Range(5, 500)][SerializeField] float incapacitatedTimer = 5f;
    [Range(6, 500)][SerializeField] float incapacitatedDelay = 6f;
    [Range(0, 120)][SerializeField] float idleDelay = 1f;
    [SerializeField] EnemyState currentState;

    Coroutine stateRoutine;

    NoiseSensor noiseSensor;
    private Vector3 lastKnownNoisePosition;

    float distance;
    float distanceTimer;
    float angleTimer;

    bool isAttacking;
    void Awake()
    {
        movement = GetComponent<enemyMovement>();
        enemyRef = GetComponent<enemyReferences>();
        health = GetComponent<enemyHealth>();
        attack = GetComponent<enemyAttack>();
        noiseSensor = GetComponent<NoiseSensor>();
    }
    void Start()
    {
        

        StartCoroutine(StartAfterIdle());
    }
    void Update()
    {
        HandleUpdates();
    }
    private void OnEnable()
    {
        noiseSensor.OnNoiseHeard += HearNoise;
    }

    private void OnDisable()
    {
        noiseSensor.OnNoiseHeard -= HearNoise;
    }

    void HearNoise(NoiseData noiseData)
    {
        if (currentState == EnemyState.CHASING ||
            currentState == EnemyState.ATTACKING ||
            currentState == EnemyState.INCAPACITATED)
        {
            return;
        }

        lastKnownNoisePosition = noiseData.position;

        ChangeState(EnemyState.INVESTIGATING_NOISE);
    }
    void HandleUpdates()
    {
        if (enemyRef.Target == null) return;
        if (currentState == EnemyState.INCAPACITATED) return;

        if (health.IsDead)
        {
            ChangeState(EnemyState.INCAPACITATED);
            return;
        }

        if (PlayerFound())
        {
            ChangeState(EnemyState.CHASING);
            return;
        }

        switch (currentState)
        {
            case EnemyState.ROAMING:
                HandleRoam();
                break;

            case EnemyState.CHASING:
                HandleChase();
                break;
        }
    }
    void ChangeState(EnemyState newState)
    {
        if (currentState == newState || isAttacking) return;

        if (currentState == EnemyState.INVESTIGATING)
        {
            movement.Investigate(false);
        }

        if (currentState == EnemyState.ATTACKING)
        {
            isAttacking = false;
            distanceTimer = 0f;
        }

        if (stateRoutine != null)
        {
            StopCoroutine(stateRoutine);
            stateRoutine = null;
        }

        currentState = newState;

        switch (newState)
        {
            case EnemyState.ROAMING:
                movement.EnableAgentRotation(true);
                movement.Stop(false);
                movement.StopChase();
                movement.GoToNextPoint();
                break;

            case EnemyState.CHASING:
                movement.EnableAgentRotation(true);
                movement.Chase();
                break;

            case EnemyState.INVESTIGATING:
                movement.EnableAgentRotation(false);
                movement.Stop(true);
                movement.SetSpeed(0);
                movement.Investigate(true);

                stateRoutine = StartCoroutine(Investigate());
                break;

            case EnemyState.DEAD:
                HandleDead();
                break;

            case EnemyState.INCAPACITATED:
                stateRoutine = StartCoroutine(Incapacitated());
                break;

            case EnemyState.ATTACKING:
                movement.EnableAgentRotation(false);
                enemyRef.Agent.ResetPath();
                enemyRef.Agent.velocity = Vector3.zero;

                movement.Stop(true);
                movement.SetSpeed(0);

                attack.EnableCollider();

                if (!isAttacking)
                {
                    stateRoutine = StartCoroutine(Attacking());
                }
                break;

            case EnemyState.INVESTIGATING_NOISE:
                movement.EnableAgentRotation(true);
                movement.Stop(false);
                movement.SetSpeed(movement.RoamSpeed);
                movement.MoveToNoise(lastKnownNoisePosition);
                stateRoutine = StartCoroutine(InvestigateNoise());
                break;
        }
    }
    void HandleRoam()
    {
        if (ReachedDestination())
        {
            ChangeState(EnemyState.INVESTIGATING);
            return;
        }

        movement.SetSpeed(movement.RoamSpeed);
        movement.SetMovement();
    }
    void HandleChase()
    {
        distanceTimer += Time.deltaTime;

        if (distanceTimer >= 0.1f)
        {
            distanceTimer = 0f;

            distance = Vector3.Distance(transform.position, enemyRef.Target.position);
            movement.SetSpeed(movement.ChaseSpeed);
            movement.SetMovement();

            if (!isAttacking && distance <= attack.AttackDistance)
            {
                ChangeState(EnemyState.ATTACKING);
            }
        }
    }
    void HandleDead()
    {
        movement.Stop(true);
        enemyRef.Agent.ResetPath();

        movement.EnableNav(false);
        movement.SetSpeed(0);
        movement.ShouldUpdatePath(false);

        health.Death(true);
    }
    bool ReachedDestination()
    {
        return !enemyRef.Agent.pathPending && enemyRef.Agent.hasPath &&
           enemyRef.Agent.remainingDistance <= 0.5f && enemyRef.Agent.velocity.sqrMagnitude < 0.01f;
    }
    bool PlayerFound()
    {
        return (CanSeePlayer() && distance <= detectionRange);
    }
    bool CanSeePlayer()
    {
        angleTimer += Time.deltaTime;

        if (angleTimer >= 0.1f)
        {
            angleTimer = 0;

            Vector3 playerDir = (enemyRef.Target.transform.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(playerDir, transform.forward);

            if (angleToPlayer <= detectionAngle)
            {
                if (Physics.Raycast(transform.position, playerDir, out RaycastHit hit, detectionRange))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        movement.RotateToPlayer(transform);
                        return true;
                    }
                }
            }
            return false;
        }
        return false;
    }
    IEnumerator Incapacitated()
    {
        if (currentState != EnemyState.INCAPACITATED) yield break;

        HandleDead();

        yield return new WaitForSeconds(incapacitatedTimer);

        health.StandUp(true);
        health.Death(false);

        health.IsDead = false;
        health.CurrentHP = 10;
        health.IncapInvinsibility = true;

        yield return new WaitForSeconds(incapacitatedDelay);

        movement.EnableNav(true);
        enemyRef.Agent.ResetPath();

        movement.Stop(false);

        movement.ShouldUpdatePath(true);

        health.StandUp(false);

        health.IncapInvinsibility = false;
        stateRoutine = null;

        if (PlayerFound())
        {
            ChangeState(EnemyState.CHASING);
        }
        else
        {
            ChangeState(EnemyState.ROAMING);
        }
    }
    IEnumerator Attacking()
    {
        isAttacking = true;

        while (currentState == EnemyState.ATTACKING)
        {
            movement.RotateToPlayer(transform);

            attack.Attack(true);
            attack.EnableCollider();

            float timer = 0f;

            while (timer < attack.AttackDelay)
            {
                movement.RotateToPlayer(transform);
                if (health.IsDead)
                {
                    attack.Attack(false);
                    attack.DisableCollider();

                    isAttacking = false;

                    ChangeState(EnemyState.INCAPACITATED);
                    yield break;
                }

                timer += Time.deltaTime;
                yield return null;
            }

            attack.Attack(false);
            attack.DisableCollider();

            float dist = Vector3.Distance(transform.position, enemyRef.Target.position);

            if (dist > attack.AttackDistance)
            {
                break;
            }
        }

        isAttacking = false;
        ChangeState(EnemyState.CHASING);
    }
    IEnumerator Investigate()
    {
        if (currentState != EnemyState.INVESTIGATING) yield break;

        if (PlayerFound())
        {
            ChangeState(EnemyState.CHASING);
            yield break;
        }
        yield return new WaitForSeconds(investigateTime);

        movement.Stop(false);
        movement.SetSpeed(movement.OrigSpeed);

        stateRoutine = null;
        ChangeState(EnemyState.ROAMING);
    }
    IEnumerator StartAfterIdle()
    {
        currentState = EnemyState.IDLE;
        movement.ShouldUpdatePath(false);

        yield return new WaitForSeconds(idleDelay);

        movement.ShouldUpdatePath(true);
        ChangeState(EnemyState.ROAMING);
    }
    IEnumerator InvestigateNoise()
    {
        float timeout = 6f;
        float timer = 0f;

        movement.Stop(false);

        while (true)
        {
            if (PlayerFound())
            {
                ChangeState(EnemyState.CHASING);
                yield break;
            }

            timer += Time.deltaTime;
            if (timer >= timeout)
                break;

            if (ReachedDestination())
            {
                ChangeState(EnemyState.INVESTIGATING);
                yield break;
            }

            yield return null;
        }

        movement.Stop(true);

        yield return new WaitForSeconds(3f);

        ChangeState(EnemyState.ROAMING);
    }
}