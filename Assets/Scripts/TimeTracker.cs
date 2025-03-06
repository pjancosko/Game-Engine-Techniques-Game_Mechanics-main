using UnityEngine;
using TMPro;
using System.Collections;

public class TimeTracker : MonoBehaviour
{
    [SerializeField] private float timeSpeed = 60f; // Adjustable in Unity Editor during runtime
    public TextMeshProUGUI timeDisplay; // Attach a TextMeshProUGUI element in Inspector
    public Light sun; // Attach the directional light (sun) in Inspector
    public Material skyboxMaterial; // Attach the Skybox material in Inspector
    public Light[] sceneLights; // Attach all scene lights to adjust brightness dynamically
    public Renderer[] emissiveObjects; // Objects that may emit light, adjust their emission dynamically

    private float timer = 0f;
    private int seconds = 0;
    private int minutes = 0;
    private int hours = 0;
    private int days = 0;
    private int weeks = 0;

    void Start()
    {
        StartCoroutine(TimeCoroutine());
    }

    IEnumerator TimeCoroutine()
    {
        while (true)
        {
            yield return null; // Run every frame
            AdvanceTime();
        }
    }
    
    void AdvanceTime()
    {
        float deltaTime = Time.deltaTime * timeSpeed;
        timer += deltaTime;

        while (timer >= 1f)
        {
            timer -= 1f;
            seconds++;
            
            if (seconds >= 60)
            {
                seconds = 0;
                minutes++;
                
                if (minutes >= 60)
                {
                    minutes = 0;
                    hours++;
                    
                    if (hours >= 24)
                    {
                        hours = 0;
                        days++;
                        
                        if (days >= 7)
                        {
                            days = 0;
                            weeks++;
                        }
                    }
                }
            }
        }
        
        UpdateTimeDisplay();
        UpdateSunPosition();
        UpdateSkybox();
        UpdateSceneLighting();
        UpdateEmissiveObjects();
    }
    
    void UpdateTimeDisplay()
    {
        if (timeDisplay != null)
        {
            timeDisplay.text = $"Time: {weeks} weeks, {days} days, {hours} hours, {minutes} minutes, {seconds} seconds";
        }
    }
    
    void UpdateSunPosition()
    {
        if (sun != null)
        {
            // Rotate the sun based on the time of day (360 degrees per 24 hours)
            float sunRotation = (hours * 15f) + (minutes * 0.25f) + (seconds * (0.25f / 60f));
            sun.transform.rotation = Quaternion.Euler(new Vector3(sunRotation - 90, 0, 0));

            // Adjust Sun Intensity based on Time (Day/Night Cycle)
            float sunIntensity = Mathf.Clamp01(Mathf.Sin(((hours + minutes / 60f) / 24f) * Mathf.PI));
            sun.intensity = Mathf.Lerp(0.2f, 1.25f, sunIntensity); // Lerp for smoother transitions
        }
    }
    
    void UpdateSkybox()
    {
        if (skyboxMaterial != null)
        {
            // Slow down Skybox rotation by scaling the rotation factor
            float skyboxRotation = ((hours * 15f) + (minutes * 0.25f) + (seconds * (0.25f / 60f))) * 0.3f;
            skyboxMaterial.SetFloat("_Rotation", skyboxRotation);
            
            // Adjust Skybox brightness, increasing by 20% at its brightest point
            float brightnessFactor = Mathf.Clamp01(Mathf.Sin(((hours + minutes / 60f) / 24f) * Mathf.PI));
            brightnessFactor = Mathf.Lerp(0.2f, 1.2f, brightnessFactor); // Increase peak brightness
            Color skyboxColor = Color.Lerp(Color.black, Color.gray * 0.7f, brightnessFactor); 
            skyboxMaterial.SetColor("_Tint", skyboxColor);
        }
    }
    
    void UpdateSceneLighting()
    {
        float lightFactor = Mathf.Clamp01(Mathf.Sin(((hours + minutes / 60f) / 24f) * Mathf.PI));
        float sceneLightIntensity = Mathf.Lerp(0f, 1f, lightFactor); // Darker at night
        
        foreach (Light sceneLight in sceneLights)
        {
            if (sceneLight != null)
            {
                sceneLight.intensity = sceneLightIntensity;
            }
        }
    }
    
    void UpdateEmissiveObjects()
    {
        float emissionFactor = Mathf.Clamp01(Mathf.Sin(((hours + minutes / 60f) / 24f) * Mathf.PI));
        Color emissionColor = Color.Lerp(Color.black, Color.gray * 0.2f, emissionFactor); // Reduce emission at night
        
        foreach (Renderer obj in emissiveObjects)
        {
            if (obj != null && obj.material.HasProperty("_EmissionColor"))
            {
                obj.material.SetColor("_EmissionColor", emissionColor);
                obj.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            }
        }
    }
}
