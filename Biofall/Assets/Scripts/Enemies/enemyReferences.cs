using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using NUnit.Framework;

public class enemyReferences : MonoBehaviour
{
    [SerializeField] List<Transform> roamPos;
    [SerializeField] Collider armCollider;
    NavMeshAgent agent;
    Animator animator;
    enemyAnims eAnims;
    List<NoiseData> soundPoints = new List<NoiseData>();
    GameObject player = null;
    HidingSystem visibility;

    public List<Transform> RoamPos => roamPos;
    public NavMeshAgent Agent => agent;
    public Animator Animator => animator;
    public enemyAnims EAnims => eAnims;
    [HideInInspector] public List<NoiseData> SoundPoints => soundPoints;
    [HideInInspector] public GameObject Player => player;
    [HideInInspector] public HidingSystem Visibility => visibility;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        eAnims = GetComponent<enemyAnims>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Start()
    {
        if (player == null)
        {
            Debug.Log("Player Game Object == null");
        }
        if (eAnims == null)
        {
            Debug.Log("No enemyAnims script on the Enemy");
        }
        if (agent == null)
        {
            Debug.Log("Nav Mesh Agent == null!");
        }
        if(roamPos.Count == 0)
        {
            Debug.Log("Roam Positions array is empty");
        }
        visibility = player.GetComponent<HidingSystem>();
    }
    public void EnableDamage()
    {
        armCollider.enabled = true;
    }
    public void DisableDamage()
    {
        armCollider.enabled = false;
    }
}
