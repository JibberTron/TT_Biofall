using UnityEngine;

public class enemyAttack : MonoBehaviour
{
    enemyReferences enemyRef;

    [Header("-----AI Attack Stats-----")]
    [SerializeField] Collider armCollider;
    [Range(2.6f, 5.2f)][SerializeField] float attackDelay = 2.6f;
    [Range(0, 100)][SerializeField] int damage = 5;
    [HideInInspector]public float AttackDelay => attackDelay;
    [HideInInspector]public float AttackDistance => attackDistance;

    float attackDistance = 1.3f;
    bool canHit;

    void Start()
    {
        enemyRef = GetComponentInParent<enemyReferences>();
    }
    void Update()
    {
        
    }
    public void EnableCollider()
    {
        canHit = true;
    }
    public void DisableCollider()
    {
        canHit = false;
    }
    public void EnableDamage()
    {
        armCollider.enabled = true;
    }
    public void DisableDamage()
    {
        armCollider.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!canHit) return;

        Vector3 hitPosition = other.ClosestPoint(transform.position);
        Vector3 hitDirection = (other.transform.position - transform.position).normalized;

        iDamage dam = other.GetComponentInParent<iDamage>();
        if (dam == null) return;
        
        dam.TakeDamage(damage);

    }
    public void Attack(bool _isTrue)
    {
        enemyRef.EAnims.Attack(_isTrue);
    }

}
