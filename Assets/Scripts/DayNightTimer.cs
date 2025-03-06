using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    [Tooltip("Total length of a full day (0 to 24 hours) in seconds.")]
    public float dayLengthInSeconds = 120f;
    [Tooltip("Current time of day (0 - 24). 0 = midnight, 12 = noon.")]
    [Range(0f, 24f)]
    public float currentTime = 6f;
    private float timeMultiplier;
    private float continuousTime;

    [Header("Sun Settings")]
    [Tooltip("Directional Light representing the sun.")]
    public Light sun;
    [Tooltip("Curve controlling the sun's intensity over the day (x: normalized time, y: intensity factor). " +
             "Ensure the values at 0 and 1 match for a smooth loop.")]
    public AnimationCurve sunIntensityCurve = new AnimationCurve(
        new Keyframe(0f, 0.1f),    // Midnight
        new Keyframe(0.25f, 0.2f),   // Dawn
        new Keyframe(0.5f, 1f),      // Noon (peak)
        new Keyframe(0.75f, 0.2f),   // Dusk
        new Keyframe(1f, 0.1f)       // Midnight
    );
    [Tooltip("Maximum intensity of the sun at its peak.")]
    public float maxSunIntensity = 3f;

    [Header("Ambient Light Settings")]
    [Tooltip("Gradient for ambient light color over the day (x: normalized time). " +
             "Make sure the color at 0 and 1 is the same for a smooth loop.")]
    public Gradient ambientColorGradient;

    void Start()
    {
        // Calculate how many in-game hours pass per real-time second.
        timeMultiplier = 24f / dayLengthInSeconds;
        continuousTime = currentTime; // Initialize continuous time

        // Set the curve to loop so that evaluation at 0 and 1 is smooth.
        sunIntensityCurve.preWrapMode = WrapMode.Loop;
        sunIntensityCurve.postWrapMode = WrapMode.Loop;

        if (sun == null)
        {
            Debug.LogError("DayNightCycle: No Sun light assigned. Please assign a directional light in the Inspector.");
        }
    }

    void Update()
    {
        // Increase continuousTime (this never resets)
        continuousTime += timeMultiplier * Time.deltaTime;
        // Wrap currentTime between 0 and 24 for intensity and ambient calculations
        currentTime = Mathf.Repeat(continuousTime, 24f);

        UpdateSun();
        UpdateAmbientLight();
    }

    void UpdateSun()
    {
        if (sun == null)
            return;

        // Use continuousTime for rotation to ensure smooth, continuous movement.
        float sunAngle = (continuousTime / 24f) * 360f - 90f;
        sun.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);

        // Use the wrapped currentTime (normalized) for intensity.
        float normalizedTime = currentTime / 24f;
        float intensityFactor = sunIntensityCurve.Evaluate(normalizedTime);
        sun.intensity = intensityFactor * maxSunIntensity;
    }

    void UpdateAmbientLight()
    {
        // Update ambient light color based on the gradient.
        float normalizedTime = currentTime / 24f;
        RenderSettings.ambientLight = ambientColorGradient.Evaluate(normalizedTime);
    }
}
