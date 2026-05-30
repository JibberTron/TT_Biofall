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

    bool hasSeenPlayer;
    Transform player;
    int splatCount;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (hasSeenPlayer || player == null) return;
        CheckSight();
    }

    void CheckSight()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        if (distToPlayer > sightRange) return;

        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > sightAngle * 0.5f) return;

        if (Physics.Raycast(transform.position + Vector3.up, dirToPlayer, distToPlayer, sightMask)) return;

        hasSeenPlayer = true;
        StartCoroutine(ScreamThenHeadbutt());
    }

    IEnumerator ScreamThenHeadbutt()
    {
        // Scream
        animator.SetTrigger("Scream");
        if (audioSource != null && screamClip != null)
            audioSource.PlayOneShot(screamClip, sfxVolume);

        yield return new WaitForSeconds(screamDuration);

        // Just loop the animation, sound fires from Animation Event
        while (true)
        {
            animator.SetTrigger("Headbutt");
            yield return new WaitForSeconds(headbuttInterval);
        }
    }

    // Called by Animation Event at the exact impact frame
    public void OnHeadbuttImpact()
    {
        if (audioSource != null && headbuttClip != null)
            audioSource.PlayOneShot(headbuttClip, sfxVolume);

        
        if (bloodSplatPrefab != null && glassPoint != null && splatCount < maxSplats)
        {
            Quaternion randomRot = glassPoint.rotation * Quaternion.Euler(0, 0, Random.Range(0f, 360f));
            GameObject splat = Instantiate(bloodSplatPrefab, glassPoint.position, randomRot);
            splatCount++;
        }
    }
}