using System.Collections;
using UnityEngine;

public class enemyBrain : MonoBehaviour
{
    enemyReferences enemyRef;
    enemyMovement movement;
    enemyHealth health;

    public enum EnemyState
    {
        IDLE,
        ROAMING,
        INVESTIGATING,
        CHASING,
        ATTACKING,
        INCAPACITATED,
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

    float distance;
    float distanceTimer;
    float angleTimer;
    

    void Start()
    {
        movement = GetComponent<enemyMovement>();
        enemyRef = GetComponent<enemyReferences>();
        health = GetComponent<enemyHealth>();

        StartCoroutine(StartAfterIdle());
    }
    void Update()
    {
        HandleUpdates();
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
        if (currentState == newState) return;
        if(currentState == EnemyState.INVESTIGATING)
        {
            movement.Investigate(false);
        }
        if(stateRoutine != null)
        {
            StopCoroutine(stateRoutine);
            stateRoutine = null;
        }
        currentState = newState;

        switch (newState)
        {
            case EnemyState.ROAMING:
                movement.Stop(false);
                movement.StopChase();
                movement.GoToNextPoint();
                break;

            case EnemyState.CHASING:
                movement.Chase();
                break;

            case EnemyState.INVESTIGATING:
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
        }
    }
    void HandleRoam()
    {
        if (ReachedDestination())
        { 
            Debug.Log("RIP");
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
        }
        movement.SetSpeed(movement.ChaseSpeed);
        movement.SetMovement();
    }
    void HandleAttack()
    {

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
        if(angleTimer >= 0.1f)
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
        if(currentState != EnemyState.INCAPACITATED)
        {
            yield break;
        }
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
    IEnumerator Investigate()
    {
        if(currentState != EnemyState.INVESTIGATING)
        {
            yield break;
        }
        if(PlayerFound())
        {
            ChangeState(EnemyState.CHASING);
            yield break;
        }
        yield return new WaitForSeconds(investigateTime);

        Debug.Log("OUTSIDE OF INVESTIGATE CO");

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
}