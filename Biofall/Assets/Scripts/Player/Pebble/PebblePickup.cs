using UnityEngine;

public class PebblePickup : MonoBehaviour, IInteractable
{
    [SerializeField] private int pebbleAmount = 3;
    [SerializeField] private bool destroyAfterPickup = true;

    public void Interact()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("No player found.");
            return;
        }

        PlayerInventory inventory = player.GetComponent<PlayerInventory>();

        if (inventory == null)
        {
            Debug.LogWarning("Player has no PlayerInventory.");
            return;
        }

        inventory.AddPebbles(pebbleAmount);

        if (destroyAfterPickup)
        {
            Destroy(gameObject);
        }
    }
}