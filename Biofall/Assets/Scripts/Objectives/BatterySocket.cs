using UnityEngine;
using TMPro;

public class BatterySocket : MonoBehaviour, IInteractable
{
    [Header("Battery Requirement")]
    [SerializeField] private int requiredBatteries = 1;
    [SerializeField] private int insertedBatteries = 0;
    [SerializeField] private TMP_Text batteryCountText;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip insertBatterySound;
    [SerializeField] private AudioClip removeBatterySound;
    [SerializeField] private AudioClip poweredOnSound;
    [SerializeField] private AudioClip failedSound;

    [Header("Power Target")]
    [SerializeField] private PowerReceiver[] powerReceivers = new PowerReceiver[0];

    [Header("Behavior")]
    [SerializeField] private bool allowBatteryRemoval = true;
    [SerializeField] bool shouldActivateEnemy = false;
    [SerializeField]enemyBrain enemyRef;

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

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void InsertBattery(PlayerInventory inventory)
    {
        if (inventory.TryUseBattery())
        {
            insertedBatteries++;
            PlaySound(insertBatterySound);

            Debug.Log($"Inserted battery: {insertedBatteries}/{requiredBatteries}");
        }
        else
        {
            PlaySound(failedSound);
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
            PlaySound(failedSound);
            return;
        }

        insertedBatteries--;
        inventory.AddBatteries(1);
        PlaySound(removeBatterySound);

        Debug.Log($"Removed battery: {insertedBatteries}/{requiredBatteries}");
    }

    private bool wasPowered;

    private void UpdatePowerState()
    {
        bool isPowered = insertedBatteries >= requiredBatteries;

        foreach (PowerReceiver receiver in powerReceivers)
        {
            if (receiver == null)
            {
                continue;
            }

            if (isPowered && !wasPowered)
            {
                receiver.AddPowerSource();
            }
            else if (!isPowered && wasPowered)
            {
                receiver.RemovePowerSource();
            }
        }
        if (isPowered)
        {
            if (shouldActivateEnemy)
            {
                enemyRef.SetActiveState(enemyBrain.EnemyActiveState.ACTIVE);
            }
        }
        if (isPowered && !wasPowered)
        {
            PlaySound(poweredOnSound);
        }

        wasPowered = isPowered;
    }
}