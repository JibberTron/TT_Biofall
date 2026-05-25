using UnityEngine;

public class SteamHazard : MonoBehaviour
{
    [Header("Steam Hazard")]
    [SerializeField] private GameObject steamVisual;
    [SerializeField] private Collider steamBlocker;
    [SerializeField] private ParticleSystem steamParticles;

    [Header("Audio")]
    [SerializeField] private AudioSource steamAudioSource;
    [SerializeField] private AudioClip steamLoopSound;

    [SerializeField] private bool startsActive = true;

    private bool isActive;

    private void Start()
    {
        SetActive(startsActive);
    }

    public void SetActive(bool active)
    {
        isActive = active;

        if (steamVisual != null)
        {
            steamVisual.SetActive(isActive);
        }

        if (steamParticles != null)
        {
            if (isActive)
            {
                steamParticles.Play();
            }
            else
            {
                steamParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }

        if (steamAudioSource != null)
        {
            if (isActive)
            {
                if (steamLoopSound != null)
                {
                    steamAudioSource.clip = steamLoopSound;
                }

                steamAudioSource.loop = true;

                if (!steamAudioSource.isPlaying)
                {
                    steamAudioSource.Play();
                }
            }
            else
            {
                steamAudioSource.Stop();
            }
        }

        if (steamBlocker != null)
        {
            steamBlocker.enabled = isActive;
        }
    }

    public bool IsActive()
    {
        return isActive;
    }
}