using UnityEngine;

public class HidingSpot : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform hidePoint;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private Camera hidingCamera;

    public void Interact()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null) return;

        HidingSystem hidingSystem = player.GetComponent<HidingSystem>();

        if (hidingSystem == null) return;

        if (hidingSystem.IsHiding())
        {
            hidingSystem.ExitHidingSpot();
        }
        else
        {
            hidingSystem.EnterHidingSpot(hidePoint, exitPoint, hidingCamera);
        }
    }
}