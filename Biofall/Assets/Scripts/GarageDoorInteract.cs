using UnityEngine;

public class GarageDoorInteract : MonoBehaviour, IInteractable
{
    [SerializeField] AudioSource sfx_Sounds;
    [SerializeField] AudioClip doorOpen;
    [SerializeField] AudioClip doorClosed;

    [SerializeField] float openHeight = 3f;
    [SerializeField] float speed = 2f;
    [SerializeField] float interactCooldown = 1f;

    PowerReceiver blocker;

    float cooldownTimer;
    Vector3 closedPos;
    Vector3 openPos;

    float t = 0f;      
    bool isOpen = false;
    bool isBlocked;

    void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + Vector3.up * openHeight;
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        t = Mathf.MoveTowards(t, isOpen ? 1f : 0f, speed * Time.deltaTime);

        transform.position = Vector3.Lerp(closedPos, openPos, t);
    }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Blocked"))
        {
            blocker = other.GetComponent<PowerReceiver>();
            if (blocker == null) return;
            if (blocker.IsPowered)
            {
                isBlocked = false;
            }
            else
            {
                isBlocked = true;
            }
        }
    }
    public void Interact()
    {
        if (isBlocked) return;
        if (cooldownTimer > 0f) return;

        isOpen = !isOpen;
        cooldownTimer = interactCooldown;

        if (sfx_Sounds != null)
        {
            sfx_Sounds.PlayOneShot(isOpen ? doorOpen : doorClosed);
        }
    }
}