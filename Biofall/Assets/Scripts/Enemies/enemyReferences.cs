using UnityEngine;
using UnityEngine.AI;

public class enemyReferences : MonoBehaviour
{
    [SerializeField] Transform[] roamPos;
    [SerializeField] Transform target;
    NavMeshAgent agent;
    Animator animator;

    public Transform[] RoamPos => roamPos;
    public Transform Target => target;
    public NavMeshAgent Agent => agent;
    public Animator Animator => animator;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }
}
