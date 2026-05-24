using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    [Header("Light")]
    [SerializeField] private Light pointLight;

    [Header("Mesh Renderer")]
    [SerializeField] private Renderer emissiveRenderer;

    [Header("Flicker Settings")]
    [SerializeField] private float minIntensity = 0.5f;
    [SerializeField] private float maxIntensity = 2f;
    [SerializeField] private float flickerSpeed = 0.05f;

    [Header("Emission")]
    [SerializeField] private Color emissionColor = Color.white;
    [SerializeField] private float emissionMultiplier = 2f;

    private Material materialInstance;

    private void Start()
    {
        if (emissiveRenderer != null)
        {
            materialInstance = emissiveRenderer.material;
        }

        InvokeRepeating(nameof(Flicker), 0f, flickerSpeed);
    }

    private void Flicker()
    {
        float intensity = Random.Range(minIntensity, maxIntensity);

        if (pointLight != null)
        {
            pointLight.intensity = intensity;
        }

        if (materialInstance != null)
        {
            Color finalEmission = emissionColor * intensity * emissionMultiplier;

            materialInstance.SetColor("_EmissionColor", finalEmission);
        }
    }
}