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

    [Header("Recoil")]
    [SerializeField] float recoilAmount = 2f;

    [Header("References")]
    [SerializeField] CameraOrbit cameraOrbit;
    [SerializeField] Transform gunBarrel;
    [SerializeField] Animator animator;
    [SerializeField] HidingSystem hidingSystem;
    [SerializeField] InfectionHallucination hallucination;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip shootSound;
    [SerializeField] AudioClip reloadSound;
    [SerializeField] float shootVolume = 1f;
    [SerializeField] float reloadVolume = 0.5f;

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

        if (Input.GetMouseButtonDown(0) && cameraOrbit.isAiming
            && Time.time >= nextFireTime
            && (hidingSystem == null || !hidingSystem.IsHiding())
            && (hallucination == null || !hallucination.IsHallucinating()))
            Shoot();
    }

    void Shoot()
    {
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        animator.SetTrigger("Shoot");
        nextFireTime = Time.time + fireRate;
        currentAmmo--;

        if (muzzleFlash != null)
            muzzleFlash.Play();

        if (audioSource != null && shootSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(shootSound, shootVolume);
        }

        if (cameraOrbit != null)
            cameraOrbit.AddRecoil(recoilAmount);

        NoiseSystem.CreateNoise(new NoiseData(
            transform.position,
            15f,
            0.5f,
            1f,
            gameObject
        ));

        Vector2 wobble = Random.insideUnitCircle * wobbleAmount;
        Vector3 shootDir = Camera.main.transform.forward
                         + Camera.main.transform.right * wobble.x
                         + Camera.main.transform.up * wobble.y;

        Ray ray = new Ray(Camera.main.transform.position, shootDir);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, shootDist))
        {
            Debug.Log("Hit: " + hit.collider.name);
            iDamage dmg = hit.collider.GetComponent<iDamage>();
            if (dmg == null)
                dmg = hit.collider.GetComponentInParent<iDamage>();
            if (dmg != null)
                dmg.TakeDamage(10);
        }
        else { 
            Debug.Log("Hit nothing"); 
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
        animator.SetTrigger("Reload");

        yield return new WaitForSeconds(0.3f);

        if (audioSource != null && reloadSound != null)
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(reloadSound, reloadVolume);
        }

        yield return new WaitForSeconds(reloadTime - 0.3f);

        int needed = maxAmmo - currentAmmo;
        int pulled = Mathf.Min(needed, totalAmmo);
        currentAmmo += pulled;
        totalAmmo -= pulled;
        isReloading = false;
    }

    public void AddAmmo(int amount) => totalAmmo += amount;
    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => maxAmmo;
    public int GetTotalAmmo() => totalAmmo;
    public bool IsReloading() => isReloading;
}