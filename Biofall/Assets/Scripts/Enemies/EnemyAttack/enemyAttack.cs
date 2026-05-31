using UnityEngine;

public class enemyAttack : MonoBehaviour
{
    enemyReferences enemyRef;

    [Header("-----AI Attack Stats-----")]
    float attackDelay = 1.1f;
    [Range(0, 100)][SerializeField] int damage = 5;
    [HideInInspector]public float AttackDelay => attackDelay;
    [HideInInspector]public float AttackDistance => attackDistance;

    float attackDistance = 2f;

    void Awake()
    {
        enemyRef = GetComponentInParent<enemyReferences>();
        if(enemyRef == null)
        {
            Debug.Log("Enemy Reference == null");
            return;
        }
    }
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        iDamage dam = other.GetComponent<iDamage>();
        if (dam == null) return;
        
        dam.TakeDamage(damage);

    }
    public void Attack(bool _isTrue)
    {
        enemyRef.EAnims.Attack(_isTrue);
    }

}
