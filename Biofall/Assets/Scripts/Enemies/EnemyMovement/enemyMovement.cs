using UnityEngine;
using System.Collections;
//TODO: Make it to where the enemy will only go by the patrol points closest to the player
//  by getting the distance from patrol point to player we can figure out which ones are closest and only patrol those areas
// if there is only one patrol point in the area maybe make a random patrol in situations like that
public class enemyMovement : MonoBehaviour
{
    enemyReferences enemyRef;
    public enemyReferences EnemyRef => enemyRef;

    [Header("Stats")]
    [SerializeField] float pathToDelay = 0.2f;
    [SerializeField] Collider armCollider;
    int currentPos;

    Coroutine pathRoutine;

    float origSpeed;
    public float OrigSpeed;

    bool shouldUpdatePath = true;

    void Awake()
    {
        enemyRef = GetComponent<enemyReferences>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        origSpeed = enemyRef.Agent.speed;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void chase()
    {
        EnableNav(true);
        Stop(false);
        ShouldUpdatePath(true);

        if (pathRoutine != null) return;
        pathRoutine = StartCoroutine(UpdatePath());
    }
    void stopChase()
    {
        if (pathRoutine == null) return;

       StopCoroutine(pathRoutine);
       pathRoutine = null;
    }
    void goToNextPoint()
    {
        if (enemyRef.RoamPos.Length == 0) return;
        enemyRef.Agent.destination = enemyRef.RoamPos[Random.Range(0, enemyRef.RoamPos.Length)].position;
        currentPos = (currentPos + 1) % enemyRef.RoamPos.Length;
    }
    public void SetSpeed(float _value)
    {
        enemyRef.Agent.speed = _value;
    }
    public void Stop(bool _should)
    {
        enemyRef.Agent.isStopped = _should;
    }
    public void EnableNav(bool _should)
    {
        enemyRef.Agent.enabled = _should;
    }
    public void ShouldUpdatePath(bool _should)
    {
        shouldUpdatePath = _should;
    }
    public void MoveTo(Vector3 _loc)
    {
        Stop(false);
        enemyRef.Agent.SetDestination(_loc);
    }
    IEnumerator UpdatePath()
    {
        while (shouldUpdatePath)
        {
            if(Vector3.Distance(enemyRef.Agent.destination, enemyRef.Target.position) > 0.3f)
            {
                enemyRef.Agent.SetDestination(enemyRef.Target.position);
            }
 
            yield return new WaitForSeconds(0.1f) ;
        }
        pathRoutine = null;
    }

    public void StopChase() => stopChase();
    public void GoToNextPoint() => goToNextPoint();
    public void Chase() => chase();
}
