using UnityEngine;
using TMPro;

public class InventoryHUD : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;

    [Header("Text")]
    [SerializeField] private TMP_Text batteryText;
    [SerializeField] private TMP_Text pebbleText;
    [SerializeField] private TMP_Text ammoText;

    private void Update()
    {
        if (inventory == null)
        {
            return;
        }

        if (batteryText != null)
        {
            batteryText.text = $"Batteries: {inventory.Batteries}";
        }

        if (pebbleText != null)
        {
            pebbleText.text = $"Pebbles: {inventory.Pebbles}";
        }

        if (ammoText != null)
        {
            ammoText.text = $"Ammo: {inventory.Ammo}";
        }
    }
}