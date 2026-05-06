using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CharacterController controller;
    [SerializeField] Animator animator;

    [Header("Movement")]
    [SerializeField] float speed = 5f;
    [SerializeField] float sprintMod = 2f;

    [Header("Crouch")]
    [SerializeField] float crouchSpeed = 2f;
    [SerializeField] float crouchHeight = 1f;


    [Header("Gravity")]
    [SerializeField] float gravity = 25f;

    

    Vector3 moveDir;
    Vector3 playerVel;
    bool isCrouching;
    Vector3 originalCenter;
    float originalHeight;
    float crouchBlendTime;

    public bool IsCrouching =>isCrouching;

    void Start()
    {
        originalCenter = controller.center;
        originalHeight = controller.height;
    }

    void Update()
    {
        Crouch();
        Movement();
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


        float v = Input.GetAxis("Vertical");

        Transform cam = Camera.main.transform;
        Vector3 camForward = new Vector3(cam.forward.x, 0f, cam.forward.z).normalized;

        moveDir = v * camForward;

        float currentSpeed;
        if (isCrouching)
            currentSpeed = crouchSpeed;
        else if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed = speed * sprintMod;
        else
            currentSpeed = speed;

        controller.Move(moveDir * currentSpeed * Time.deltaTime);

        if (moveDir != Vector3.zero)
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * 10f);

        playerVel.y -= gravity * Time.deltaTime;
        controller.Move(playerVel * Time.deltaTime);

        animator.SetFloat("Speed", Mathf.Abs(v));
        animator.SetBool("Sprint", Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(v) > 0.1f && !isCrouching);
    }
}