using UnityEngine;
using UnityEngine.UI;

public class OralAntibodyPickup : MonoBehaviour, IInteractable
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
                inf.UseOralAntibody();
                Debug.Log("Oral antibody picked up!");
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered trigger: " + other.name);

        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            Debug.Log("Player near antibody");
        }
            
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNearby = false;
    }
}
