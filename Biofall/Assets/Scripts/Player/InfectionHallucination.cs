using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class InfectionHallucination : MonoBehaviour
{
    [Header("Thresholds")]
    [SerializeField] float infectionThreshold = 75f;

    [Header("Episode Settings")]
    [SerializeField] float minDuration = 2f;
    [SerializeField] float maxDuration = 5f;
    [SerializeField] float minCooldown = 5f;
    [SerializeField] float maxCooldown = 15f;

    [Header("Heartbeat Pulse")]
    [SerializeField] float pulseSpeed = 2f;
    [SerializeField] float minVignetteIntensity = 0.3f;
    [SerializeField] float maxVignetteIntensity = 0.8f;

    [Header("Effects")]
    [SerializeField] Color vignetteColor = new Color(0.4f, 0f, 0.5f);
    [SerializeField] float maxSaturation = -80f;
    [SerializeField] float maxBlur = 0.4f;
    [SerializeField] float maxAberration = 0.8f;
    [SerializeField] float maxLensDistortion = -0.3f;
    [SerializeField] float maxFilmGrain = 0.8f;
    [SerializeField] float maxHueShift = 20f;

    [Header("Heartbeat Audio")]
    [SerializeField] AudioSource heartbeatSource;
    [SerializeField] AudioClip heartbeatClip;
    [SerializeField] float heartbeatVolume = 0.4f;
    [SerializeField] float heartbeatFadeSpeed = 2f;

    [Header("References")]
    [SerializeField] Volume volume;
    [SerializeField] InfectionSystem infectionSystem;
    [SerializeField] GunManager gunManager;
    [SerializeField] Animator animator;

    Vignette vignette;
    ColorAdjustments colorAdjustments;
    MotionBlur motionBlur;
    ChromaticAberration chromaticAberration;
    LensDistortion lensDistortion;
    FilmGrain filmGrain;
    AudioReverbFilter reverbFilter;

    bool isHallucinating;
    float cooldownTimer;

    void Start()
    {
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out colorAdjustments);
        volume.profile.TryGet(out motionBlur);
        volume.profile.TryGet(out chromaticAberration);
        volume.profile.TryGet(out lensDistortion);
        volume.profile.TryGet(out filmGrain);

        if (heartbeatSource != null)
        {
            heartbeatSource.clip = heartbeatClip;
            heartbeatSource.loop = true;
            heartbeatSource.volume = 0f;
            heartbeatSource.pitch = 0.85f;

            reverbFilter = heartbeatSource.gameObject.GetComponent<AudioReverbFilter>();
            if (reverbFilter == null)
                reverbFilter = heartbeatSource.gameObject.AddComponent<AudioReverbFilter>();

            reverbFilter.reverbPreset = AudioReverbPreset.Cave;
            reverbFilter.enabled = false;
        }

        ResetEffects();
        cooldownTimer = Random.Range(minCooldown, maxCooldown);
    }

    void Update()
    {
        if (infectionSystem == null) return;

        float infection = infectionSystem.currentInfection;

        if (infection < infectionThreshold)
        {
            if (isHallucinating) StopAllCoroutines();
            ResetEffects();
            StopHeartbeat();
            isHallucinating = false;
            return;
        }

        if (!isHallucinating)
        {
            cooldownTimer -= Time.deltaTime;

            float infectionPercent = (infection - infectionThreshold) / (infectionSystem.maxInfection - infectionThreshold);
            float cooldownMod = Mathf.Lerp(1f, 0.3f, infectionPercent);

            if (cooldownTimer <= 0f)
            {
                float duration = Random.Range(minDuration, maxDuration);
                StartCoroutine(HallucinationEpisode(duration));
                cooldownTimer = Random.Range(minCooldown, maxCooldown) * cooldownMod;
            }
        }
    }

    IEnumerator HallucinationEpisode(float duration)
    {
        isHallucinating = true;

        if (gunManager != null) gunManager.enabled = false;
        if (animator != null)
        {
            animator.SetBool("Hallucinating", true);
            animator.SetLayerWeight(animator.GetLayerIndex("Hallucination"), 1f);
        }

        if (vignette != null) vignette.color.value = vignetteColor;

        StartHeartbeat();

        // Fade in
        float fadeIn = 0f;
        while (fadeIn < 1f)
        {
            fadeIn += Time.deltaTime * 1.5f;
            if (vignette != null)
                vignette.intensity.value = Mathf.Lerp(0f, minVignetteIntensity, fadeIn);
            if (colorAdjustments != null)
                colorAdjustments.saturation.value = Mathf.Lerp(0f, maxSaturation * 0.5f, fadeIn);
            if (motionBlur != null)
                motionBlur.intensity.value = Mathf.Lerp(0f, maxBlur * 0.5f, fadeIn);
            if (chromaticAberration != null)
                chromaticAberration.intensity.value = Mathf.Lerp(0f, maxAberration * 0.5f, fadeIn);
            if (lensDistortion != null)
                lensDistortion.intensity.value = Mathf.Lerp(0f, maxLensDistortion * 0.5f, fadeIn);
            if (filmGrain != null)
                filmGrain.intensity.value = Mathf.Lerp(0f, maxFilmGrain * 0.5f, fadeIn);

            if (heartbeatSource != null)
                heartbeatSource.volume = Mathf.Lerp(0f, heartbeatVolume, fadeIn);

            yield return null;
        }

        float elapsed = 0f;
        float pulseTime = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            pulseTime += Time.deltaTime * pulseSpeed;

            float pulse = Mathf.Abs(Mathf.Sin(pulseTime * Mathf.PI));

            if (vignette != null)
                vignette.intensity.value = Mathf.Lerp(minVignetteIntensity, maxVignetteIntensity, pulse);
            if (colorAdjustments != null)
            {
                colorAdjustments.saturation.value = Mathf.Lerp(0f, maxSaturation, pulse);
                colorAdjustments.hueShift.value = Mathf.Lerp(0f, maxHueShift, pulse);
            }
            if (motionBlur != null)
                motionBlur.intensity.value = Mathf.Lerp(0f, maxBlur, pulse);
            if (chromaticAberration != null)
                chromaticAberration.intensity.value = Mathf.Lerp(0f, maxAberration, pulse);
            if (lensDistortion != null)
                lensDistortion.intensity.value = Mathf.Lerp(0f, maxLensDistortion, pulse);
            if (filmGrain != null)
                filmGrain.intensity.value = Mathf.Lerp(0f, maxFilmGrain, pulse);

            yield return null;
        }

        // Fade out
        float fadeTime = 0f;
        while (fadeTime < 1f)
        {
            fadeTime += Time.deltaTime * 1f;
            if (vignette != null)
                vignette.intensity.value = Mathf.Lerp(maxVignetteIntensity, 0f, fadeTime);
            if (colorAdjustments != null)
            {
                colorAdjustments.saturation.value = Mathf.Lerp(maxSaturation, 0f, fadeTime);
                colorAdjustments.hueShift.value = Mathf.Lerp(maxHueShift, 0f, fadeTime);
            }
            if (motionBlur != null)
                motionBlur.intensity.value = Mathf.Lerp(maxBlur, 0f, fadeTime);
            if (chromaticAberration != null)
                chromaticAberration.intensity.value = Mathf.Lerp(maxAberration, 0f, fadeTime);
            if (lensDistortion != null)
                lensDistortion.intensity.value = Mathf.Lerp(maxLensDistortion, 0f, fadeTime);
            if (filmGrain != null)
                filmGrain.intensity.value = Mathf.Lerp(maxFilmGrain, 0f, fadeTime);

            if (heartbeatSource != null)
                heartbeatSource.volume = Mathf.Lerp(heartbeatVolume, 0f, fadeTime);

            yield return null;
        }

        ResetEffects();
        StopHeartbeat();

        if (gunManager != null) gunManager.enabled = true;
        if (animator != null)
        {
            animator.SetBool("Hallucinating", false);
            animator.SetLayerWeight(animator.GetLayerIndex("Hallucination"), 0f);
        }

        isHallucinating = false;
    }

    void StartHeartbeat()
    {
        if (heartbeatSource == null || heartbeatClip == null) return;
        heartbeatSource.volume = 0f;
        heartbeatSource.Play();
        if (reverbFilter != null) reverbFilter.enabled = true;
    }

    void StopHeartbeat()
    {
        if (heartbeatSource == null) return;
        heartbeatSource.Stop();
        heartbeatSource.volume = 0f;
        if (reverbFilter != null) reverbFilter.enabled = false;
    }

    void ResetEffects()
    {
        if (vignette != null) vignette.intensity.value = 0f;
        if (colorAdjustments != null)
        {
            colorAdjustments.saturation.value = 0f;
            colorAdjustments.hueShift.value = 0f;
        }
        if (motionBlur != null) motionBlur.intensity.value = 0f;
        if (chromaticAberration != null) chromaticAberration.intensity.value = 0f;
        if (lensDistortion != null) lensDistortion.intensity.value = 0f;
        if (filmGrain != null) filmGrain.intensity.value = 0f;
    }

    public bool IsHallucinating() => isHallucinating;

    public void ForceStopHallucination()
    {
        if (!isHallucinating) return;

        StopAllCoroutines();
        ResetEffects();
        StopHeartbeat();

        if (gunManager != null) gunManager.enabled = true;
        if (animator != null)
        {
            animator.SetBool("Hallucinating", false);
            animator.SetLayerWeight(animator.GetLayerIndex("Hallucination"), 0f);
        }

        isHallucinating = false;
        cooldownTimer = Random.Range(minCooldown, maxCooldown);
    }
}