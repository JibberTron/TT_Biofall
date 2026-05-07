using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DoorInteract : MonoBehaviour, IInteractable
{
    [SerializeField] Transform hinge; 

    bool isOpen = false;

    public void Interact()
    {
       if(isOpen)
        {
            hinge.localRotation = Quaternion.Euler(0, 0, 0);
            isOpen = false;

            Debug.Log("Door Closed");
        }
       else
        {
            hinge.localRotation = Quaternion.Euler(0, 90, 0);
            isOpen = true;

            Debug.Log("Door Opened");
        }
    }
    
}
