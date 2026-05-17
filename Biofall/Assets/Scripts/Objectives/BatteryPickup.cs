using UnityEngine;

public class BatteryPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private int amount = 1;

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

        inventory.AddBatteries(amount);
        Destroy(gameObject);
    }
}