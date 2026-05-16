using UnityEngine;

public class enemyHealth : MonoBehaviour, iDamage
{
    [Header("Health")]
    [SerializeField] public int currentHP = 100;
    [SerializeField] int maxHP = 10;

    public bool isDead = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeDamage(int amount)
    {
        if (isDead) return;
        currentHP -= amount;

        if (currentHP <= 0)
        {
            isDead = true;
        }
    }
}
