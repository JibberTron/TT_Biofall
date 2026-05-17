using UnityEngine;
using UnityEngine.AI;

public class enemyReferences : MonoBehaviour
{
    [SerializeField] Transform[] roamPos;
    [SerializeField] Transform target;
    NavMeshAgent agent;
    Animator animator;
    enemyAnims eAnims;

    public Transform[] RoamPos => roamPos;
    public Transform Target => target;
    public NavMeshAgent Agent => agent;
    public Animator Animator => animator;
    public enemyAnims EAnims => eAnims;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        eAnims = GetComponent<enemyAnims>();
    }
    void Start()
    {
        if (target == null)
        {
            Debug.Log("Enemy Reference target == null!");
        }
        if (eAnims == null)
        {
            Debug.Log("No enemyAnims script on the Enemy");
        }
        if (agent == null)
        {
            Debug.Log("Nav Mesh Agent == null!");
        }
        if(roamPos.Length == 0)
        {
            Debug.Log("Roam Positions array is empty");
        }
    }
}
