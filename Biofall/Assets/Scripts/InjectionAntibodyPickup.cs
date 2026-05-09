using UnityEngine;

public class InjectionAntibodyPickup : MonoBehaviour, IInteractable
{
   public void Interact()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if(player != null)
        {
            InfectionSystem inf = player.GetComponent<InfectionSystem>();

            if(inf != null)
            {
                inf.UseInjectionAntibody();

                Debug.Log("Infection Antibody used!");

                Destroy(gameObject);
            }
        }
    }
}
