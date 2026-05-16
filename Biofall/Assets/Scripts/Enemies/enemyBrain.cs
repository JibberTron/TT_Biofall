using System.Collections;
using UnityEngine;

public class enemyBrain : MonoBehaviour
{
    public enum EnemyState
    {
        IDLE,
        ROAMING,
        INVESTIGATING,
        CHASING,
        INCAPACITATED,
        DEAD
    }

    enemyMovement movement;
    enemyAnims anims;
    enemyHealth health;

    [Header("-----AI Stats-----")]
    [Range(5, 100)][SerializeField] float detectionRange = 8f;
    [Range(3, 10)][SerializeField] float investigateTime = 3f;
    [Range(0, 90)][SerializeField] float detectionAngle = 90f;
    [Range(5, 500)][SerializeField] float incapacitatedTimer = 5f;

    float distance;
    float distanceTimer;

    EnemyState currentState;
    public EnemyState CurrentState => currentState;
    
    bool isInvestigating;
    bool isIncapacitated;

    void Start()
    {
        movement = GetComponent<enemyMovement>();
        anims = GetComponent<enemyAnims>();
        health = GetComponent<enemyHealth>();

        StartCoroutine(StartAfterIdle());
    }

    void Update()
    {
        if (movement.EnemyRef.Target == null || isInvestigating) return;
        if (currentState == EnemyState.INCAPACITATED) return;
        if (health.isDead)
        {
            ChangeState(EnemyState.INCAPACITATED);
            return;
        }

        if (currentState != EnemyState.CHASING && PlayerFound())
        {
            ChangeState(EnemyState.CHASING);
            return;
        }

        switch (currentState)
        {
            case EnemyState.ROAMING:
                HandleRoam();
                break;

            case EnemyState.INVESTIGATING:
                HandleInvestigate();
                break;

            case EnemyState.CHASING:
                HandleChase();
                break;
        }
    }
    void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        switch (newState)
        {
            case EnemyState.ROAMING:
                movement.StopChase();
                movement.GoToNextPoint();
                break;

            case EnemyState.CHASING:
                movement.Chase();
                break;

            case EnemyState.INVESTIGATING:
                StartCoroutine(Investigate());
                break;

            case EnemyState.DEAD:
                HandleDead();
                break;

            case EnemyState.INCAPACITATED:
                Debug.Log("HERER");
                StartCoroutine(Incapacitated());
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

        movement.SetSpeed(1.5f);
        anims.SetMovement(movement.EnemyRef.Agent.velocity.magnitude);
    }
    void HandleChase()
    {
        distanceTimer += Time.deltaTime;

        if (distanceTimer >= 0.1f)
        {
            distanceTimer = 0f;

            distance = Vector3.Distance(transform.position, movement.EnemyRef.Target.position);
        }
        movement.SetSpeed(2f);
        anims.SetMovement(movement.EnemyRef.Agent.velocity.magnitude);
    }
    void HandleInvestigate()
    {
        // animation handled in coroutine
    }
    void HandleDead()
    {
        movement.Stop(true);
        movement.EnemyRef.Agent.ResetPath();
        movement.EnableNav(false);
        movement.SetSpeed(0);
        movement.ShouldUpdatePath(false);
        anims.Death(true);
    }
    bool ReachedDestination()
    {
        return !isInvestigating && !movement.EnemyRef.Agent.pathPending && movement.EnemyRef.Agent.hasPath && 
            movement.EnemyRef.Agent.remainingDistance <= 0.5f && movement.EnemyRef.Agent.velocity.sqrMagnitude < 0.01f;
    }
    bool PlayerFound()
    {
        return (CanSeePlayer() && distance <= detectionRange);
    }
    bool CanSeePlayer()
    {
        Vector3 playerDir = (movement.EnemyRef.Target.transform.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, playerDir);

        if (angleToPlayer <= detectionAngle)
        {
            if (Physics.Raycast(transform.position, playerDir, out RaycastHit hit, detectionRange))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    RotateToPlayer();
                    return true;
                }
            }
        }
        return false;
    }
    void RotateToPlayer()
    {
        Vector3 look = movement.EnemyRef.Target.position - transform.position;
        look.y = 0;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look), 0.2f);
    }
    IEnumerator Incapacitated()
    {
        if (currentState == EnemyState.INCAPACITATED)
        {
            HandleDead();
            isIncapacitated = true;
            yield return new WaitForSeconds(incapacitatedTimer);
   
            anims.Death(false);
            health.isDead = false;
            health.currentHP = 10;
            
            anims.StandUp(isIncapacitated);

            yield return new WaitForSeconds(6f);

            movement.EnableNav(true);
            movement.EnemyRef.Agent.ResetPath();
            movement.Stop(false);

            isIncapacitated = false;
            movement.ShouldUpdatePath(true);
            anims.StandUp(isIncapacitated);
            if(PlayerFound())
            {
                ChangeState(EnemyState.CHASING);
            }
            else
            {
                ChangeState(EnemyState.ROAMING);
            }
            
        }
    }
    IEnumerator Investigate()
    {
        isInvestigating = true;

        movement.Stop(true);
        movement.SetSpeed(0);
        anims.Investigate(true);
  
        yield return new WaitForSeconds(investigateTime);

        anims.Investigate(false);
        movement.SetSpeed(movement.OrigSpeed);
        movement.Stop(false);

        if (PlayerFound())
        {
            ChangeState(EnemyState.CHASING);
        }
        else
        {
            ChangeState(EnemyState.ROAMING);
        }
    
        isInvestigating = false;
    }
    IEnumerator StartAfterIdle()
    {
        currentState = EnemyState.IDLE;
        movement.ShouldUpdatePath(false);

        yield return new WaitForSeconds(1f);

        movement.ShouldUpdatePath(true);
        ChangeState(EnemyState.ROAMING);
    }
}