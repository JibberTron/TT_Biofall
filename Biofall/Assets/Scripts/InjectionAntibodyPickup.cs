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

                Debug.Log("Infection Antibody picked up!");

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
