using UnityEngine;

public class AmmoPickup : MonoBehaviour, IInteractable
{
    [SerializeField] int ammoAmount = 6;
    [SerializeField] float bobSpeed = 1f;
    [SerializeField] float bobHeight = 0.1f;
    [SerializeField] float rotateSpeed = 50f;
    [SerializeField] bool destroyAfterPickup = true;

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    public void Interact()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Gun gun = player.GetComponent<Gun>();
        if (gun == null)
            gun = player.GetComponentInChildren<Gun>();

        if (gun == null) return;

        gun.AddAmmo(ammoAmount);

        if (destroyAfterPickup)
            Destroy(gameObject);
    }
}