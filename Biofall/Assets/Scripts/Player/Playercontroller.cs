using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CharacterController controller;
    [SerializeField] Animator animator;
    [SerializeField] HealthSystem healthSystem;
    [SerializeField] HidingSystem hidingSystem;

    [Header("Movement")]
    [SerializeField] float speed = 5f;
    [SerializeField] float sprintMod = 2f;

    [Header("Crouch")]
    [SerializeField] float crouchSpeed = 2f;
    [SerializeField] float crouchHeight = 1f;
    [SerializeField] float standingHeight = 2f;

    [Header("Gravity")]
    [SerializeField] float gravity = 25f;

    [Header("Combat")]
    [SerializeField] float shootDist = 100f;

    [Header("Aim")]
    [SerializeField] GunManager gunManager;
    [SerializeField] CameraOrbit cameraOrbit;
    [SerializeField] MultiAimConstraint bodyAimConstraint;
    [SerializeField] MultiAimConstraint handAimConstraint;

    [Header("Stats")]
    public int HP = 100;
    public int HPOrig = 100;

    Vector3 moveDir;
    Vector3 playerVel;
    float shootTimer;
    float emptyCooldown;
    bool isCrouching;
    float crouchBlendTime;
    Vector3 originalCenter;
    float originalHeight;

    void Start()
    {
        originalCenter = controller.center;
        originalHeight = controller.height;
    }

    void Update()
    {
        Crouch();
        Movement();
        HandleAim();
    }

    void HandleAim()
    {
        bool canAim = gunManager != null && gunManager.HasGun();
        bool isAiming = canAim && cameraOrbit != null && cameraOrbit.isAiming;

        animator.SetBool("Aiming", isAiming);

        float targetWeight = isAiming ? 1f : 0f;

        if (bodyAimConstraint != null)
            bodyAimConstraint.weight = Mathf.Lerp(bodyAimConstraint.weight, targetWeight, Time.deltaTime * 10f);

        if (handAimConstraint != null)
            handAimConstraint.weight = Mathf.Lerp(handAimConstraint.weight, targetWeight, Time.deltaTime * 10f);

        // Rotate player to face camera direction while aiming
        if (isAiming)
        {
            Transform cam = Camera.main.transform;
            Vector3 camForward = new Vector3(cam.forward.x, 0f, cam.forward.z).normalized;
            transform.forward = Vector3.Slerp(transform.forward, camForward, Time.deltaTime * 15f);
        }
    }

    void Crouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;

            if (isCrouching)
            {
                controller.height = crouchHeight;
                controller.center = new Vector3(0, originalCenter.y - (originalHeight - crouchHeight) / 2f, 0);
                animator.SetBool("Crouch", true);
                crouchBlendTime = 0.4f;
            }
            else
            {
                controller.height = originalHeight;
                controller.center = originalCenter;
                animator.SetBool("Crouch", false);
                animator.speed = 1f;
            }
        }

        if (isCrouching)
        {
            if (crouchBlendTime > 0f)
                crouchBlendTime -= Time.deltaTime;
        }
    }

    void Movement()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.yellow);

        shootTimer += Time.deltaTime;

        if (emptyCooldown > 0f)
            emptyCooldown -= Time.deltaTime;

        if (controller.isGrounded && playerVel.y < 0)
            playerVel.y = -2f;

        float v = Input.GetAxis("Vertical");

        if (healthSystem != null && healthSystem.isHit)
        {
            animator.SetFloat("Speed", 0f);
            playerVel.y -= gravity * Time.deltaTime;
            controller.Move(playerVel * Time.deltaTime);
            return;
        }

        bool hiding = hidingSystem != null && hidingSystem.IsHiding();

        Transform cam = Camera.main.transform;
        Vector3 camForward = new Vector3(cam.forward.x, 0f, cam.forward.z).normalized;

        moveDir = v * camForward;

        float currentSpeed;
        if (hiding)
            currentSpeed = crouchSpeed * 0.5f;
        else if (isCrouching)
            currentSpeed = crouchSpeed;
        else if (Input.GetKey(KeyCode.LeftShift) && !hiding)
            currentSpeed = speed * sprintMod;
        else
            currentSpeed = speed;

        controller.Move(moveDir * currentSpeed * Time.deltaTime);

        if (moveDir != Vector3.zero && !cameraOrbit.isAiming)
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * 10f);

        playerVel.y -= gravity * Time.deltaTime;
        controller.Move(playerVel * Time.deltaTime);

        animator.SetFloat("Speed", Mathf.Abs(v));
        animator.SetBool("Sprint", Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(v) > 0.1f && !isCrouching && !hiding);
    }
}