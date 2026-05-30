using UnityEngine;
using System.Collections;

public class ZombieGirlCaged : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] float sightRange = 10f;
    [SerializeField] float sightAngle = 90f;
    [SerializeField] LayerMask sightMask;

    [Header("Timings")]
    [SerializeField] float screamDuration = 1.5f;
    [SerializeField] float headbuttInterval = 1.2f;

    [Header("References")]
    [SerializeField] Animator animator;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip screamClip;
    [SerializeField] AudioClip headbuttClip;
    [SerializeField] float sfxVolume = 0.8f;

    [Header("Blood")]
    [SerializeField] GameObject bloodSplatPrefab;
    [SerializeField] Transform glassPoint;
    [SerializeField] int maxSplats = 5;

    [Header("Easter Egg")]
    [SerializeField] AudioClip thrillerClip;
    [SerializeField] float thrillerDelay = 60f;
    [SerializeField] float thrillerDuration = 30f;
    [SerializeField] Light thrillerSpotlight;
    [SerializeField] float colorChangeSpeed = 0.3f;

    bool hasSeenPlayer;
    bool thrillerTriggered;
    Transform player;
    int splatCount;
    float playerInSightTimer;
    Coroutine headbuttCoroutine;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (thrillerTriggered || player == null) return;

        if (!hasSeenPlayer)
        {
            CheckSight();
            return;
        }

        if (CanSeePlayer())
        {
            playerInSightTimer += Time.deltaTime;
            if (playerInSightTimer >= thrillerDelay)
                TriggerThriller();
        }
        else
        {
            playerInSightTimer = 0f;
        }
    }

    bool CanSeePlayer()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        if (distToPlayer > sightRange) return false;

        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > sightAngle * 0.5f) return false;

        if (Physics.Raycast(transform.position + Vector3.up, dirToPlayer, distToPlayer, sightMask)) return false;

        return true;
    }

    void CheckSight()
    {
        if (!CanSeePlayer()) return;

        hasSeenPlayer = true;
        headbuttCoroutine = StartCoroutine(ScreamThenHeadbutt());
    }

    IEnumerator ScreamThenHeadbutt()
    {
        animator.SetTrigger("Scream");
        if (audioSource != null && screamClip != null)
            audioSource.PlayOneShot(screamClip, sfxVolume);

        yield return new WaitForSeconds(screamDuration);

        while (true)
        {
            animator.SetTrigger("Headbutt");
            yield return new WaitForSeconds(headbuttInterval);
        }
    }

    void TriggerThriller()
    {
        thrillerTriggered = true;

        if (headbuttCoroutine != null)
            StopCoroutine(headbuttCoroutine);

        animator.applyRootMotion = false;
        animator.ResetTrigger("Headbutt");
        animator.SetTrigger("Thriller");

        if (thrillerSpotlight != null)
        {
            thrillerSpotlight.enabled = true;
            StartCoroutine(CycleSpotlightColors());
        }

        if (audioSource != null && thrillerClip != null)
        {
            audioSource.Stop();
            audioSource.clip = thrillerClip;
            audioSource.loop = true;
            audioSource.Play();
        }

        StartCoroutine(ThrillerDeath());
    }

    IEnumerator CycleSpotlightColors()
    {
        Color[] colors = new Color[]
        {
            Color.red,
            Color.blue,
            Color.green,
            Color.yellow,
            Color.magenta,
            Color.cyan
        };

        int index = 0;

        while (true)
        {
            thrillerSpotlight.color = colors[index];
            index = (index + 1) % colors.Length;
            yield return new WaitForSeconds(colorChangeSpeed);
        }
    }

    IEnumerator ThrillerDeath()
    {
        yield return new WaitForSeconds(thrillerDuration);

        audioSource.Stop();
        animator.enabled = false;

        ZombieGirlRagdoll ragdoll = GetComponentInChildren<ZombieGirlRagdoll>();
        if (ragdoll != null)
            ragdoll.TriggerDeath();

        yield return new WaitForSeconds(2f);

        if (thrillerSpotlight != null)
            thrillerSpotlight.enabled = false;
    }

    public void OnHeadbuttImpact()
    {
        if (audioSource != null && headbuttClip != null)
            audioSource.PlayOneShot(headbuttClip, sfxVolume);

        if (bloodSplatPrefab != null && glassPoint != null && splatCount < maxSplats)
        {
            Quaternion randomRot = glassPoint.rotation * Quaternion.Euler(0, 0, Random.Range(0f, 360f));
            Instantiate(bloodSplatPrefab, glassPoint.position, randomRot);
            splatCount++;
        }
    }
}