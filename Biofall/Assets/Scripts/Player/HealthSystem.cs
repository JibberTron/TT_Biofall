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
    [SerializeField] float hitAnimLength = 1.033f;

    [Header("References")]
    [SerializeField] Animator animator;
    [SerializeField] PlayerController playerController;
    [SerializeField] InfectionHallucination hallucination;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] hurtClips;
    [SerializeField] AudioClip deathClip;
    [SerializeField] float hurtVolume = 0.25f;
    [SerializeField] float deathVolume = 0.25f;

    int hitStack;
    float timeSinceLastHit;
    bool recentlyHit;
    float hitAnimCooldown;
    public bool isHit;
    bool isDead;

    InfectionSystem infectionSystem;
    RagdollController ragdollController;

    void Start()
    {
        infectionSystem = GetComponent<InfectionSystem>();
        ragdollController = GetComponentInChildren<RagdollController>();
        UpdatePlayerUI();
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
        if (isDead) return;

        if (hallucination != null && hallucination.IsHallucinating())
        {
            Die();
            return;
        }

        int scaledDamage = Mathf.RoundToInt(amount * Mathf.Pow(1.2f, hitStack));

        hitStack++;
        timeSinceLastHit = 0f;
        recentlyHit = true;

        currentHealth -= scaledDamage;
        currentHealth = Mathf.Max(currentHealth, 0);

        UpdatePlayerUI();

        if (infectionSystem != null)
            infectionSystem.AddInfection(scaledDamage * 0.2f);

        TriggerHitAnimation(hitFromPosition);
        PlayHurtSound();

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

    void PlayHurtSound()
    {
        if (audioSource == null || hurtClips.Length == 0) return;
        AudioClip clip = hurtClips[Random.Range(0, hurtClips.Length)];
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(clip, hurtVolume);
        audioSource.pitch = 1f;
    }

    void Recover()
    {
        if (hitStack > 1) return;

        currentHealth += recoveryAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (audioSource != null && deathClip != null)
            audioSource.PlayOneShot(deathClip, deathVolume);

        if (playerController != null)
            playerController.enabled = false;

        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
            cc.enabled = false;

        if (ragdollController != null)
            ragdollController.TriggerDeath();

        CameraOrbit cam = Camera.main.GetComponent<CameraOrbit>();
        if (cam != null)
            cam.TriggerDeathCam();

        Invoke(nameof(GameOver), 3f);
    }

    void GameOver()
    {
        Debug.Log("Game Over");
        Gamemanager.instance.GameOver();
    }

    public void UpdatePlayerUI()
    {
        if (Gamemanager.instance != null)
            Gamemanager.instance.playerHPBar.fillAmount = (float)currentHealth / maxHealth;
    }
}