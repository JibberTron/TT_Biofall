using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThrownPebble : MonoBehaviour
{
    [Header("Noise Settings")]
    [SerializeField] private float soundRadius = 8f;
    [SerializeField] private float soundDuration = 1.5f;
    [SerializeField] private float attractionStrength = 1f;

    [Header("Impact Settings")]
    [SerializeField] private float minimumImpactVelocity = 1f;
    [SerializeField] private bool makeNoiseOnlyOnce = true;
    [SerializeField] private float destroyAfterSeconds = 6f;

    [Header("Feedback")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip impactSound;

    private bool hasMadeNoise;

    private void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (makeNoiseOnlyOnce && hasMadeNoise)
        {
            return;
        }

        if (collision.relativeVelocity.magnitude < minimumImpactVelocity)
        {
            return;
        }

        EmitNoise();
    }

    private void EmitNoise()
    {
        hasMadeNoise = true;

        if (audioSource != null && impactSound != null)
        {
            audioSource.PlayOneShot(impactSound);
        }

        NoiseData noiseData = new NoiseData(
            transform.position,
            soundRadius,
            soundDuration,
            attractionStrength,
            gameObject
        );

        NoiseSystem.CreateNoise(noiseData);

        Debug.Log($"Pebble made noise at {transform.position}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, soundRadius);
    }
}