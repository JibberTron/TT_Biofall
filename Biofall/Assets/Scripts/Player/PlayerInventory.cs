using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Limits")]
    [SerializeField] private int maxBatteries = 10;
    [SerializeField] private int maxPebbles = 5;
    [SerializeField] private int maxAmmo = 30;

    [Header("Inventory Counts")]
    [SerializeField] private int batteries;
    [SerializeField] private int pebbles;
    [SerializeField] private int ammo;

    public int Batteries => batteries;
    public int Pebbles => pebbles;
    public int Ammo => ammo;

    public void AddBatteries(int amount)
    {
        batteries = Mathf.Clamp(batteries + amount, 0, maxBatteries);
        Debug.Log($"Batteries: {batteries}/{maxBatteries}");
    }

    public bool TryUseBattery()
    {
        if (batteries <= 0)
        {
            Debug.Log("No batteries available.");
            return false;
        }

        batteries--;
        Debug.Log($"Battery used. Batteries left: {batteries}/{maxBatteries}");
        return true;
    }

    public void AddPebbles(int amount)
    {
        pebbles = Mathf.Clamp(pebbles + amount, 0, maxPebbles);
        Debug.Log($"Pebbles: {pebbles}/{maxPebbles}");
    }

    public bool TryUsePebble()
    {
        if (pebbles <= 0)
        {
            Debug.Log("No pebbles available.");
            return false;
        }

        pebbles--;
        Debug.Log($"Pebble used. Pebbles left: {pebbles}/{maxPebbles}");
        return true;
    }

    public void AddAmmo(int amount)
    {
        ammo = Mathf.Clamp(ammo + amount, 0, maxAmmo);
        Debug.Log($"Ammo: {ammo}/{maxAmmo}");
    }

    public bool TryUseAmmo(int amount = 1)
    {
        if (ammo < amount)
        {
            Debug.Log("Not enough ammo.");
            return false;
        }

        ammo -= amount;
        Debug.Log($"Ammo used. Ammo left: {ammo}/{maxAmmo}");
        return true;
    }
}