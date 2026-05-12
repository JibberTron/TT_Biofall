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
        ATTACKING
    }

    [SerializeField] enemyMovement movement;
    [SerializeField] float detectionRange = 8f;
    [SerializeField] enemyAnims anims;

    public EnemyState currentState = EnemyState.IDLE;
    Vector3 playerDir;

    bool nextPoint = false;
    bool isIdle = false;
    bool isInvestigating;
    bool shouldGrab;
    // save
    float distance;
    float angleToPlayer;
    
    void Awake()
    {

    }

    void Start()
    {
        if (movement == null)
        {
            movement = GetComponent<enemyMovement>();
        }
        if (anims == null)
        {
            anims = GetComponent<enemyAnims>();
        }
        StartCoroutine(IdleDelay());
    }
    void Update()
    {
        if (movement.EnemyRef.Target == null) return;
        if (isIdle) return;
        SetStates();
    }
    void SetStates()
    {
        distance = Vector3.Distance(transform.position, movement.EnemyRef.Target.position);

        if (distance <= detectionRange && CanSeePlayer() && !isInvestigating)
        {
            isInvestigating = false;
            Debug.Log("Is Chasing now");

            currentState = EnemyState.CHASING;
            movement.Chase();
            anims.SetSpeed(movement.EnemyRef.Agent.velocity.magnitude, 2.5f);
            nextPoint = false;
            return;
        }
        if (currentState == EnemyState.INVESTIGATING && isInvestigating)
        {
            if (distance <= detectionRange && CanSeePlayer())
            {
                anims.Investigate(false);
                isInvestigating = false;
                return;
            }
            anims.Investigate(true);
            Debug.Log("Investigating now");

            return;
        }
        if (currentState == EnemyState.ROAMING)
        {
            anims.Investigate(false);
           // anims.PlayRun(false);
            anims.SetSpeed(movement.EnemyRef.Agent.velocity.magnitude, 2f);
            movement.Roam();
        }
        if (!nextPoint)
        {
            Debug.Log("!nextPoint now");

            currentState = EnemyState.ROAMING;
            movement.GoToNextPoint();
            nextPoint = true;
        }

        if (!isInvestigating && !movement.EnemyRef.Agent.pathPending && movement.EnemyRef.Agent.hasPath
            && movement.EnemyRef.Agent.remainingDistance <= 0.5f && movement.EnemyRef.Agent.velocity.sqrMagnitude < 0.01f)
        {
            StartCoroutine(LookingAround());
        }
    }
    void HandleChase()
    {
        if(distance <= detectionRange && CanSeePlayer())
        {

        }
    }
    void RotateToPlayer()
    {
        Vector3 lookPos = movement.EnemyRef.Target.position - transform.position;
        lookPos.y = 0;
        Quaternion rot = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 0.2f);
    }
    bool CanSeePlayer()
    {
        playerDir = (movement.EnemyRef.Target.transform.position - transform.position).normalized;
        angleToPlayer = Vector3.Angle(transform.forward, playerDir);
        Debug.DrawRay(transform.position, playerDir * detectionRange, Color.red);
        if (angleToPlayer <= 90)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, playerDir, out hit, detectionRange))
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
    IEnumerator IdleDelay()
    {
        isIdle = true;
        currentState = EnemyState.IDLE;

        Debug.Log("Idle now");

        yield return new WaitForSeconds(5f);

        Debug.Log("Roam now");

        isIdle = false;
    }
    IEnumerator LookingAround()
    {
        isInvestigating = true;
        currentState = EnemyState.INVESTIGATING;
        movement.EnemyRef.Agent.isStopped = true;

        Debug.Log("Looking around now");

        yield return new WaitForSeconds(5f);

        Debug.Log("GoToNextPoint now");

        movement.EnemyRef.Agent.isStopped = false;
        currentState = EnemyState.ROAMING;
        movement.GoToNextPoint();

        isInvestigating = false;
    }

}