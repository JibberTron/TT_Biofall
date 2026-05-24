using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    public enum ItemType
    {
        Flashlight,
        Key
    }

    [SerializeField] private ItemType itemType;
    [SerializeField] string itemName;

    public void Interact()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();

        switch (itemType)
        {
            case ItemType.Flashlight:
                if (inventory != null)
                {
                    inventory.GiveFlashlight();
                }

                Debug.Log("Flashlight picked up!");
                break;

            case ItemType.Key:
                if (inventory != null)
                {
                    inventory.AddKey(1);
                }

                Debug.Log("Key picked up!");
                break;
        }

        Destroy(gameObject);
    }
}
