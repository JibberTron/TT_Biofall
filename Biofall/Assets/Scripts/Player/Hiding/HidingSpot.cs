using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    [SerializeField] KeyCode hideKey = KeyCode.F;

    HidingSystem hidingSystem;
    bool playerInRange;

    void OnTriggerEnter(Collider other)
    {
        HidingSystem hs = other.GetComponent<HidingSystem>();
        if (hs != null)
        {
            hidingSystem = hs;
            playerInRange = true;
            Debug.Log("Press F to hide");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<HidingSystem>() != null)
        {
            if (hidingSystem != null && hidingSystem.IsHiding())
                hidingSystem.ForceExitHiding();

            hidingSystem = null;
            playerInRange = false;
        }
    }

    void Update()
    {
        if (playerInRange && hidingSystem != null && Input.GetKeyDown(hideKey))
            hidingSystem.ToggleHiding();
    }
}