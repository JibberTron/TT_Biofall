using System.Collections;
using UnityEngine;
using UnityEngine.AI;
//TODO: Make it to where the enemy will only go by the patrol points closest to the player
//  by getting the distance from patrol point to player we can figure out which ones are closest and only patrol those areas
// if there is only one patrol point in the area maybe make a random patrol in situations like that
public class enemyMovement : MonoBehaviour
{
    enemyReferences enemyRef;

    [Header("-----AI Movement Stats-----")]
    [Range(1, 5)][SerializeField] float roamSpeed = 1.5f;
    [Range(1, 10)][SerializeField] float chaseSpeed = 2f;
    [SerializeField] float pathToDelay = 0.2f;

    int currentPos;

    Coroutine pathRoutine;

    float origSpeed;

    bool shouldUpdatePath = true;

    [HideInInspector]public float OrigSpeed => origSpeed;
    public float RoamSpeed => roamSpeed;
    public float ChaseSpeed => chaseSpeed;
    void Awake()
    {
        enemyRef = GetComponent<enemyReferences>();
    }
    void Start()
    {
        origSpeed = enemyRef.Agent.speed;
    }
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
    bool CheckDistanceToRoamPoint(Vector3 _loc)
    {
        float distance = Vector3.Distance(enemyRef.Target.position, _loc);
        if(distance <= 100f)
        {
            return true;
        }
        
        return false;
    }
    void goToNextPoint()
    {
        if (enemyRef.RoamPos.Length == 0) return;
        
        enemyRef.Agent.destination = enemyRef.RoamPos[Random.Range(0, enemyRef.RoamPos.Length)].position;
        currentPos = (currentPos + 1) % enemyRef.RoamPos.Length;
    }
    public  void AddSoundPoints(NoiseData _pos)
    {
        enemyRef.SoundPoints.Add(_pos);
    }
    public void RemoveSoundPoints()
    {
        enemyRef.SoundPoints.Clear();
    }
    public void EnableAgentRotation(bool _should)
    {
        enemyRef.Agent.updateRotation = _should;
    }
    public void SetMovement()
    {
        enemyRef.EAnims.SetMovement(enemyRef.Agent.velocity.magnitude);
    }
    public void Investigate(bool _isTrue)
    {
        enemyRef.EAnims.Investigate(_isTrue);
    }
    public void RotateToPlayer(Transform _loc)
    {
        Vector3 look = enemyRef.Target.position - _loc.position;
        look.y = 0;

        if (look.sqrMagnitude < 0.001f) return;

        Quaternion lookRot = Quaternion.LookRotation(look);

        _loc.rotation = Quaternion.Slerp(_loc.rotation, lookRot, 10f * Time.deltaTime);
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
 
            yield return new WaitForSeconds(pathToDelay) ;
        }
        pathRoutine = null;
    }

    // ==================================================================
    public void MoveToNoise(Vector3 _loc)
    {
        Stop(false);

        if (NavMesh.SamplePosition(_loc, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            enemyRef.Agent.SetDestination(hit.position);
        }
        else
        {
            enemyRef.Agent.SetDestination(_loc);
        }
    }
    // ====================================================================
    public void StopChase() => stopChase();
    public void GoToNextPoint() => goToNextPoint();
    public void Chase() => chase();
}
