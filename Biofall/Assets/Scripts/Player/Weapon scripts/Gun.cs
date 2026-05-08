using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] int maxAmmo = 6;
    [SerializeField] int totalAmmo = 24;
    [SerializeField] float reloadTime = 2f;
    [SerializeField] float fireRate = 0.3f;
    [SerializeField] float shootDist = 100f;

    [Header("Wobble")]
    [SerializeField] float wobbleAmount = 0.02f;

    [Header("References")]
    [SerializeField] CameraOrbit cameraOrbit;
    [SerializeField] Transform gunBarrel;

    int currentAmmo;
    bool isReloading;
    float nextFireTime;

    void Start()
    {
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        if (isReloading) return;

        if (Input.GetKeyDown(KeyCode.R) || currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetMouseButtonDown(0) && cameraOrbit.isAiming && Time.time >= nextFireTime)
            Shoot();
    }

    void Shoot()
    {
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        nextFireTime = Time.time + fireRate;
        currentAmmo--;

       

        
        Vector2 wobble = Random.insideUnitCircle * wobbleAmount;
        Vector3 shootDir = Camera.main.transform.forward
                         + Camera.main.transform.right * wobble.x
                         + Camera.main.transform.up * wobble.y;

        Ray ray = new Ray(Camera.main.transform.position, shootDir);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, shootDist))
        {
            

            iDamage dmg = hit.collider.GetComponent<iDamage>();
            if (dmg == null)
                dmg = hit.collider.GetComponentInParent<iDamage>();
            if (dmg != null)
                dmg.TakeDamage(10);
        }
    }

    System.Collections.IEnumerator Reload()
    {
        if (totalAmmo <= 0)
        {
           
            isReloading = false;
            yield break;
        }

        isReloading = true;
       
        yield return new WaitForSeconds(reloadTime);

        int needed = maxAmmo - currentAmmo;
        int pulled = Mathf.Min(needed, totalAmmo);
        currentAmmo += pulled;
        totalAmmo -= pulled;

        isReloading = false;
       
    }

    public void AddAmmo(int amount)
    {
        totalAmmo += amount;
        
    }

    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => maxAmmo;
    public int GetTotalAmmo() => totalAmmo;
    public bool IsReloading() => isReloading;
}