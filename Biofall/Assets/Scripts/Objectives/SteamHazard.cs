using UnityEngine;

public class SteamHazard : MonoBehaviour
{
    [Header("Steam Hazard")]
    [SerializeField] private GameObject steamVisual;
    [SerializeField] private Collider steamBlocker;

    [SerializeField] private bool startsActive = true;

    private bool isActive;

    private void Start()
    {
        SetActive(startsActive);
    }

    public void SetActive(bool active)
    {
        isActive = active;

        if (steamVisual != null)
        {
            steamVisual.SetActive(isActive);
        }

        if (steamBlocker != null)
        {
            steamBlocker.enabled = isActive;
        }
    }

    public bool IsActive()
    {
        return isActive;
    }
}