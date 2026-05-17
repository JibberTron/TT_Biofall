using UnityEngine;

public class PressureValve : MonoBehaviour, IInteractable
{
    [Header("Steam Hazards Controlled By This Valve")]
    [SerializeField] private SteamHazard[] steamHazards;

    [Header("Valve Settings")]
    [SerializeField] private bool turnAllOnIfAllOff = true;

    public void Interact()
    {
        ToggleSteamHazards();
    }

    private void ToggleSteamHazards()
    {
        if (steamHazards == null || steamHazards.Length == 0)
        {
            Debug.LogWarning($"{gameObject.name} has no steam hazards assigned.");
            return;
        }

        bool anyActive = false;

        foreach (SteamHazard hazard in steamHazards)
        {
            if (hazard != null && hazard.IsActive())
            {
                anyActive = true;
                break;
            }
        }

        bool newState = !anyActive;

        if (turnAllOnIfAllOff)
        {
            foreach (SteamHazard hazard in steamHazards)
            {
                if (hazard != null)
                {
                    hazard.SetActive(newState);
                }
            }
        }
        else
        {
            foreach (SteamHazard hazard in steamHazards)
            {
                if (hazard != null)
                {
                    hazard.SetActive(!hazard.IsActive());
                }
            }
        }

        Debug.Log($"{gameObject.name} toggled assigned steam hazards.");
    }
}