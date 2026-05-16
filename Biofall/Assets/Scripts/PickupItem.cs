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
        switch (itemType)
        {
            case ItemType.Flashlight:
                Debug.Log("Flashlight picked up!");
                break;

            case ItemType.Key:
                Debug.Log("Key picked up!");
                break;
        }

        Destroy(gameObject);
    }
}
