/*
    PURPOSE:
    Allows enemies to hear noise events from the NoiseSystem.

    This script listens for noises created by objects such as:
    - Thrown pebbles
    - Radios
    - Alarms
    - Broken glass
    - Doors
    - Other NoiseMaker objects

    PSEUDOCODE:
    On Enable:
        Subscribe to the global noise event.

    On Disable:
        Unsubscribe from the global noise event.

    When noise is created:
        Check distance from enemy to noise.
        Apply hearing multiplier.
        Check if the noise is within hearing range.
        Check if the noise is strong enough.
        If valid, tell listeners that this enemy heard the noise.

    NOTES:
    - This script does not decide how the enemy reacts.
    - It only reports that a noise was heard.
    - EnemyRouteBrain decides whether to investigate, ignore, or chase.
*/

using UnityEngine;

public class NoiseSensor : MonoBehaviour
{
    [Header("Hearing Settings")]
    [SerializeField] private float hearingMultiplier = 1f;
    [SerializeField] private float minimumAttractionStrength = 0f;

    public System.Action<NoiseData> OnNoiseHeard;

    private void OnEnable()
    {
        NoiseSystem.OnNoiseCreated += HandleNoiseCreated;
    }

    private void OnDisable()
    {
        NoiseSystem.OnNoiseCreated -= HandleNoiseCreated;
    }

    private void HandleNoiseCreated(NoiseData noiseData)
    {
        float adjustedRadius = noiseData.radius * hearingMultiplier;
        float distanceToNoise = Vector3.Distance(transform.position, noiseData.position);

        if (distanceToNoise > adjustedRadius)
        {
            return;
        }

        if (noiseData.attractionStrength < minimumAttractionStrength)
        {
            return;
        }

        OnNoiseHeard?.Invoke(noiseData);
    }
}