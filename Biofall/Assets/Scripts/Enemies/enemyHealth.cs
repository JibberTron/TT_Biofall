using UnityEngine;

public class enemyHealth : MonoBehaviour, iDamage
{
    [Header("Health")]
    [SerializeField] int currentHP = 100;
    [SerializeField] int maxHP = 10;

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
        currentHP -= amount;
        Debug.Log(currentHP);
        if (currentHP <= 0)
        {
            Debug.Log("DEAD");
        }
    }
}
