using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class enemyMovement : MonoBehaviour
{
    enemyReferences enemyRef;

    [Header("-----AI Movement Stats-----")]
    [Range(2.5f, 5)][SerializeField] float roamSpeed = 2.5f;
    [Range(6, 10)][SerializeField] float chaseSpeed = 6f;

    int currentPos;

    Coroutine pathRoutine;
    GameObject player = null;

    float origSpeed;
    float pathToDelay = 0.2f;

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
        player = enemyRef.Player;
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
    void goToNextPoint()
    {
        if (enemyRef.RoamPos.Count == 0) return;

        enemyRef.Agent.destination = enemyRef.RoamPos[currentPos].position;
        currentPos = (currentPos + 1) % enemyRef.RoamPos.Count;
    }
    public  void AddSoundPoints(NoiseData _sound)
    {
        enemyRef.SoundPoints.Add(_sound);
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
        float speed = enemyRef.Agent.velocity.magnitude;
        speed = Mathf.Min(speed, 6f);
        enemyRef.EAnims.SetMovement(speed);
    }
    public void ResetMovement()
    {
        enemyRef.EAnims.SetMovement(0);
    }
    public void Investigate(bool _isTrue)
    {
        enemyRef.EAnims.Investigate(_isTrue);
    }
    public void RotateToPlayer(Transform _loc)
    {
        Vector3 look = player.transform.position - _loc.position;
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
        enemyRef.Agent.ResetPath();
        enemyRef.Agent.SetDestination(_loc);
    }
    public void MoveToDest(Vector3 _loc)
    {
        Stop(false);
        enemyRef.Agent.SetDestination(_loc);
    }
    IEnumerator UpdatePath()
    {
        while (shouldUpdatePath)
        {
            if (enemyRef.Agent.isOnNavMesh)
            {
                enemyRef.Agent.SetDestination(player.transform.position);
            }

            yield return new WaitForSeconds(pathToDelay) ;
        }
        pathRoutine = null;
    }
    public void StopChase() => stopChase();
    public void GoToNextPoint() => goToNextPoint();
    public void Chase() => chase();
}
