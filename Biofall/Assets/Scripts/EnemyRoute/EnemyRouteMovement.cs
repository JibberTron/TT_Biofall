/*
    PURPOSE:
    Handles enemy movement commands through the NavMeshAgent.

    This script does not decide what the enemy wants.
    It only moves the enemy where the brain tells it to go.

    PSEUDOCODE:
    If told to travel:
        Set travel speed.
        Move to the travel point.

    If told to wander:
        Set wander speed.
        Move to the wander point.

    If told to investigate:
        Set investigate speed.
        Move to the investigation location.

    If told to chase:
        Start repeatedly updating the destination to the player's position.

    If told to stop:
        Pause the NavMeshAgent.

    NOTES:
    - This script is intentionally simple.
    - EnemyRouteBrain decides the state.
    - This script executes the movement.
    - Chase uses a coroutine so the enemy updates the player's position over time instead of only once.
*/

using System.Collections;
using UnityEngine;

public class EnemyRouteMovement : MonoBehaviour
{
    private EnemyRouteReferences enemyRef;

    [Header("Movement Speeds")]
    [SerializeField] private float travelSpeed = 2f;
    [SerializeField] private float wanderSpeed = 1.5f;
    [SerializeField] private float investigateSpeed = 3f;
    [SerializeField] private float chaseSpeed = 5f;

    [Header("Path Updating")]
    [SerializeField] private float chasePathUpdateDelay = 0.2f;

    private Coroutine chaseRoutine;

    public EnemyRouteReferences EnemyRef => enemyRef;

    private void Awake()
    {
        enemyRef = GetComponent<EnemyRouteReferences>();
    }

    public void MoveToTravelPoint(Vector3 position)
    {
        MoveTo(position, travelSpeed);
    }

    public void MoveToWanderPoint(Vector3 position)
    {
        MoveTo(position, wanderSpeed);
    }

    public void MoveToInvestigatePoint(Vector3 position)
    {
        MoveTo(position, investigateSpeed);
    }

    public void MoveTo(Vector3 position, float speed)
    {
        enemyRef.Agent.speed = speed;
        enemyRef.Agent.isStopped = false;
        enemyRef.Agent.SetDestination(position);
    }

    public void StopMovement(bool shouldStop)
    {
        enemyRef.Agent.isStopped = shouldStop;
    }

    public bool ReachedDestination(float stoppingDistance = 1.25f)
    {
        if (enemyRef.Agent.pathPending)
        {
            return false;
        }

        if (!enemyRef.Agent.hasPath)
        {
            return false;
        }

        return enemyRef.Agent.remainingDistance <= stoppingDistance;
    }

    public void StartChase()
    {
        if (chaseRoutine == null)
        {
            chaseRoutine = StartCoroutine(UpdateChasePath());
        }
    }

    public void StopChase()
    {
        if (chaseRoutine != null)
        {
            StopCoroutine(chaseRoutine);
            chaseRoutine = null;
        }
    }

    private IEnumerator UpdateChasePath()
    {
        while (true)
        {
            if (enemyRef.Target != null && enemyRef.Agent.isOnNavMesh)
            {
                enemyRef.Agent.speed = chaseSpeed;
                enemyRef.Agent.isStopped = false;

                Vector3 targetPosition = enemyRef.Target.position;

                if (enemyRef.Agent.destination != targetPosition)
                {
                    enemyRef.Agent.SetDestination(targetPosition);
                }
            }

            yield return new WaitForSeconds(chasePathUpdateDelay);
        }
    }
}