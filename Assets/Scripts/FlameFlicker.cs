using UnityEngine;

public class FlickerEmission : MonoBehaviour
{
    public Renderer flameRenderer;
    public Color baseEmission = new Color(1f, 0.5f, 0.1f);  // warm orange
    public float intensityMin = 0.5f;
    public float intensityMax = 1.5f;
    public float flickerSpeed = 2f;

    void Update()
    {
        float intensity = Mathf.Lerp(intensityMin, intensityMax, Mathf.PerlinNoise(Time.time * flickerSpeed, 0f));
        flameRenderer.material.SetColor("_EmissionColor", baseEmission * intensity);
    }
}
