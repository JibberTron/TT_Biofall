using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [Range(1, 500)][SerializeField] int roamDist = 20;
    Vector3 startingPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        roam();
    }
    void roam()
    {
        agent.stoppingDistance = 0;

        Vector3 ranPos = Random.insideUnitSphere * roamDist;
        //Debug.Log(ranPos);
        ranPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);
        //Debug.Log(hit.position);
        agent.SetDestination(ranPos);
        if(agent.transform.localPosition == ranPos)
        {
            agent.isStopped = true;
        }
    }
}
