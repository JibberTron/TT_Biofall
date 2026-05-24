using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] float interactDist = 5f;
    [SerializeField] Camera playerCamera;
    [SerializeField] LayerMask interactMask;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        Ray ray = new Ray(
            playerCamera.transform.position + playerCamera.transform.forward * 0.5f,
            playerCamera.transform.forward
        );

        int raycastMask = ~LayerMask.GetMask("Player");

        if (Physics.Raycast(ray, out RaycastHit hit, interactDist, raycastMask))
        {
            Debug.Log("Ray hit: " + hit.collider.name);

            bool hitIsInteractableLayer = ((1 << hit.collider.gameObject.layer) & interactMask) != 0;

            if (!hitIsInteractableLayer)
            {
                Debug.Log("Interaction blocked by: " + hit.collider.name);
                return;
            }

            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                Debug.Log("Interactable found");
                interactable.Interact();
            }
            else
            {
                Debug.Log("Hit object is on Interactable layer but has no IInteractable.");
            }
        }
    }
}
