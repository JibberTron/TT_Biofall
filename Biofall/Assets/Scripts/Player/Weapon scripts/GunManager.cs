using UnityEngine;

public class GunManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject gunModel;
    [SerializeField] Gun gun;
    [SerializeField] CameraOrbit cameraOrbit;
    [SerializeField] Animator animator;

    [Header("State")]
    public bool hasGun = false;

    void Start()
    {
        SetGunState(hasGun);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            PickUpGun();
    }

    public void PickUpGun()
    {
        hasGun = true;
        SetGunState(true);
    }

    public void DropGun()
    {
        hasGun = false;
        SetGunState(false);
    }

    void SetGunState(bool state)
    {
        if (gunModel != null)
            gunModel.SetActive(state);

        if (gun != null)
            gun.enabled = state;

        if (!state)
        {
            if (cameraOrbit != null)
                cameraOrbit.isAiming = false;

            if (animator != null)
                animator.SetBool("Aiming", false);
        }
    }

    public bool HasGun() => hasGun;
}