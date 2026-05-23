using UnityEngine;
using TMPro;

public class BatterySocket : MonoBehaviour, IInteractable
{
    [Header("Battery Requirement")]
    [SerializeField] private int requiredBatteries = 1;
    [SerializeField] private int insertedBatteries = 0;
    [SerializeField] private TMP_Text batteryCountText;

    [Header("Power Target")]
    [SerializeField] private PowerReceiver[] powerReceivers;

    [Header("Behavior")]
    [SerializeField] private bool allowBatteryRemoval = true;

    private void Start()
    {
        UpdateBatteryText();
        UpdatePowerState();
    }

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
        UpdateBatteryText();
    }

    private void InsertBattery(PlayerInventory inventory)
    {
        if (inventory.TryUseBattery())
        {
            insertedBatteries++;
            Debug.Log($"Inserted battery: {insertedBatteries}/{requiredBatteries}");
        }
    }

    private void UpdateBatteryText()
    {
        if (batteryCountText != null)
        {
            batteryCountText.text = $"{insertedBatteries}/{requiredBatteries}";
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

        foreach (PowerReceiver receiver in powerReceivers)
        {
            if (receiver != null)
            {
                receiver.SetPowered(isPowered);
            }
        }
    }
}