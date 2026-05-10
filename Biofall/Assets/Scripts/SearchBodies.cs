using UnityEngine;

public class SearchBodies : MonoBehaviour, IInteractable
{
    public enum LootType
    {
        OralAntibody,
        InjectionAntibody,
        Ammo,
        Flashlight,
        Gun,
        Nothing
    }

    [SerializeField] LootType[] possibleFinds;

    private bool hasBeenSearched = false;

    public void Interact()
    {
        if(hasBeenSearched)
        {
            Debug.Log("This body has already been searched!");
            return;
        }

        hasBeenSearched = true;
        int randomIndex = Random.Range(0, possibleFinds.Length);
        LootType foundLoot = possibleFinds[randomIndex];
        Debug.Log("Found: " +  foundLoot);
    }
}
