using UnityEngine;
using System.Collections;

public class RagdollController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Animator animator;

    [Header("Settings")]
    [SerializeField] float ragdollDuration = 1.5f;
    [SerializeField] float recoveryDuration = 1f;
    [SerializeField] float hitForce = 50f;

    Rigidbody[] ragdollBodies;
    Collider[] ragdollColliders;
    Transform[] bones;
    Vector3[] bonePositions;
    Quaternion[] boneRotations;
    bool isRagdolling;
    Transform hipsTransform;
    Vector3 hipsOffset;

    void Start()
    {
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();
        bones = GetComponentsInChildren<Transform>();
        bonePositions = new Vector3[bones.Length];
        boneRotations = new Quaternion[bones.Length];

        DisableRagdoll();

        foreach (var rb in ragdollBodies)
        {
            if (rb.name.Contains("Hips"))
            {
                hipsTransform = rb.transform;
                hipsOffset = hipsTransform.localPosition;
                break;
            }
        }
    }

    void SavePose()
    {
        for (int i = 0; i < bones.Length; i++)
        {
            bonePositions[i] = bones[i].localPosition;
            boneRotations[i] = bones[i].localRotation;
        }
    }

    void DisableRagdoll()
    {
        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }
        foreach (var col in ragdollColliders)
            col.enabled = false;

        if (animator != null)
            animator.enabled = true;
    }

    void EnableRagdoll()
    {
        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
        }
        foreach (var col in ragdollColliders)
            col.enabled = true;

        if (animator != null)
            animator.enabled = false;
    }

    public void TriggerRagdoll(Vector3 hitDirection)
    {
        if (isRagdolling) return;
        StartCoroutine(RagdollSequence(hitDirection));
    }

    public void TriggerDeath()
    {
        StopAllCoroutines();
        isRagdolling = true;
        EnableRagdoll();
    }

    IEnumerator RagdollSequence(Vector3 hitDirection)
    {
        isRagdolling = true;

        SavePose();
        EnableRagdoll();

        Rigidbody hips = null;
        foreach (var rb in ragdollBodies)
        {
            if (rb.name.Contains("Hips"))
            {
                hips = rb;
                break;
            }
        }

        if (hips != null)
            hips.AddForce(hitDirection * hitForce, ForceMode.Impulse);

        
        float elapsed2 = 0f;
        while (elapsed2 < ragdollDuration)
        {
            elapsed2 += Time.deltaTime;
            if (hipsTransform != null)
                hipsTransform.position = transform.parent.position + hipsOffset;
            yield return null;
        }

        Vector3[] ragdollPos = new Vector3[bones.Length];
        Quaternion[] ragdollRot = new Quaternion[bones.Length];
        for (int i = 0; i < bones.Length; i++)
        {
            ragdollPos[i] = bones[i].localPosition;
            ragdollRot[i] = bones[i].localRotation;
        }

        DisableRagdoll();

        float elapsed = 0f;
        while (elapsed < recoveryDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / recoveryDuration;

            for (int i = 0; i < bones.Length; i++)
            {
                bones[i].localPosition = Vector3.Lerp(ragdollPos[i], bonePositions[i], t);
                bones[i].localRotation = Quaternion.Lerp(ragdollRot[i], boneRotations[i], t);
            }

            yield return null;
        }

        isRagdolling = false;
    }

    public bool IsRagdolling() => isRagdolling;
}