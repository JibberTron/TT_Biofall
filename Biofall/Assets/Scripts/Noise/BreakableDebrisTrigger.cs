using UnityEngine;

public class BreakableDebrisTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField, Range(0f, 100f)] private float activationChance = 100f;

    [Header("Object States")]
    [SerializeField] private GameObject wholeObject;
    [SerializeField] private GameObject brokenObjectParent;

    [Header("Physics")]
    [SerializeField] private Rigidbody[] brokenRigidbodies;
    [SerializeField] private float explosionForce = 2f;
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float upwardModifier = 0.25f;

    [Header("Noise")]
    [SerializeField] private NoiseMaker noiseMaker;

    private bool isBroken;

    private void Reset()
    {
        noiseMaker = GetComponent<NoiseMaker>();
    }

    private void Start()
    {
        if (brokenObjectParent != null)
        {
            brokenObjectParent.SetActive(false);
        }

        if (wholeObject != null)
        {
            wholeObject.SetActive(true);
        }

        foreach (Rigidbody rb in brokenRigidbodies)
        {
            if (rb == null) continue;

            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        if (isBroken)
        {
            return;
        }

        TryBreak();
    }

    private void TryBreak()
    {
        float roll = Random.Range(0f, 100f);

        if (roll > activationChance)
        {
            Debug.Log($"{gameObject.name} did not break. Roll: {roll:F1}");
            return;
        }

        BreakObject();
    }

    private void BreakObject()
    {
        if (isBroken)
        {
            return;
        }

        isBroken = true;

        if (wholeObject != null)
        {
            wholeObject.SetActive(false);
        }

        if (brokenObjectParent != null)
        {
            brokenObjectParent.SetActive(true);
        }

        foreach (Rigidbody rb in brokenRigidbodies)
        {
            if (rb == null) continue;

            rb.isKinematic = false;
            rb.useGravity = true;

            rb.AddExplosionForce(
                explosionForce,
                transform.position,
                explosionRadius,
                upwardModifier,
                ForceMode.Impulse
            );
        }

        if (noiseMaker != null)
        {
            noiseMaker.ActivateNoise();
        }

        Debug.Log($"{gameObject.name} broke and created noise.");
    }
}