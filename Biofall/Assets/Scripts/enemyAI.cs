using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, iDamage
{
    public Transform target;
    enemyReferences enemyRef;

    [Header("Stats")]
    [SerializeField] float pathToDelay = 0.2f;
    [SerializeField] float detectionRange = 8f;
    [SerializeField] int HP;
    [SerializeField] Transform[] roamPos;
    [SerializeField] Collider armCollider;
    bool playerInRange;
    int currentPos;

    Coroutine pathRoutine;
    Vector3 playerDir;

    float angleToPlayer;
    bool lookingAround;
    bool isAttacking;
    bool isRunning;

    void Awake()
    {
        enemyRef = GetComponent<enemyReferences>();
    }
    void Start()
    {
        armCollider.enabled = false;
    }
    void Update()
    {
        Movement();
    }
    void Movement()
    {
        if (target != null)
        {
            bool inRange = Vector3.Distance(transform.position, target.position) <= detectionRange;
            if (inRange && CanSeePlayer())
            {
                enemyRef.agent.speed = 5f;
                enemyRef.animator.SetBool("Running", true);
                if (pathRoutine == null)
                {
                    pathRoutine = StartCoroutine(UpdatePath());
                }
            }
            else
            {
                enemyRef.agent.speed = 2f;
                enemyRef.animator.SetBool("Running", false);
                if (pathRoutine != null)
                {
                    StopCoroutine(pathRoutine);
                    pathRoutine = null;
                }
                CheckRoam();
            }
            enemyRef.animator.SetFloat("Speed", enemyRef.agent.velocity.magnitude);
        }
    }
    void GoToNextPoint()
    {
        if (roamPos.Length == 0) return;
        enemyRef.agent.destination = roamPos[currentPos].position;
        currentPos = (currentPos + 1) % roamPos.Length;
    }
    void CheckRoam()
    {
        if (lookingAround) return;

        if (!enemyRef.agent.pathPending && enemyRef.agent.remainingDistance <= 0.5f)
        {
            StartCoroutine(LookingAround());
        }
    }
    void RotateToPlayer()
    {
        Vector3 lookPos = target.position - transform.position;
        lookPos.y = 0;
        Quaternion rot = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 0.2f);
    }
    bool CanSeePlayer()
    {
        playerDir = (target.transform.position - transform.position).normalized;
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
    IEnumerator UpdatePath()
    {
        while (true)
        {
            float dist = Vector3.Distance(transform.position, target.position);

            if (dist <= 2f)
            {
                enemyRef.agent.isStopped = true;
                if(!isAttacking)
                {
                    StartCoroutine(Attacking());
                }
            }
            else
            {
                enemyRef.agent.isStopped = false;
                enemyRef.agent.SetDestination(target.position);              
            }
            yield return new WaitForSeconds(pathToDelay);

        }
    }
    IEnumerator LookingAround()
    {
        lookingAround = true;

        enemyRef.agent.isStopped = true;

        enemyRef.animator.SetBool("IsLooking", true);

        yield return new WaitForSeconds(5f);

        enemyRef.animator.SetBool("IsLooking", false);

        enemyRef.agent.isStopped = false;

        GoToNextPoint();
        lookingAround = false;
    }
    IEnumerator Attacking()
    {
        isAttacking = true;
        enemyRef.animator.SetTrigger("Attack");
        yield return new WaitForSeconds(1.5f);
        isAttacking = false;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           // playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //playerInRange = false;
        }
    }
    public void TakeDamage(int amount)
    {
        HP -= amount;
        Debug.Log(HP);
        if(HP <= 0)
        {
            Debug.Log("DEAD");
        }
    }
}