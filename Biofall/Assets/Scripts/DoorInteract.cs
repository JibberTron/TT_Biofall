using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Unity.VisualScripting.Member;

public class DoorInteract : MonoBehaviour, IInteractable
{
    [SerializeField] Transform hinge;
    NavMeshObstacle obst;

    bool isOpen = false;
    bool doorGuard;

    void Start()
    {
        obst = GetComponentInChildren<NavMeshObstacle>();
        obst.enabled = false;
    }
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
        if (!doorGuard)
        {
            Quaternion beginRot = hinge.localRotation;
            Quaternion endRot = Quaternion.Euler(0, 90, 0);
            float dur = 1f;
            float time = 0;

            isOpen = true;
            obst.enabled = false;

            while (time < dur)
            {
                time += Time.deltaTime;
                float t = time / dur;

                Debug.Log("Door Opened");
                hinge.localRotation = Quaternion.Slerp(beginRot, endRot, time);

                yield return null;
            }
        }
        doorGuard = true;
    }
    IEnumerator CloseDoor()
    {
        if (doorGuard)
        {
            Quaternion beginRot = hinge.localRotation;
            Quaternion endRot = Quaternion.Euler(0, 0, 0);

            float dur = 1f;
            float time = 0;
            isOpen = false;

            while (time < dur)
            {
                time += Time.deltaTime;
                float t = time / dur;

                Debug.Log("Door Closed");
                hinge.localRotation = Quaternion.Slerp(beginRot, endRot, time);
                yield return null;
            }
            obst.enabled = true;
        }
        doorGuard = false;
    }
}
