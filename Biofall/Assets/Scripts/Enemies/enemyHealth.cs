using UnityEngine;

public class enemyHealth : MonoBehaviour, iDamage
{
    enemyReferences enemyRef;

    [Header("-----AI Health-----")]
    [Range(1, 150)][SerializeField] int maxHP = 10;

    int currentHP = 100;

    bool isDead = false;
    bool incapInvinsibility = false;

    public int MaxHP => maxHP;
    public int CurrentHP { get { return currentHP; }  set { currentHP = value; } }
    public bool IsDead{ get { return isDead; } set { isDead = value; } }
    public bool IncapInvinsibility { get { return incapInvinsibility; } set { incapInvinsibility = value; } }
   
    void Awake()
    {
        enemyRef = GetComponentInParent<enemyReferences>();
        if (enemyRef == null)
        {
            Debug.Log("Enemy Reference == null");
            return;
        }
    }
    void Start()
    {
        currentHP = maxHP;
    }
    void Update()
    {
        
    }
    public void Death(bool _isTrue)
    {
        enemyRef.EAnims.Death(_isTrue);
    }
    public void StandUp(bool _isTrue)
    {
        enemyRef.EAnims.StandUp(_isTrue);
    }
    public void TakeDamage(int amount)
    {
        if (isDead || incapInvinsibility) return;
        
        currentHP -= amount;

        if (currentHP <= 0)
        {
            isDead = true;
            enemyRef.Sounds.PlayClip(enemyRef.Sounds.EnemyDeath);
            return;
        }
        enemyRef.Sounds.PlayClip(enemyRef.Sounds.EnemyHit);

    }
}
