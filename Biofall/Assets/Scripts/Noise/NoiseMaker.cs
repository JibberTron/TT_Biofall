using UnityEngine;

public class NoiseMaker : MonoBehaviour, IInteractable
{
    [Header("Noise Settings")]
    [SerializeField] private float soundRadius = 10f;
    [SerializeField] private float soundDuration = 3f;
    [SerializeField] private float attractionStrength = 1f;

    [Header("Activation Settings")]
    [SerializeField] private bool canInteract = true;
    [SerializeField] private bool toggleable = false;
    [SerializeField] private bool startsActive = false;
    [SerializeField] private bool breakable = false;
    [SerializeField] private bool destroyWhenBroken = false;

    [Header("Feedback")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip activationSound;
    [SerializeField] private GameObject visualFeedbackObject;

    private bool isActive;
    private bool isBroken;
    private Coroutine noiseRoutine;

    private void Start()
    {
        if (visualFeedbackObject != null)
        {
            visualFeedbackObject.SetActive(false);
        }

        if (startsActive)
        {
            ActivateNoise();
        }
    }

    public void Interact()
    {
        if (!canInteract || isBroken)
        {
            return;
        }

        if (toggleable)
        {
            if (isActive)
            {
                DeactivateNoise();
            }
            else
            {
                ActivateNoise();
            }
        }
        else
        {
            ActivateNoise();
        }
    }

    public void ActivateNoise()
    {
        if (isBroken)
        {
            return;
        }

        isActive = true;

        if (visualFeedbackObject != null)
        {
            visualFeedbackObject.SetActive(true);
        }

        if (audioSource != null && activationSound != null)
        {
            audioSource.clip = activationSound;
            audioSource.Play();
        }

        NoiseData noiseData = new NoiseData(
            transform.position,
            soundRadius,
            soundDuration,
            attractionStrength,
            gameObject
        );

        NoiseSystem.CreateNoise(noiseData);

        if (noiseRoutine != null)
        {
            StopCoroutine(noiseRoutine);
        }

        noiseRoutine = StartCoroutine(NoiseDurationRoutine());
    }

    public void DeactivateNoise()
    {
        isActive = false;

        if (visualFeedbackObject != null)
        {
            visualFeedbackObject.SetActive(false);
        }

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        if (noiseRoutine != null)
        {
            StopCoroutine(noiseRoutine);
            noiseRoutine = null;
        }
    }

    public void BreakNoiseMaker()
    {
        if (!breakable)
        {
            return;
        }

        isBroken = true;
        DeactivateNoise();

        if (destroyWhenBroken)
        {
            Destroy(gameObject);
        }
    }

    private System.Collections.IEnumerator NoiseDurationRoutine()
    {
        yield return new WaitForSeconds(soundDuration);

        if (!toggleable)
        {
            DeactivateNoise();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, soundRadius);
    }
}