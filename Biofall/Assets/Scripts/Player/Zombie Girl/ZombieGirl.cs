using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class ZombieGirl : MonoBehaviour, iDamage
{
    [Header("Stats")]
    [SerializeField] int maxHealth = 100;
    [SerializeField] int damage = 15;

    [Header("Detection")]
    [SerializeField] float sightRange = 15f;
    [SerializeField] float sightAngle = 90f;
    [SerializeField] LayerMask sightMask;

    [Header("Combat")]
    [SerializeField] float attackRange = 1.5f;
    [SerializeField] float attackCooldown = 0.5f;
    [SerializeField] float attackHitTime = 0.3f;

    [Header("Scream")]
    [SerializeField] float screamDuration = 1.5f;

    [Header("Movement")]
    [SerializeField] float chaseSpeed = 4f;

    [Header("References")]
    [SerializeField] Animator animator;
    [SerializeField] NavMeshAgent agent;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip screamClip;
    [SerializeField] AudioClip runClip;
    [SerializeField] AudioClip deathClip;
    [SerializeField] float sfxVolume = 0.8f;

    int currentHealth;
    bool isDead;
    bool hasSeenPlayer;
    bool isAttacking;

    Transform player;
    HealthSystem playerHealth;

    void Start()
    {
        currentHealth = maxHealth;
        agent.speed = chaseSpeed;
        agent.stoppingDistance = attackRange - 0.2f;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<HealthSystem>();
        }
    }

    void Update()
    {
        if (isDead) return;

        if (!hasSeenPlayer)
        {
            CheckSight();
            return;
        }

        Chase();
        TryAttack();
        UpdateAnimator();
    }

    void CheckSight()
    {
        if (player == null) return;

        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        if (distToPlayer > sightRange) return;

        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > sightAngle * 0.5f) return;

        if (Physics.Raycast(transform.position + Vector3.up, dirToPlayer, distToPlayer, sightMask)) return;

        hasSeenPlayer = true;
        if (audioSource != null && screamClip != null)
            audioSource.PlayOneShot(screamClip, sfxVolume);
        StartCoroutine(ScreamThenChase());
    }

    IEnumerator ScreamThenChase()
    {
        agent.isStopped = true;
        animator.SetTrigger("Scream");

        yield return new WaitForSeconds(screamDuration);

        agent.isStopped = false;

        if (audioSource != null && runClip != null)
        {
            audioSource.clip = runClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void Chase()
    {
        if (player == null) return;
        if (isAttacking) return;
        agent.SetDestination(player.position);
    }

    void TryAttack()
    {
        if (player == null) return;
        if (isAttacking) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > attackRange) return;

        StartCoroutine(AttackSequence());
    }

    IEnumerator AttackSequence()
    {
        isAttacking = true;

        animator.SetTrigger("Attack");
        agent.isStopped = true;

        yield return new WaitForSeconds(attackHitTime);

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= attackRange && playerHealth != null)
            playerHealth.TakeDamage(damage, transform.position);

        yield return new WaitForSeconds(attackCooldown);

        agent.isStopped = false;
        isAttacking = false;
    }

    void UpdateAnimator()
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
            Die();
    }

    public void TakeDamage(int amount, Vector3 hitFromPosition)
    {
        TakeDamage(amount);
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (audioSource != null)
        {
            audioSource.loop = false;
            audioSource.Stop();
            if (deathClip != null)
                audioSource.PlayOneShot(deathClip, sfxVolume);
        }

        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.enabled = false;

        animator.enabled = false;

        ZombieGirlRagdoll ragdoll = GetComponentInChildren<ZombieGirlRagdoll>();
        if (ragdoll != null)
            ragdoll.TriggerDeath();

        
    }
}