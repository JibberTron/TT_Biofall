using UnityEngine;

public class SearchBodies : MonoBehaviour, IInteractable
{
    public enum LootType
    {
        OralAntibody,
        Syringe,
        Ammo,
        Flashlight,
        Gun,
        BrokenFlashlight,
        BloodyNotes,
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

        Debug.Log("Body searched and found: " + foundLoot);
        Debug.Log("Searches remaining: " + searchesRemaining);
    }
}
