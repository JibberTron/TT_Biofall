using UnityEngine;

public class HidingSystem : MonoBehaviour
{
    bool isHiding;

    public void ToggleHiding()
    {
        isHiding = !isHiding;
        Debug.Log(isHiding ? "Player is hiding" : "Player stopped hiding");
    }

    public void ForceExitHiding()
    {
        isHiding = false;
        Debug.Log("Player forced out of hiding");
    }

    public bool IsHiding() => isHiding;
    public float GetDetectionModifier() => isHiding ? 0f : 1f;
}