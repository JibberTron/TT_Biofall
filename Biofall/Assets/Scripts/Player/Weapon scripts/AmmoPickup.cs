using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] int ammoAmount = 6;
    [SerializeField] float bobSpeed = 1f;
    [SerializeField] float bobHeight = 0.1f;
    [SerializeField] float rotateSpeed = 50f;

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Bob up and down
        transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobHeight;

        // Slowly rotate
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Gun gun = other.GetComponent<Gun>();
        if (gun == null)
            gun = other.GetComponentInChildren<Gun>();

        if (gun != null)
        {
            gun.AddAmmo(ammoAmount);
            Destroy(gameObject);
        }
    }
}