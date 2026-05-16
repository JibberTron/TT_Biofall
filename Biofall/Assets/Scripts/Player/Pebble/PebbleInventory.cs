using UnityEngine;

public class PebbleInventory : MonoBehaviour
{
    [Header("Pebble Inventory")]
    [SerializeField] private int currentPebbles = 0;
    [SerializeField] private int maxPebbles = 5;

    [Header("Throw Settings")]
    [SerializeField] private GameObject pebblePrefab;
    [SerializeField] private Transform throwOrigin;
    [SerializeField] private float throwForce = 12f;
    [SerializeField] private KeyCode throwKey = KeyCode.Mouse0;

    public int CurrentPebbles => currentPebbles;

    private void Update()
    {
        if (Input.GetKeyDown(throwKey))
        {
            ThrowPebble();
        }
    }

    public void AddPebbles(int amount)
    {
        currentPebbles = Mathf.Clamp(currentPebbles + amount, 0, maxPebbles);
        Debug.Log($"Pebbles: {currentPebbles}/{maxPebbles}");
    }

    private void ThrowPebble()
    {
        if (currentPebbles <= 0)
        {
            Debug.Log("No pebbles to throw.");
            return;
        }

        if (pebblePrefab == null || throwOrigin == null)
        {
            Debug.LogWarning("Pebble prefab or throw origin is missing.");
            return;
        }

        currentPebbles--;

        Camera cam = Camera.main;

        Vector3 targetPoint;

        if (cam != null && Physics.Raycast(cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)), out RaycastHit hit, 100f))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = cam.transform.position + cam.transform.forward * 100f;
        }

        Vector3 throwDirection = (targetPoint - throwOrigin.position).normalized;

        GameObject pebble = Instantiate(pebblePrefab, throwOrigin.position, Quaternion.LookRotation(throwDirection));

        Rigidbody rb = pebble.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        }

        Debug.Log($"Threw pebble toward reticle. Pebbles left: {currentPebbles}/{maxPebbles}");
    }
}