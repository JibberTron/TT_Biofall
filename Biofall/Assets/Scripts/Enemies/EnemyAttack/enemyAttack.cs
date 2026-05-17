using UnityEngine;

public class enemyAttack : MonoBehaviour
{
    enemyReferences enemyRef;
    public enemyReferences EnemyRef => EnemyRef;

    [Header("-----AI Attack Stats-----")]
    [SerializeField] Collider armCollider;
    [SerializeField] float attackDelay = 1f;

    void Start()
    {
        
    }
    void Update()
    {
        
    }


}
