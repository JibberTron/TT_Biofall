using UnityEngine;

public class DoorInteract : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject door;

    public void Interact()
    {
        Debug.Log("Door opened!");
        door.SetActive(false);
    }
}
