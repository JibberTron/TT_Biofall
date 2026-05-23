using UnityEngine;

public class enemyAttack : MonoBehaviour
{
    enemyReferences enemyRef;

    [Header("-----AI Attack Stats-----")]
    [Range(2.6f, 5.2f)][SerializeField] float attackDelay = 2.6f;
    [Range(0, 100)][SerializeField] int damage = 5;
    [HideInInspector]public float AttackDelay => attackDelay;
    [HideInInspector]public float AttackDistance => attackDistance;

    float attackDistance = 1.75f;

    void Start()
    {
        enemyRef = GetComponentInParent<enemyReferences>();
    }
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Vector3 hitPosition = other.ClosestPoint(transform.position);
        Vector3 hitDirection = (other.transform.position - transform.position).normalized;

        iDamage dam = other.GetComponent<iDamage>();
        if (dam == null) return;
        
        dam.TakeDamage(damage);

    }
    public void Attack(bool _isTrue)
    {
        enemyRef.EAnims.Attack(_isTrue);
    }

}
