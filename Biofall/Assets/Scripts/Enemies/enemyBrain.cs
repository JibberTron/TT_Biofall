using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class enemyBrain : MonoBehaviour
{
    enemyReferences enemyRef;
    enemyMovement movement;
    enemyHealth health;
    enemyAttack attack;

    enum EnemyState
    {
        IDLE,
        ROAMING,
        CHASING,
        ATTACKING,
        INCAPACITATED,
        INVESTIGATING,
        INVESTIGATING_NOISE,
        INVESTIGATING_HIDING,
        DEAD
    }

    public enum EnemyActiveState
    {
        ACTIVE,
        DEACTIVE
    }

    [Header("-----AI Stats-----")]
    [Range(5, 100)][SerializeField] float detectionRange = 8f;
    [Range(5, 100)][SerializeField] float outOfDetectionRange = 18f;
    [Range(3, 10)][SerializeField] float investigateTime = 3f;
    [Range(0, 120)][SerializeField] float detectionAngle = 120f;
    [Range(5, 500)][SerializeField] float incapacitatedTimer = 5f;
    [Range(6, 500)][SerializeField] float incapacitatedDelay = 6f;

    EnemyState currentState;
    public EnemyActiveState activeState = EnemyActiveState.ACTIVE;

    Coroutine stateRoutine;
    NoiseSensor noiseSensor;
    GameObject player;

    const float stateLockTime = 0.2f;
    const float hidingGiveUpCooldown = 10f;
    float idleDelay = 1f;
    float stateLockTimer = 0f;
    float hidingGiveUpTimer = 0f;
    float hideTimer;

    bool isAttacking;
    bool gavUpOnHiding = false;
    bool isInvestigating = false;
    bool hasPlayedChaseSound;

    void Awake()
    {
        movement = GetComponent<enemyMovement>();
        enemyRef = GetComponent<enemyReferences>();
        health = GetComponent<enemyHealth>();
        attack = GetComponentInChildren<enemyAttack>();
        noiseSensor = GetComponent<NoiseSensor>();
    }
    void Start()
    {
        StartChecks();
        player = enemyRef.Player;
        if(activeState == EnemyActiveState.ACTIVE)
        {
            StartCoroutine(StartAfterIdle());
        }
        else
        {
            currentState = EnemyState.IDLE;
        }
        
    }
    void Update()
    {
        HandleUpdates();     
    }
    public void SetActiveState(EnemyActiveState _state)
    {
        activeState = _state;
        if(_state == EnemyActiveState.ACTIVE)
        {
            StartCoroutine(StartAfterIdle());
            return;
        }
        else
        {
            currentState = EnemyState.IDLE;
            return;
        }
    }
    void StartChecks()
    {
        if(movement == null)
        {
            Debug.Log("Enemy Movement == null");
            return;
        }
        if (enemyRef == null)
        {
            Debug.Log("Enemy References == null");
        }
        if (health == null)
        {
            Debug.Log("Enemy Health == null");
            return;
        }
        if (attack == null)
        {
            Debug.Log("Enemy Attack == null");
            return;
        }
        if (noiseSensor == null)
        {
            Debug.Log("Noise Sensor == null");
            return;
        }
    }
    void HandleUpdates()
    {
        if (player == null) return;
       
        stateLockTimer += Time.deltaTime;
        if (gavUpOnHiding)
        {
            hidingGiveUpTimer -= Time.deltaTime;
            if (hidingGiveUpTimer <= 0f) gavUpOnHiding = false;
        }
        if (currentState == EnemyState.INVESTIGATING && stateRoutine == null && !isInvestigating)
        {
            ChangeState(EnemyState.ROAMING);
        }
        if (currentState == EnemyState.INCAPACITATED) return;

        if (health.IsDead)
        {
            ChangeState(EnemyState.INCAPACITATED);
            return;
        }

        switch (currentState)
        {
            case EnemyState.ROAMING:
                HandleRoam();
                return;

            case EnemyState.CHASING:
                HandleChase();
                break;
        }
    }
    void ChangeState(EnemyState _newState)
    {
        if (currentState == _newState || isAttacking) return;

        if (stateLockTimer < stateLockTime)
        {
            return;
        }

        if (_newState != EnemyState.INVESTIGATING)
        {
            isInvestigating = false;
        }

        stateLockTimer = 0f;

        if (currentState == EnemyState.CHASING)
        {
            movement.StopChase();
        }               
        if (currentState == EnemyState.INVESTIGATING)
        {
            movement.Investigate(false);
            movement.Stop(false);
        }
        if (currentState == EnemyState.ATTACKING)
        {
            isAttacking = false;
        }
        if (stateRoutine != null)
        {
            StopCoroutine(stateRoutine);
            stateRoutine = null;
        }

        currentState = _newState;

        switch (_newState)
        {
            case EnemyState.ROAMING:
                gavUpOnHiding = true;
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

                if (!isAttacking)
                {
                    stateRoutine = StartCoroutine(Attacking());
                }
                break;

            case EnemyState.INVESTIGATING_NOISE:
                movement.EnableAgentRotation(true);
                movement.Stop(false);
                movement.SetSpeed(movement.RoamSpeed);

                stateRoutine = StartCoroutine(InvestigateNoise());
                break;

            case EnemyState.INVESTIGATING_HIDING:
                gavUpOnHiding = true;
                hidingGiveUpTimer = hidingGiveUpCooldown;
                movement.Stop(false);
                movement.StopChase();
                stateRoutine = StartCoroutine(InvestigateHiding());
                break;
        }
    }   
    void HearNoise(NoiseData _noiseData)
    {
        if (currentState == EnemyState.ATTACKING || currentState == EnemyState.INCAPACITATED 
            || enemyRef.Visibility.IsHiding() || activeState == EnemyActiveState.DEACTIVE) return;
 
        movement.AddSoundPoints(_noiseData);

        if (currentState == EnemyState.INVESTIGATING_NOISE)
        {
            return;
        }
        if(currentState == EnemyState.CHASING)
        {
            return;
        }

        ChangeState(EnemyState.INVESTIGATING_NOISE);
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
    
        if (PlayerFound() && currentState != EnemyState.INVESTIGATING_HIDING)
        {
            HandleChaseNoise();
            ChangeState(EnemyState.CHASING);
            return;
        }
    }
    void HandleChase()
    {
        if (currentState != EnemyState.CHASING) return;
        
        movement.SetSpeed(movement.ChaseSpeed);
        movement.SetMovement();

        if (enemyRef.Visibility.IsHiding())
        {        
            if (!HasArrived()) return;
            
            ChangeState(EnemyState.INVESTIGATING_HIDING);
            return;
        }

        if (Vector3.Distance(transform.position, player.transform.position) >= outOfDetectionRange)
        {
            movement.RemoveSoundPoints();
            hasPlayedChaseSound = false;          
            ChangeState(EnemyState.INVESTIGATING);
            return;
        }

        if (!isAttacking && Vector3.Distance(transform.position, player.transform.position) <= attack.AttackDistance)
        {
            ChangeState(EnemyState.ATTACKING);
        }
    }
    void HandleChaseNoise()
    {
        if (!hasPlayedChaseSound)
        {
            hasPlayedChaseSound = true;
            enemyRef.Sounds.PlayClip(enemyRef.Sounds.EnemyChase);
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
        hasPlayedChaseSound = false;
    }
    bool HasArrived()
    {
        if (!enemyRef.Agent.enabled) return false;
        if (enemyRef.Agent.pathPending) return false;

        if (!enemyRef.Agent.hasPath) return true;

        if (enemyRef.Agent.remainingDistance > enemyRef.Agent.stoppingDistance)
            return false;

        if (enemyRef.Agent.velocity.sqrMagnitude > 0.05f)
            return false;

        return true;
    }
    bool ReachedDestination()
    {
        if (!enemyRef.Agent.enabled) return false;

        if (enemyRef.Agent.pathPending) return false;

        if (enemyRef.Agent.remainingDistance > enemyRef.Agent.stoppingDistance + 0.1f)
            return false;

        if (enemyRef.Agent.hasPath && enemyRef.Agent.velocity.sqrMagnitude > 0.01f)
            return false;

        return true;
    }
    bool HasReachedNoisePoint(Vector3 _target)
    {
        NavMeshAgent agent = enemyRef.Agent;
        if (agent.pathPending) return false;

        float distToTarget = Vector3.Distance(transform.position, _target);

        return distToTarget <= 2.8f && agent.remainingDistance <= agent.stoppingDistance + 0.6f && agent.velocity.sqrMagnitude < 0.25f;
    }
    bool PlayerFound()
    {
        if (enemyRef.Visibility.IsHiding()) return false;

        float currentDistance = Vector3.Distance(transform.position, player.transform.position);
        return CanSeePlayer() && currentDistance <= detectionRange;
    }
    bool CanSeePlayer()
    {
        if (enemyRef.Visibility.IsHiding())
            return false;

        Vector3 playerDir = (player.transform.position - transform.position).normalized;

        float angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        if (angleToPlayer > detectionAngle)
            return false;

        if (Physics.Raycast(transform.position, playerDir, out RaycastHit hit, detectionRange))
        {
            return hit.collider.CompareTag("Player");
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
        health.CurrentHP = health.MaxHP;
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
            HandleChaseNoise();
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

            float timer = 0f;

            while (timer < attack.AttackDelay)
            {
                movement.RotateToPlayer(transform);

                if (health.IsDead)
                {
                    attack.Attack(false);
                    isAttacking = false;
                    stateRoutine = null;
                    ChangeState(EnemyState.INCAPACITATED);
                    yield break;
                }

                timer += Time.deltaTime;
                yield return null;
            }

            attack.Attack(false);

            float dist = Vector3.Distance(transform.position, player.transform.position);

            if (dist > attack.AttackDistance) break;
        }
        stateRoutine = null;
        isAttacking = false;
        ChangeState(EnemyState.CHASING);
    }
    IEnumerator StartAfterIdle()
    {
        currentState = EnemyState.IDLE;

        movement.ShouldUpdatePath(false);

        yield return new WaitForSeconds(idleDelay);

        movement.ShouldUpdatePath(true);
        stateRoutine = null;
        ChangeState(EnemyState.ROAMING);
    }
    IEnumerator Investigate()
    {
        if (currentState != EnemyState.INVESTIGATING) yield break;

        float timer = 0;

        while (currentState == EnemyState.INVESTIGATING)
        {
            if (PlayerFound())
            {
                stateRoutine = null;
                HandleChaseNoise();
                ChangeState(EnemyState.CHASING);
                yield break;
            }

            if (timer >= investigateTime) break;

            timer += Time.deltaTime;
            yield return null;
        }

        movement.Stop(false);
        movement.SetSpeed(movement.OrigSpeed);

        stateRoutine = null;
        isInvestigating = false;
        ChangeState(EnemyState.ROAMING);
    }
    IEnumerator InvestigateNoise()
    {
        if (enemyRef.SoundPoints.Count == 0)
        {
            ChangeState(EnemyState.ROAMING);
            yield break;
        }
        hasPlayedChaseSound = false;
        Vector3 currentTarget = Vector3.zero;
        float timer = 0f;
        const float timeout = 20f;

        while (currentState == EnemyState.INVESTIGATING_NOISE)
        {
            if (PlayerFound())
            {
                movement.RemoveSoundPoints();
                HandleChaseNoise();
                ChangeState(EnemyState.CHASING);
                yield break;
            }

            NoiseData targetNoise = enemyRef.SoundPoints[0];
            foreach (var noise in enemyRef.SoundPoints)
            {
                if (noise.attractionStrength > targetNoise.attractionStrength)
                {
                    targetNoise = noise;
                }              
            }

            if (Vector3.Distance(currentTarget, targetNoise.position) > 1.5f)
            {
                currentTarget = targetNoise.position;
                movement.MoveTo(currentTarget);
            }

            movement.SetMovement();

            if (HasReachedNoisePoint(currentTarget))
            {
                movement.RemoveSoundPoints();
                ChangeState(EnemyState.INVESTIGATING);
                yield break;
            }

            if (timer >= timeout) break;

            timer += Time.deltaTime;
            yield return null;
        }

        movement.RemoveSoundPoints();
        ChangeState(EnemyState.ROAMING);
    }
    IEnumerator InvestigateHiding()
    {
        yield return null;

        float moveTimeout = 5f;
        float timer = 0f;

        while (enemyRef.Agent.pathPending)  yield return null;

        while (true)
        {
            if (!enemyRef.Agent.enabled)
                yield break;

            bool arrived = enemyRef.Agent.hasPath == false || 
                (enemyRef.Agent.remainingDistance <= enemyRef.Agent.stoppingDistance &&enemyRef.Agent.velocity.sqrMagnitude < 0.05f);

            if (arrived) break;

            timer += Time.deltaTime;

            if (timer >= moveTimeout)
            {
                Debug.Log("Failed to reach hiding location");
                stateRoutine = null;
                ChangeState(EnemyState.ROAMING);
                yield break;
            }

            yield return null;
        }

        movement.Stop(true);
        enemyRef.Agent.ResetPath();

        movement.ResetMovement();
        movement.SetSpeed(0);
        movement.Investigate(true);

        hideTimer = 0f;

        while (currentState == EnemyState.INVESTIGATING_HIDING)
        {
            if (PlayerFound())
            {
                movement.Investigate(false);
                stateRoutine = null;
                ChangeState(EnemyState.CHASING);
                yield break;
            }

            hideTimer += Time.deltaTime;

            if (hideTimer >= investigateTime) break;

            yield return null;
        }

        movement.Investigate(false);

        gavUpOnHiding = true;
        hidingGiveUpTimer = hidingGiveUpCooldown;
        if (PlayerFound())
        {
            stateRoutine = null;
            HandleChaseNoise();
            ChangeState(EnemyState.CHASING);
            yield break;
        }
        ChangeState(EnemyState.ROAMING);
    }
    void OnEnable() => noiseSensor.OnNoiseHeard += HearNoise;
    void OnDisable() => noiseSensor.OnNoiseHeard -= HearNoise;
}