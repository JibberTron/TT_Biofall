using UnityEngine;

public class AimTarget : MonoBehaviour
{
    [SerializeField] CameraOrbit cameraOrbit;
    [SerializeField] float aimDistance = 20f;

    [Header("Spine Pitch")]
    [SerializeField] Transform spine1;
    [SerializeField] Transform spine2;
    [SerializeField] float minPitch = -30f;
    [SerializeField] float maxPitch = 40f;
    [SerializeField] float pitchSmoothing = 8f;

    float currentPitch;
    float targetPitch;

    void Update()
    {
        if (cameraOrbit == null || !cameraOrbit.isAiming)
            return;

        Transform cam = Camera.main.transform;
        transform.position = cam.position + cam.forward * aimDistance;
    }

    void LateUpdate()
    {
        if (spine1 == null || spine2 == null) return;

        bool isAiming = cameraOrbit != null && cameraOrbit.isAiming;

        if (isAiming)
        {
            float camPitch = Camera.main.transform.eulerAngles.x;
            if (camPitch > 180f) camPitch -= 360f;
            targetPitch = Mathf.Clamp(camPitch, minPitch, maxPitch);
        }
        else
        {
            targetPitch = 0f;
        }

        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * pitchSmoothing);

        spine1.localRotation *= Quaternion.Euler(currentPitch * 0.5f, 0f, 0f);
        spine2.localRotation *= Quaternion.Euler(currentPitch * 0.5f, 0f, 0f);
    }
}