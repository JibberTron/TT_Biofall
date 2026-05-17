using UnityEngine;

public class PebbleThrower : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private PlayerInventory inventory;

    [Header("Throw Settings")]
    [SerializeField] private GameObject pebblePrefab;
    [SerializeField] private Transform throwOrigin;
    [SerializeField] private float throwForce = 12f;
    [SerializeField] private KeyCode throwKey = KeyCode.Mouse0;
    [SerializeField] private float aimDistance = 100f;

    private void Awake()
    {
        if (inventory == null)
        {
            inventory = GetComponent<PlayerInventory>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(throwKey))
        {
            ThrowPebble();
        }
    }

    private void ThrowPebble()
    {
        if (inventory == null || !inventory.TryUsePebble())
        {
            return;
        }

        if (pebblePrefab == null || throwOrigin == null)
        {
            Debug.LogWarning("Pebble prefab or throw origin is missing.");
            return;
        }

        Camera cam = Camera.main;

        Vector3 targetPoint;

        if (cam != null && Physics.Raycast(cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)), out RaycastHit hit, aimDistance))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = throwOrigin.position + throwOrigin.forward * aimDistance;
        }

        Vector3 throwDirection = (targetPoint - throwOrigin.position).normalized;

        GameObject pebble = Instantiate(pebblePrefab, throwOrigin.position, Quaternion.LookRotation(throwDirection));

        Rigidbody rb = pebble.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        }

        Debug.Log($"Threw pebble. Pebbles left: {inventory.Pebbles}");
    }
}