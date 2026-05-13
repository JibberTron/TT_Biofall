using UnityEngine;

public class LabNotePickup : MonoBehaviour, IInteractable
{
    [SerializeField] string noteTitle;

    [TextArea]
    [SerializeField] string noteText;

    [SerializeField] bool destroyAfterPickup = true;

    public void Interact()
    {
        Debug.Log("Lab Note Found: " + noteTitle);

        Debug.Log(noteText);

        if(destroyAfterPickup)
        {
            Destroy(gameObject);
        }
    }
}
