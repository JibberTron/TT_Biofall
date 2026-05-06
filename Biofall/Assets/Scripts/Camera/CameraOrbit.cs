using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float mouseSensitivity = 3f;
    [SerializeField] float distance = 3f;
    [SerializeField] float minVerticalAngle = -20f;
    [SerializeField] float maxVerticalAngle = 60f;
    [SerializeField] float shoulderOffsetX = 0.7f;
    [SerializeField] float shoulderOffsetY = 1.5f;

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

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = target != null ? target.eulerAngles.y : 0f;
        pitch = 10f;
        currentDistance = distance;
    }

    void LateUpdate()
    {
        if (target == null) return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * mouseSensitivity;
        pitch -= mouseY * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minVerticalAngle, maxVerticalAngle);

        
        float targetPan = mouseX * panAmount;
        currentPan = Mathf.Lerp(currentPan, targetPan, Time.deltaTime * panSmoothing);

        
        bool sprinting = Input.GetKey(KeyCode.LeftShift);
        float targetDistance = sprinting ? sprintDistance : distance;
        currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * distanceSmoothing);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 shoulderPos = target.position
                               + rotation * Vector3.right * (shoulderOffsetX + currentPan)
                               + Vector3.up * shoulderOffsetY;

        transform.position = shoulderPos - rotation * Vector3.forward * currentDistance;
        transform.rotation = rotation;
    }
}