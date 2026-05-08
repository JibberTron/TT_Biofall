using UnityEngine;

public class OralAntibodyPickup : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if(player != null)
        {
            InfectionSystem inf = player.GetComponent<InfectionSystem>();

            if(inf != null)
            {
                inf.UseOralAntibody();
                Debug.Log("Oral antibody used!");
                Destroy(gameObject);
            }
        }
    }
}
