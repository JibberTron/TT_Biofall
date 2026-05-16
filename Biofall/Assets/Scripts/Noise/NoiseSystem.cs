using System;
using UnityEngine;

public static class NoiseSystem
{
    public static event Action<NoiseData> OnNoiseCreated;

    public static void CreateNoise(NoiseData noiseData)
    {
        OnNoiseCreated?.Invoke(noiseData);
    }
}