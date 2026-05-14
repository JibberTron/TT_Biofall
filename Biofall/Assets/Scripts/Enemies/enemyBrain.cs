using System.Collections;
using UnityEngine;

public class enemyBrain : MonoBehaviour
{
    public enum EnemyState
    {
        IDLE,
        ROAMING,
        INVESTIGATING,
        CHASING
    }

    [SerializeField] enemyMovement movement;
    [SerializeField] enemyAnims anims;
    [SerializeField] float detectionRange = 8f;
    [SerializeField] float investigateTime = 3f;
    [SerializeField] float detectionAngle = 90f;

    public EnemyState currentState;

    bool isInvestigating;

    void Start()
    {
        movement = GetComponent<enemyMovement>();
        anims = GetComponent<enemyAnims>();

        StartCoroutine(StartAfterIdle());
    }

    void Update()
    {
        if (movement.EnemyRef.Target == null || isInvestigating) return;

        float distance = Vector3.Distance(transform.position, movement.EnemyRef.Target.position);

        if (currentState != EnemyState.CHASING && distance <= detectionRange && CanSeePlayer())
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
        }
    }
    void HandleRoam()
    {
        if (ReachedDestination())
        {
            ChangeState(EnemyState.INVESTIGATING);
            return;
        }

        anims.SetSpeed(movement.EnemyRef.Agent.velocity.magnitude, 2f);
    }
    void HandleChase()
    {
        anims.SetSpeed(movement.EnemyRef.Agent.velocity.magnitude, 2.5f);
    }
    void HandleInvestigate()
    {
        // animation handled in coroutine
    }
    IEnumerator Investigate()
    {
        isInvestigating = true;

        movement.Stop(true);
        anims.Investigate(true);

        yield return new WaitForSeconds(investigateTime);

        anims.Investigate(false);
        movement.EnemyRef.Agent.isStopped = false;

        if (CanSeePlayer())
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
        isInvestigating = true;

        yield return new WaitForSeconds(3f);

        isInvestigating = false;
        ChangeState(EnemyState.ROAMING);
    }
    bool ReachedDestination()
    {
        return !isInvestigating && !movement.EnemyRef.Agent.pathPending && movement.EnemyRef.Agent.hasPath && 
            movement.EnemyRef.Agent.remainingDistance <= 0.5f && movement.EnemyRef.Agent.velocity.sqrMagnitude < 0.01f;
    }

    bool CanSeePlayer()
    {
        Vector3 playerDir = (movement.EnemyRef.Target.transform.position - transform.position).normalized;
        detectionAngle = Vector3.Angle(transform.forward, playerDir);

        if (detectionAngle <= 90)
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

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(look),
            0.2f
        );
    }
}