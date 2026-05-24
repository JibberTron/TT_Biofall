using UnityEngine;

public class PowerReceiver : MonoBehaviour
{
    [Header("Power Objects")]
    [SerializeField] private GameObject objectWhenPowered;
    [SerializeField] private GameObject objectWhenUnpowered;

    private int activePowerSources;
    private bool isPowered;

    public bool IsPowered => isPowered;

    public void AddPowerSource()
    {
        activePowerSources++;
        UpdatePoweredState();
    }

    public void RemovePowerSource()
    {
        activePowerSources = Mathf.Max(0, activePowerSources - 1);
        UpdatePoweredState();
    }

    public void SetPowered(bool powered)
    {
        if (powered)
        {
            AddPowerSource();
        }
        else
        {
            RemovePowerSource();
        }
    }

    private void UpdatePoweredState()
    {
        isPowered = activePowerSources > 0;

        if (objectWhenPowered != null)
        {
            objectWhenPowered.SetActive(isPowered);
        }

        if (objectWhenUnpowered != null)
        {
            objectWhenUnpowered.SetActive(!isPowered);
        }

        Debug.Log($"{gameObject.name} powered: {isPowered} | Sources: {activePowerSources}");
    }
}