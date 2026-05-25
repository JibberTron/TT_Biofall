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

    [Header("inventory")]
    [SerializeField] private PlayerInventory inventory;

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


    private InventoryItems currentSlot;

 
    // Update is called once per frame
    void Update()
    {
        Scroll();
        Slot();
        UpdateInventoryUI();
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

    void CycleSlot(int direction)
    {
        int itemsCount = System.Enum.GetValues(typeof(InventoryItems)).Length;

        int nextItem = ((int)currentSlot + direction + itemsCount) % itemsCount;

        currentSlot = (InventoryItems)nextItem;

        Debug.Log("Current Slot: " + currentSlot);
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
        ammoText.text = inventory.Ammo.ToString();
        batteryText.text = inventory.Batteries.ToString();
        pebbleText.text = inventory.Pebbles.ToString();

        gunIcon.enabled = inventory.Ammo > 0;
        batteryIcon.enabled = inventory.Batteries > 0;
        pebbleIcon.enabled = inventory.Pebbles > 0;

        ammoText.enabled = inventory.Ammo > 0;
        batteryText.enabled = inventory.Batteries > 0;
        pebbleText.enabled = inventory.Pebbles > 0;
    }
}
