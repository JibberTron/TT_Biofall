using UnityEngine;

public class TVStaticTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField] private string playerTag = "Player";

    [SerializeField, Range(0f, 100f)]
    private float activationChance = 20f;

    [SerializeField] private bool canRetryUntilTriggered = true;

    [Header("TV Objects")]
    [SerializeField] private GameObject tvOffObject;
    [SerializeField] private GameObject tvOnObject;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip staticClip;

    [Header("Noise")]
    [SerializeField] private NoiseMaker noiseMaker;

    [Header("TV Behavior")]
    [SerializeField] private bool turnOffAfterDuration = true;
    [SerializeField] private float activeDuration = 5f;

    private bool hasTriggered;

    private void Start()
    {
        SetTVState(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        if (hasTriggered)
        {
            return;
        }

        TryActivateTV();
    }

    private void TryActivateTV()
    {
        float roll = Random.Range(0f, 100f);

        Debug.Log($"TV rolled {roll:F1}");

        if (roll > activationChance)
        {
            if (!canRetryUntilTriggered)
            {
                hasTriggered = true;
            }

            return;
        }

        ActivateTV();
    }

    private void ActivateTV()
    {
        hasTriggered = true;

        SetTVState(true);

        if (audioSource != null && staticClip != null)
        {
            audioSource.clip = staticClip;
            audioSource.loop = true;
            audioSource.Play();
        }

        if (noiseMaker != null)
        {
            noiseMaker.ActivateNoise();
        }

        Debug.Log($"{gameObject.name} TV activated.");

        if (turnOffAfterDuration)
        {
            Invoke(nameof(DeactivateTV), activeDuration);
        }
    }

    private void DeactivateTV()
    {
        SetTVState(false);

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        Debug.Log($"{gameObject.name} TV deactivated.");
    }

    private void SetTVState(bool isOn)
    {
        if (tvOffObject != null)
        {
            tvOffObject.SetActive(!isOn);
        }

        if (tvOnObject != null)
        {
            tvOnObject.SetActive(isOn);
        }
    }
}