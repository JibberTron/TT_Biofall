using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] string itemName;

    public void Interact()
    {
        Debug.Log(itemName + " picked up!");
        Destroy(gameObject);
    }
}
