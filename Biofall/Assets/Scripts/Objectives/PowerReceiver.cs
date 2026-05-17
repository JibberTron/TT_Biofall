using UnityEngine;

public class PowerReceiver : MonoBehaviour
{
    [Header("Power Objects")]
    [SerializeField] private GameObject objectWhenPowered;
    [SerializeField] private GameObject objectWhenUnpowered;

    private bool isPowered;

    public void SetPowered(bool powered)
    {
        isPowered = powered;

        if (objectWhenPowered != null)
        {
            objectWhenPowered.SetActive(isPowered);
        }

        if (objectWhenUnpowered != null)
        {
            objectWhenUnpowered.SetActive(!isPowered);
        }

        Debug.Log($"{gameObject.name} powered: {isPowered}");
    }
}