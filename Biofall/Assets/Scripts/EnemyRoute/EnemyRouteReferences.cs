using UnityEngine;
using UnityEngine.AI;

public class EnemyRouteReferences : MonoBehaviour
{
    [Header("Core References")]
    [SerializeField] private Transform target;
    [SerializeField] private RoomPatrolPoint[] roomPoints;

    [Header("Combat References")]
    [SerializeField] private Collider attackCollider;

    private NavMeshAgent agent;
    private Animator animator;

    public Transform Target => target;
    public RoomPatrolPoint[] RoomPoints => roomPoints;
    public NavMeshAgent Agent => agent;
    public Animator Animator => animator;
    public Collider AttackCollider => attackCollider;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }
}