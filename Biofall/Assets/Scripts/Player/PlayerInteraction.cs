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
        Ray ray = new Ray(playerCamera.transform.position + playerCamera.transform.forward * 0.5f, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDist, interactMask))
        {
            Debug.Log("Ray hit: " + hit.collider.name);

            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if(interactable != null )
            {
                Debug.Log("Interactable found");
                interactable.Interact();
            }
            else
            {
                Debug.Log("Hit object is NOT interactable");
            }
        }
        else
        {
            Debug.Log("Ray hit nothing!");
        }
    }
}
