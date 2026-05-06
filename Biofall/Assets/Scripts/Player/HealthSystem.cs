using UnityEngine;

public class HealthSystem : MonoBehaviour, iDamage
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth = 100;

    [Header("Recovery")]
    [SerializeField] float hitWindow = 3f;
    [SerializeField] int recoveryAmount = 10;

    int hitStack;
    float timeSinceLastHit;
    bool recentlyHit;

    InfectionSystem infectionSystem;

    void Start()
    {
        infectionSystem = GetComponent<InfectionSystem>();
    }

    void Update()
    {
        if (recentlyHit)
        {
            timeSinceLastHit += Time.deltaTime;

            if (timeSinceLastHit >= hitWindow)
            {
                Recover();
                recentlyHit = false;
                hitStack = 0;
                timeSinceLastHit = 0f;
            }
        }

        
    }

    public void TakeDamage(int amount)
    {
        int scaledDamage = Mathf.RoundToInt(amount * Mathf.Pow(1.2f, hitStack));

        hitStack++;
        timeSinceLastHit = 0f;
        recentlyHit = true;

        currentHealth -= scaledDamage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (infectionSystem != null)
            infectionSystem.AddInfection(scaledDamage * 0.2f);

        

        if (currentHealth <= 0)
            Die();
    }

    void Recover()
    {
        if (hitStack > 1)
        {
           
            return;
        }

        currentHealth += recoveryAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
       
    }

    void Die()
    {
        Debug.Log("Player died.");
    }
}