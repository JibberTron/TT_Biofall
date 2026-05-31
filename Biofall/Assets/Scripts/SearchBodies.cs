using UnityEngine;

public class SearchBodies : MonoBehaviour, IInteractable
{
    public enum LootType
    {
        OralAntibody,
        Syringe,
        Flashlight,
        Key,
        Nothing
    }

    [SerializeField] LootType[] possibleFinds;
    [SerializeField] int searchesRemaining = 3;

    private bool hasBeenSearched = false;

    public void Interact()
    {
        if(searchesRemaining <= 0)
        {
            Debug.Log("This body has already been fully searched!");
            return;
        }

        searchesRemaining--;

        int randomIndex = Random.Range(0, possibleFinds.Length);
        LootType foundLoot = possibleFinds[randomIndex];

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();

        if(inventory != null )
        {
            switch (foundLoot)
            {
                case LootType.OralAntibody:
                    inventory.AddOralAntibody(1);
                    break;

                case LootType.Syringe:
                    inventory.AddInjectionAntibody(1);
                    break;

                case LootType.Flashlight:
                    inventory.GiveFlashlight();
                    break;

                case LootType.Key:
                    inventory.AddKey(1);
                    break;

                case LootType.Nothing:
                    break;
            }
        }

        Debug.Log("Body searched and found: " + foundLoot);
        Debug.Log("Searches remaining: " + searchesRemaining);
    }
}
