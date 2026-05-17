using UnityEngine;

public class enemyHealth : MonoBehaviour, iDamage
{
    enemyReferences enemyRef;

    [Header("Health")]
    [SerializeField] int currentHP = 100;
    [SerializeField] int maxHP = 10;

    bool isDead = false;
    bool incapInvinsibility = false;

    public int CurrentHP { get { return currentHP; }  set { currentHP = value; } }
    public bool IsDead{ get { return isDead; } set { isDead = value; } }
    public bool IncapInvinsibility { get { return incapInvinsibility; } set { incapInvinsibility = value; } }
   
    void Start()
    {
        enemyRef = GetComponent<enemyReferences>();
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
        }
    }
}
