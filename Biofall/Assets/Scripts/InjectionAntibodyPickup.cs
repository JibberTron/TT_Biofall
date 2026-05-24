using UnityEngine;
using UnityEngine.UI;

public class InjectionAntibodyPickup : MonoBehaviour, IInteractable
{
    private bool playerNearby;

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }
    public void Interact()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if(player != null)
        {
            InfectionSystem inf = player.GetComponent<InfectionSystem>();

            if(inf != null)
            {
                inf.UseInjectionAntibody();

                PlayerInventory inventory = player.GetComponent<PlayerInventory>();

                if (inventory != null)
                {
                    inventory.AddInjectionAntibody(1);
                    Debug.Log("Injection Antibody Count: " + inventory.InjectionAntibodies);
                }

                Debug.Log("Injection Antibody picked up!");

                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNearby = false;
    }
}
