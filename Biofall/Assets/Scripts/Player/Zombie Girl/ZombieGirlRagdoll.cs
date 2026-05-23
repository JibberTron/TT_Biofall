using UnityEngine;

public class ZombieGirlRagdoll : MonoBehaviour
{
    Rigidbody[] ragdollBodies;
    Collider[] ragdollColliders;

    void Start()
    {
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();
        DisableRagdoll();
    }

    void DisableRagdoll()
    {
        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }
        foreach (var col in ragdollColliders)
        {
            if (col.gameObject == gameObject) continue;
            col.enabled = false;
        }
    }

    public void TriggerDeath()
    {
        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        foreach (var col in ragdollColliders)
        {
            if (col.gameObject == gameObject) continue;
            col.enabled = true;
        }
    }
}