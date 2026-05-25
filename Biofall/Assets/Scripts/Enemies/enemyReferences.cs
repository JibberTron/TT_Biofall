using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class enemyReferences : MonoBehaviour
{
    [SerializeField] List<Transform> roamPos;
    [SerializeField] Collider armCollider;
    NavMeshAgent agent;
    Animator animator;
    enemyAnims eAnims;
    enemySounds sounds;
    List<NoiseData> soundPoints = new List<NoiseData>();
    GameObject player = null;
    HidingSystem visibility;

    public List<Transform> RoamPos => roamPos;
    public NavMeshAgent Agent => agent;
    public Animator Animator => animator;
    public enemySounds Sounds => sounds;
    public enemyAnims EAnims => eAnims;
    [HideInInspector] public List<NoiseData> SoundPoints => soundPoints;
    [HideInInspector] public GameObject Player => player;
    [HideInInspector] public HidingSystem Visibility => visibility;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        eAnims = GetComponent<enemyAnims>();
        sounds = GetComponent<enemySounds>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Start()
    {
        if (player == null)
        {
            Debug.Log("Player Game Object == null");
            return;
        }
        if (eAnims == null)
        {
            Debug.Log("No enemyAnims script on the Enemy");
            return;
        }
        if (agent == null)
        {
            Debug.Log("Nav Mesh Agent == null!");
            return;
        }
        if(roamPos.Count == 0)
        {
            Debug.Log("Roam Positions array is empty");
            return;
        }
        if (sounds == null)
        {
            Debug.Log("Enemy Sounds == null");
            return;
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
