using UnityEngine;

public class FootstepSounds : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip footstepClip;

    [Header("Pitch Variation")]
    [SerializeField] float minPitch = 0.8f;
    [SerializeField] float maxPitch = 1.2f;

    [Header("Step Intervals")]
    [SerializeField] float walkInterval = 0.5f;
    [SerializeField] float sprintInterval = 0.3f;
    [SerializeField] float crouchInterval = 0.7f;

    [Header("Volume")]
    [SerializeField] float walkVolume = 0.6f;
    [SerializeField] float sprintVolume = 1f;
    [SerializeField] float crouchVolume = 0.25f;

    [Header("References")]
    [SerializeField] CharacterController controller;
    [SerializeField] Animator animator;

    float stepTimer;

    void Update()
    {
        
        bool isMoving = animator != null && (animator.GetFloat("Speed") > 0.1f || Mathf.Abs(animator.GetFloat("Horizontal")) > 0.1f);
        bool isSprinting = animator != null && animator.GetBool("Sprint");
        bool isCrouching = animator != null && animator.GetBool("Crouch");

        if (!isMoving)
        {
            stepTimer = 0f;
            return;
        }

        float interval = isSprinting ? sprintInterval
                       : isCrouching ? crouchInterval
                       : walkInterval;

        float volume = isSprinting ? sprintVolume
                     : isCrouching ? crouchVolume
                     : walkVolume;

        stepTimer += Time.deltaTime;

        if (stepTimer >= interval)
        {
            stepTimer = 0f;
            PlayFootstep(volume);
        }
    }

    void PlayFootstep(float volume)
    {
        if (footstepClip == null || audioSource == null) return;
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(footstepClip, volume);
    }
}