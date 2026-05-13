using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DoorInteract : MonoBehaviour, IInteractable
{
    [SerializeField] Transform hinge;

    bool isOpen = false;

    public void Interact()
    {
        if (isOpen)
        {
            StartCoroutine(CloseDoor());
        }
        else
        {
            StartCoroutine(OpenDoor());

        }
    }
    IEnumerator OpenDoor()
    {
        Quaternion beginRot = hinge.localRotation;
        Quaternion endRot = Quaternion.Euler(0, 90, 0);
        float dur = 1f;
        float time = 0;
        while (time < dur)
        {
            time += Time.deltaTime;
            isOpen = true;

            Debug.Log("Door Opened");
            hinge.localRotation = Quaternion.Slerp(beginRot, endRot, time);
            yield return null;
        }
    }
    IEnumerator CloseDoor()
    {
        Quaternion beginRot = hinge.localRotation;
        Quaternion endRot = Quaternion.Euler(0, 0, 0);
        float dur = 1f;
        float time = 0;
        while (time < dur)
        {
            time += Time.deltaTime;
            isOpen = false;

            Debug.Log("Door Closed");
            hinge.localRotation = Quaternion.Slerp(beginRot, endRot, time);
            yield return null;
        }
    }
}
