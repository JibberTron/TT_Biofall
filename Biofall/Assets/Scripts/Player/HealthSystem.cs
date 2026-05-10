using UnityEngine;

public class HealthSystem : MonoBehaviour, iDamage
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth = 100;

    [Header("Recovery")]
    [SerializeField] float hitWindow = 3f;
    [SerializeField] int recoveryAmount = 10;

    [Header("Hit Animation")]
    [SerializeField] float hitAnimLength = 2f;

    [Header("References")]
    [SerializeField] Animator animator;

    int hitStack;
    float timeSinceLastHit;
    bool recentlyHit;
    float hitAnimCooldown;
    public bool isHit;

    InfectionSystem infectionSystem;

    void Start()
    {
        infectionSystem = GetComponent<InfectionSystem>();
    }

    void Update()
    {
        if (hitAnimCooldown > 0f)
        {
            hitAnimCooldown -= Time.deltaTime;
            if (hitAnimCooldown <= 0f)
                isHit = false;
        }

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
        TakeDamage(amount, transform.position);
    }

    public void TakeDamage(int amount, Vector3 hitFromPosition)
    {
        int scaledDamage = Mathf.RoundToInt(amount * Mathf.Pow(1.2f, hitStack));

        hitStack++;
        timeSinceLastHit = 0f;
        recentlyHit = true;

        currentHealth -= scaledDamage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (infectionSystem != null)
            infectionSystem.AddInfection(scaledDamage * 0.2f);

        TriggerHitAnimation(hitFromPosition);

        if (currentHealth <= 0)
            Die();
    }

    void TriggerHitAnimation(Vector3 hitFromPosition)
    {
        if (animator == null) return;
        if (hitAnimCooldown > 0f) return;

        Vector3 localHitDir = transform.InverseTransformPoint(hitFromPosition);

        if (localHitDir.x >= 0)
            animator.SetTrigger("HitRight");
        else
            animator.SetTrigger("HitLeft");

        hitAnimCooldown = hitAnimLength;
        isHit = true;
    }

    void Recover()
    {
        if (hitStack > 1) return;

        currentHealth += recoveryAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    void Die()
    {
        Debug.Log("Player died.");
    }
}