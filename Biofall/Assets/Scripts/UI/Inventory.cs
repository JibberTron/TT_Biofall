using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    public enum InventoryItems
    {
        Gun,
        Batteries,
        Pebbles
    }

    [Header("Inventory")]
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private GunManager gunManager;

    [Header("Slot Images")]
    [SerializeField] private Image gunSlot;
    [SerializeField] private Image batterySlot;
    [SerializeField] private Image pebbleSlot;

    [Header("Item Icons")]
    [SerializeField] private Image gunIcon;
    [SerializeField] private Image batteryIcon;
    [SerializeField] private Image pebbleIcon;

    [Header("Text")]
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private TMP_Text batteryText;
    [SerializeField] private TMP_Text pebbleText;

    private InventoryItems currentSlot = InventoryItems.Gun;
    private bool hasGun;
    private Gun gun;

    private void Update()
    {
        Scroll();
        Slot();
        UpdateInventoryUI();

        if(GameObject.FindFirstObjectByType<GunManager>()  != null )
        {
            hasGun = true;
        }
    }

    void Scroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if(scroll > 0f)
        {
            CycleSlot(1);
        }

        else if(scroll < 0f)
        {
            CycleSlot(-1);
        }
    }

    bool CanUseSlot(InventoryItems item)
    {
        switch (item)
        {
            case InventoryItems.Gun:
                return gunManager != null && gunManager.HasGun();

            case InventoryItems.Batteries:
                return true;

            case InventoryItems.Pebbles:
                return true;

            default:
                return false;
        }
    }

    void CycleSlot(int direction)
    {
        int itemsCount = System.Enum.GetValues(typeof(InventoryItems)).Length;

        for (int i = 0; i < itemsCount; i++)
        {
            int nextItem = ((int)currentSlot + direction + itemsCount) % itemsCount;

            currentSlot = (InventoryItems)nextItem;

            if (CanUseSlot(currentSlot))
            {
                Debug.Log("Current Slot:" + currentSlot);
                return;
            }
        }
    }

    void Slot()
    {
        gunSlot.gameObject.SetActive(false);
        batterySlot.gameObject.SetActive(false);
        pebbleSlot.gameObject.SetActive(false);

        switch(currentSlot)
        {
            case InventoryItems.Gun:
                gunSlot.gameObject.SetActive(true);
                break;
            case InventoryItems.Batteries:
                batterySlot.gameObject.SetActive(true);
                break;
            case InventoryItems.Pebbles:
                pebbleSlot.gameObject.SetActive(true);
                break;
        }
    }

    void UpdateInventoryUI()
    {
        batteryText.text = inventory.Batteries.ToString();
        pebbleText.text = inventory.Pebbles.ToString();

        batteryIcon.enabled = true;
        pebbleIcon.enabled = inventory.Pebbles > 0;

        batteryText.enabled = true;
        pebbleText.enabled = inventory.Pebbles > 0;

        if (gunManager != null && gunManager.HasGun())
        {
            gunIcon.enabled = true;

            if (gun != null)
            {
                ammoText.text = $"{gun.GetCurrentAmmo()} / {gun.GetTotalAmmo()}";
            }

            ammoText.enabled = true;
        }
        else
        {
            gunIcon.enabled = false;
            ammoText.enabled = false;
        }
    }
}
