using System.Collections;
using UnityEngine;

public class PressureValve : MonoBehaviour, IInteractable
{
    [Header("Steam Hazards Controlled By This Valve")]
    [SerializeField] private SteamHazard[] steamHazards;

    [Header("Valve Settings")]
    [SerializeField] private KeyCode holdKey = KeyCode.E;
    [SerializeField] private float holdDuration = 1.5f;

    [Header("Valve Wheel")]
    [SerializeField] private Transform valveWheel;
    [SerializeField] private Vector3 rotationAxis = Vector3.forward;
    [SerializeField] private float totalRotationDegrees = 180f;

    [Header("Valve State Audio")]
    [SerializeField] private AudioSource valveStateAudioSource;
    [SerializeField] private AudioClip valveOnLoopSound;
    [SerializeField] private bool valveIsOn;
    [SerializeField] private AudioClip valveTurningSound;

    private bool isTurning;

    public void Interact()
    {
        if (!isTurning)
        {
            StartCoroutine(HoldValveRoutine());
        }
    }

    private IEnumerator HoldValveRoutine()
    {
        isTurning = true;

        if (valveStateAudioSource != null && valveTurningSound != null)
        {
            valveStateAudioSource.clip = valveTurningSound;
            valveStateAudioSource.loop = true;
            valveStateAudioSource.Play();
        }

        float timer = 0f;
        Quaternion startRotation = valveWheel != null ? valveWheel.localRotation : Quaternion.identity;
        Quaternion endRotation = startRotation * Quaternion.Euler(rotationAxis * totalRotationDegrees);

        while (timer < holdDuration)
        {
            if (!Input.GetKey(holdKey))
            {
                if (valveStateAudioSource != null)
                {
                    valveStateAudioSource.Stop();
                }

                isTurning = false;
                yield break;
            }

            timer += Time.deltaTime;
            float progress = timer / holdDuration;

            if (valveWheel != null)
            {
                valveWheel.localRotation = Quaternion.Slerp(startRotation, endRotation, progress);
            }

            yield return null;
        }

        ToggleSteamHazards();

        valveIsOn = !valveIsOn;

        if (valveStateAudioSource != null)
        {
            if (valveIsOn)
            {
                valveStateAudioSource.clip = valveOnLoopSound;
                valveStateAudioSource.loop = true;
                valveStateAudioSource.Play();
            }
            else
            {
                valveStateAudioSource.Stop();
            }
        }

        isTurning = false;
    }

    private void ToggleSteamHazards()
    {
        if (steamHazards == null || steamHazards.Length == 0)
        {
            Debug.LogWarning($"{gameObject.name} has no steam hazards assigned.");
            return;
        }

        foreach (SteamHazard hazard in steamHazards)
        {
            if (hazard == null)
            {
                continue;
            }

            hazard.SetActive(!hazard.IsActive());
        }

        Debug.Log($"{gameObject.name} flipped assigned steam hazards.");
    }
}