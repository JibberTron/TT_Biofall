using UnityEngine;

public class InfectionSystem : MonoBehaviour
{
    [Header("Infection")]
    public float maxInfection = 100f;
    public float currentInfection = 0f;

    [Header("Tick Rate")]
    [SerializeField] float baseTickRate = 0.1f;
    float currentTickRate;

    [Header("Antibody - Oral")]
    [SerializeField] float oralRecovery = 10f;
    [SerializeField] float oralSlowAmount = 0.05f;
    [SerializeField] float oralSlowDuration = 30f;

    [Header("Antibody - Injection")]
    [SerializeField] float injectionStopDuration = 60f;

    float slowTimer;
    float stopTimer;

    HealthSystem healthSystem;

    void Start()
    {
        currentTickRate = baseTickRate;
        healthSystem = GetComponent<HealthSystem>();
    }

    void Update()
    {
        HandleTimers();
        Tick();

        if (Input.GetKeyDown(KeyCode.O))
            UseOralAntibody();

        if (Input.GetKeyDown(KeyCode.I))
            UseInjectionAntibody();
    }

    void HandleTimers()
    {
        if (stopTimer > 0f)
        {
            stopTimer -= Time.deltaTime;
            currentTickRate = 0f;
            return;
        }

        if (slowTimer > 0f)
        {
            slowTimer -= Time.deltaTime;
            currentTickRate = baseTickRate - oralSlowAmount;
        }
        else
        {
            currentTickRate = baseTickRate;
        }
    }

    void Tick()
    {
        if (currentTickRate <= 0f) return;

        currentInfection += currentTickRate * Time.deltaTime;
        currentInfection = Mathf.Min(currentInfection, maxInfection);

        if (currentInfection >= maxInfection)
        {
            currentInfection = maxInfection;
            if (healthSystem != null)
                healthSystem.TakeDamage(healthSystem.currentHealth);
        }
    }

    public void AddInfection(float amount)
    {
        currentInfection += amount;
        currentInfection = Mathf.Min(currentInfection, maxInfection);
    }

    public void UseOralAntibody()
    {
        currentInfection -= oralRecovery;
        currentInfection = Mathf.Max(currentInfection, 0f);
        slowTimer = oralSlowDuration;
    }

    public void UseInjectionAntibody()
    {
        currentInfection = 0f;
        stopTimer = injectionStopDuration;
    }

    void Die()
    {
        Debug.Log("Player succumbed to infection.");
    }
}