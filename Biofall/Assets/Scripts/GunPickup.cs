using UnityEngine;

public class GunPickup : MonoBehaviour, IInteractable
{
    private GunManager gunManager;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if(player != null)
        {
            gunManager = player.GetComponent<GunManager>();
        }
    }

    public void Interact()
    {
        if(gunManager != null)
        {
            gunManager.PickUpGun();

            Debug.Log("Picked up gun!");

            Destroy(gameObject);
        }
    }
}
