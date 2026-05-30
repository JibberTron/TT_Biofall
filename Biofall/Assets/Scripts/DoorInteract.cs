using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Unity.VisualScripting.Member;

public class DoorInteract : MonoBehaviour, IInteractable
{
    [SerializeField] Transform hinge;
    [SerializeField] AudioSource sfx_Sounds;
    [SerializeField] AudioClip doorOpen;
    [SerializeField] AudioClip doorClosed;
    [SerializeField] BoxCollider enemyOpen;

    NavMeshObstacle obst;
    PowerReceiver blocker;

    bool isOpen = false;
    bool doorGuard;
    bool isBlocked;

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
    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Blocked"))
        {         
            blocker = other.GetComponent<PowerReceiver>();
            if (blocker == null) return;
            if(blocker.IsPowered)
            {
                isBlocked = false;
                enemyOpen.enabled = true;
            }
            else
            {
                isBlocked = true;
                enemyOpen.enabled = false;

                if (isOpen)
                {
                    StartCoroutine(CloseDoor());
                }
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (isBlocked) return;
        if(other.CompareTag("Enemy"))
        {
            StartCoroutine(OpenDoor());
        }
    }
    IEnumerator OpenDoor()
    {
        if (!doorGuard && !isBlocked)
        {
            Quaternion beginRot = hinge.localRotation;
            Quaternion endRot = Quaternion.Euler(0, 90, 0);
            float dur = 1f;
            float time = 0;

            isOpen = true;
            obst.enabled = false;

            sfx_Sounds.clip = doorOpen;
            sfx_Sounds.Play();

            while (time < dur)
            {
                time += Time.deltaTime;
                float t = time / dur;

                Debug.Log("Door Opened");
               
                hinge.localRotation = Quaternion.Slerp(beginRot, endRot, t);

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
            sfx_Sounds.clip = doorClosed;
            sfx_Sounds.Play();
            
            while (time < dur)
            {
                time += Time.deltaTime;
                float t = time / dur;

                Debug.Log("Door Closed");
                hinge.localRotation = Quaternion.Slerp(beginRot, endRot, t);
                yield return null;
            }
            obst.enabled = true;
        }
        doorGuard = false;
    }
}
