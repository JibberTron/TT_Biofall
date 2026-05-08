using UnityEngine;

public class AimTarget : MonoBehaviour
{
    [SerializeField] CameraOrbit cameraOrbit;
    [SerializeField] float aimDistance = 20f;

    void Update()
    {
        if (cameraOrbit == null || !cameraOrbit.isAiming)
            return;

        Transform cam = Camera.main.transform;
        transform.position = cam.position + cam.forward * aimDistance;
    }
}