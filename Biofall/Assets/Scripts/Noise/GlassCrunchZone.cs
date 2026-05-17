using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GlassCrunchZone : MonoBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private bool allowPlayerTrigger = true;
    [SerializeField] private bool allowEnemyTrigger = true;

    [Header("Movement Detection")]
    [SerializeField] private float movementThreshold = 0.02f;
    [SerializeField] private float crunchInterval = 0.45f;

    [Header("Noise Settings")]
    [SerializeField] private float soundRadius = 6f;
    [SerializeField] private float soundDuration = 0.5f;
    [SerializeField] private float attractionStrength = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] crunchClips;

    private Transform activeActor;
    private Vector3 lastActorPosition;
    private Coroutine crunchRoutine;
    private int lastClipIndex = -1;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsValidActor(other))
        {
            return;
        }

        activeActor = other.transform;
        lastActorPosition = activeActor.position;

        if (crunchRoutine == null)
        {
            crunchRoutine = StartCoroutine(CrunchRoutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (activeActor == null || other.transform != activeActor)
        {
            return;
        }

        activeActor = null;

        if (crunchRoutine != null)
        {
            StopCoroutine(crunchRoutine);
            crunchRoutine = null;
        }
    }

    private IEnumerator CrunchRoutine()
    {
        while (activeActor != null)
        {
            float movementAmount = Vector3.Distance(activeActor.position, lastActorPosition);

            if (movementAmount > movementThreshold)
            {
                PlayCrunch();
                EmitNoise();

                lastActorPosition = activeActor.position;
            }

            yield return new WaitForSeconds(crunchInterval);
        }
    }

    private void PlayCrunch()
    {
        if (audioSource == null || crunchClips == null || crunchClips.Length == 0)
        {
            return;
        }

        int randomIndex;

        do
        {
            randomIndex = Random.Range(0, crunchClips.Length);
        }
        while (crunchClips.Length > 1 && randomIndex == lastClipIndex);

        lastClipIndex = randomIndex;

        audioSource.PlayOneShot(crunchClips[randomIndex]);
    }

    private void EmitNoise()
    {
        NoiseData noiseData = new NoiseData(
            transform.position,
            soundRadius,
            soundDuration,
            attractionStrength,
            gameObject
        );

        NoiseSystem.CreateNoise(noiseData);
    }

    private bool IsValidActor(Collider other)
    {
        if (allowPlayerTrigger && other.CompareTag(playerTag))
        {
            return true;
        }

        if (allowEnemyTrigger && other.CompareTag(enemyTag))
        {
            return true;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, soundRadius);
    }
}