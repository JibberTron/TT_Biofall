using UnityEngine;
using System.Collections;

public class enemyMovement : MonoBehaviour, ISound
{
    enemyReferences enemyRef;
    public enemyReferences EnemyRef => enemyRef;

    [Header("Stats")]
    [SerializeField] float pathToDelay = 0.2f;
    [SerializeField] Collider armCollider;
    int currentPos;

    Coroutine pathRoutine;
 
    bool isAttacking;

    void Awake()
    {
        enemyRef = GetComponent<enemyReferences>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Chase()
    {
        if (pathRoutine == null)
        {
            pathRoutine = StartCoroutine(UpdatePath());
        }
    }
    public void StopChase()
    {
        if (pathRoutine != null)
        {
            StopCoroutine(pathRoutine);
            pathRoutine = null;
        }
    }
    public void GoToNextPoint()
    {
        if (enemyRef.RoamPos.Length == 0) return;
        enemyRef.Agent.destination = enemyRef.RoamPos[Random.Range(0, enemyRef.RoamPos.Length)].position;
        currentPos = (currentPos + 1) % enemyRef.RoamPos.Length;
    }
    public void Stop(bool _should)
    {
        enemyRef.Agent.isStopped = _should;
    }
    IEnumerator UpdatePath()
    {
        while (true)
        {
            enemyRef.Agent.isStopped = false;
            enemyRef.Agent.SetDestination(enemyRef.Target.position);

            yield return new WaitForSeconds(pathToDelay);
        }
    }
    public void MoveTo(Vector3 _loc)
    {
        enemyRef.Agent.isStopped = false;
        enemyRef.Agent.SetDestination(_loc);
    }
    public void ReactToSound(Sound _sound)
    {

        if (_sound.soundType == Sound.SoundType.DEFAULT)
        {
            MoveTo(_sound.position);
        }
        Debug.Log($"Responding to sound {_sound.position} at Range {_sound.range}");
    }
}
