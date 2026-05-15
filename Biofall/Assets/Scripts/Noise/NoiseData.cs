using UnityEngine;

public struct NoiseData
{
    public Vector3 position;
    public float radius;
    public float duration;
    public float attractionStrength;
    public GameObject sourceObject;

    public NoiseData(Vector3 position, float radius, float duration, float attractionStrength, GameObject sourceObject)
    {
        this.position = position;
        this.radius = radius;
        this.duration = duration;
        this.attractionStrength = attractionStrength;
        this.sourceObject = sourceObject;
    }
}