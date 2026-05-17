using UnityEngine;

public class BatterySocket : MonoBehaviour, IInteractable
{
    [Header("Battery Requirement")]
    [SerializeField] private int requiredBatteries = 1;
    [SerializeField] private int insertedBatteries = 0;

    [Header("Power Target")]
    [SerializeField] private PowerReceiver powerReceiver;

    [Header("Behavior")]
    [SerializeField] private bool allowBatteryRemoval = true;

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

        if (insertedBatteries < requiredBatteries)
        {
            InsertBattery(inventory);
        }
        else if (allowBatteryRemoval)
        {
            RemoveBattery(inventory);
        }
        else
        {
            Debug.Log("Battery socket is fully powered and battery removal is disabled.");
        }

        UpdatePowerState();
    }

    private void InsertBattery(PlayerInventory inventory)
    {
        if (inventory.TryUseBattery())
        {
            insertedBatteries++;
            Debug.Log($"Inserted battery: {insertedBatteries}/{requiredBatteries}");
        }
    }

    private void RemoveBattery(PlayerInventory inventory)
    {
        if (insertedBatteries <= 0)
        {
            return;
        }

        insertedBatteries--;
        inventory.AddBatteries(1);

        Debug.Log($"Removed battery: {insertedBatteries}/{requiredBatteries}");
    }

    private void UpdatePowerState()
    {
        bool isPowered = insertedBatteries >= requiredBatteries;

        if (powerReceiver != null)
        {
            powerReceiver.SetPowered(isPowered);
        }
    }
}