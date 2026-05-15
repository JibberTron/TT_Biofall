using UnityEngine;

public class PebblePickup : MonoBehaviour, IInteractable
{
    [Header("Pickup Settings")]
    [SerializeField] private int pebbleAmount = 3;
    [SerializeField] private bool destroyAfterPickup = true;

    public void Interact()
    {
        PebbleInventory inventory = FindFirstObjectByType<PebbleInventory>();

        if (inventory == null)
        {
            Debug.LogWarning("No PebbleInventory found in scene.");
            return;
        }

        inventory.AddPebbles(pebbleAmount);

        Debug.Log($"Picked up {pebbleAmount} pebbles.");

        if (destroyAfterPickup)
        {
            Destroy(gameObject);
        }
    }
}