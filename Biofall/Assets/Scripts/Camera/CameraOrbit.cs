using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float mouseSensitivity = 3f;
    [SerializeField] float minVerticalAngle = -20f;
    [SerializeField] float maxVerticalAngle = 60f;

    [Header("Normal Camera")]
    [SerializeField] float normalDistance = 3f;
    [SerializeField] float normalOffsetX = 0.7f;
    [SerializeField] float normalOffsetY = 1.5f;

    [Header("Aim Camera")]
    [SerializeField] float aimDistance = 1.5f;
    [SerializeField] float aimOffsetX = 0.5f;
    [SerializeField] float aimOffsetY = 1.6f;
    [SerializeField] float aimSmoothing = 10f;

    [Header("Pan On Rotate")]
    [SerializeField] float panAmount = 0.5f;
    [SerializeField] float panSmoothing = 5f;

    [Header("Sprint Pullback")]
    [SerializeField] float sprintDistance = 5f;
    [SerializeField] float distanceSmoothing = 5f;

    float yaw;
    float pitch;
    float currentPan;
    float currentDistance;
    float currentOffsetX;
    float currentOffsetY;

    [HideInInspector] public bool isAiming;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = target != null ? target.eulerAngles.y : 0f;
        pitch = 10f;
        currentDistance = normalDistance;
        currentOffsetX = normalOffsetX;
        currentOffsetY = normalOffsetY;
    }

    void LateUpdate()
    {
        if (target == null) return;

        isAiming = Input.GetMouseButton(1);

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * mouseSensitivity;
        pitch -= mouseY * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minVerticalAngle, maxVerticalAngle);

        float targetDistance;
        float targetOffsetX;
        float targetOffsetY;

        if (isAiming)
        {
            targetDistance = aimDistance;
            targetOffsetX = aimOffsetX;
            targetOffsetY = aimOffsetY;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            targetDistance = sprintDistance;
            targetOffsetX = normalOffsetX;
            targetOffsetY = normalOffsetY;
            currentPan = 0f;
        }
        else
        {
            float targetPan = mouseX * panAmount;
            currentPan = Mathf.Lerp(currentPan, targetPan, Time.deltaTime * panSmoothing);
            targetDistance = normalDistance;
            targetOffsetX = normalOffsetX + currentPan;
            targetOffsetY = normalOffsetY;
        }

        currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * aimSmoothing);
        currentOffsetX = Mathf.Lerp(currentOffsetX, targetOffsetX, Time.deltaTime * aimSmoothing);
        currentOffsetY = Mathf.Lerp(currentOffsetY, targetOffsetY, Time.deltaTime * aimSmoothing);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 shoulderPos = target.position
                               + rotation * Vector3.right * currentOffsetX
                               + Vector3.up * currentOffsetY;

        transform.position = shoulderPos - rotation * Vector3.forward * currentDistance;
        transform.rotation = rotation;
    }
}