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
    [SerializeField] private float chasePathUpdateDelay = 0.1f;

    [Header("Animation Parameters")]
    [SerializeField] private string speedParameter = "Speed";
    [SerializeField] private string investigateParameter = "IsLooking";
    [SerializeField] private string attackTrigger = "Attack";

    private Coroutine chaseRoutine;

    public EnemyRouteReferences EnemyRef => enemyRef;

    private void Awake()
    {
        enemyRef = GetComponent<EnemyRouteReferences>();
    }

    private void Update()
    {
        UpdateSpeedAnimation();
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

    public void SetInvestigatingAnimation(bool isInvestigating)
    {
        if (enemyRef.Animator == null)
        {
            return;
        }

        enemyRef.Animator.SetBool(investigateParameter, isInvestigating);
    }

    public void TriggerAttackAnimation()
    {
        if (enemyRef.Animator == null)
        {
            return;
        }

        enemyRef.Animator.SetTrigger(attackTrigger);
    }

    public void SetAttackCollider(bool enabled)
    {
        if (enemyRef.AttackCollider != null)
        {
            enemyRef.AttackCollider.enabled = enabled;
        }
    }

    private void UpdateSpeedAnimation()
    {
        if (enemyRef.Animator == null || enemyRef.Agent == null)
        {
            return;
        }

        enemyRef.Animator.SetFloat(speedParameter, enemyRef.Agent.velocity.magnitude);
    }

    private IEnumerator UpdateChasePath()
    {
        while (true)
        {
            if (enemyRef.Target != null && enemyRef.Agent.isOnNavMesh)
            {
                enemyRef.Agent.speed = chaseSpeed;
                enemyRef.Agent.isStopped = false;
                enemyRef.Agent.SetDestination(enemyRef.Target.position);
            }

            yield return new WaitForSeconds(chasePathUpdateDelay);
        }
    }
}