using UnityEngine;
using System.Collections;

public enum WeatherType
{
    Clear,
    Rain
}

public class WeatherManager : MonoBehaviour
{
    [Header("Particle Systems")]
    public ParticleSystem rainParticleSystem;

    [Header("Weather Settings")]
    public WeatherType currentWeather = WeatherType.Clear;

    private Terrain terrain; // Reference to the terrain
    private DayNightCycle dayNightCycle; // Reference to the DayNightCycle script

    void Start()
    {
        // Start the coroutine to wait for the terrain to be generated
        StartCoroutine(WaitForTerrainAndInitializeWeather());

        // Find the DayNightCycle script in the scene
        dayNightCycle = FindObjectOfType<DayNightCycle>();
        if (dayNightCycle == null)
        {
            Debug.LogError("WeatherManager: No DayNightCycle script found in the scene.");
        }
    }

    void Update()
    {
        // Update the WeatherManager's position to match the terrain's position
        if (terrain != null)
        {
            CenterOnTerrain();
        }

        // Check if it is night time (18:00 to 6:00)
        if (dayNightCycle != null && (dayNightCycle.currentTime >= 18f || dayNightCycle.currentTime < 6f))
        {
            // Set weather to rain during the night
            if (currentWeather != WeatherType.Rain)
            {
                SetWeather(WeatherType.Rain);
            }
        }
        else
        {
            // Set weather to clear during the day
            if (currentWeather != WeatherType.Clear)
            {
                SetWeather(WeatherType.Clear);
            }
        }
    }

    IEnumerator WaitForTerrainAndInitializeWeather()
    {
        // Wait until the terrain is generated
        while (Terrain.activeTerrain == null)
        {
            yield return null;
        }

        // Get the active terrain
        terrain = Terrain.activeTerrain;

        // Check if the terrain is found
        if (terrain != null)
        {
            // Center the WeatherManager on the terrain
            CenterOnTerrain();

            // Update the particle systems to match the terrain size
            UpdateParticleSystemShape();

            // Initialize the weather when the terrain is found
            UpdateWeather();
        }
        else
        {
            Debug.LogError("No active terrain found in the scene.");
        }
    }

    /// <summary>
    /// Centers the WeatherManager on the terrain.
    /// </summary>
    void CenterOnTerrain()
    {
        Vector3 terrainPosition = terrain.transform.position;
        Vector3 terrainSize = terrain.terrainData.size;

        // Calculate the center of the terrain
        float centerX = terrainPosition.x + terrainSize.x * 0.5f;
        float centerZ = terrainPosition.z + terrainSize.z * 0.5f;
        float centerY = terrain.SampleHeight(new Vector3(centerX, 0, centerZ)) + terrainPosition.y;

        // Set the WeatherManager's position to the center of the terrain
        transform.position = new Vector3(centerX, centerY, centerZ);
    }

    /// <summary>
    /// Updates the shape module of the particle systems so that the emission area matches the terrain size.
    /// </summary>
    void UpdateParticleSystemShape()
    {
        if (terrain == null) return;

        Vector3 terrainSize = terrain.terrainData.size;

        // Update rain particle system shape
        if (rainParticleSystem != null)
        {
            var rainShape = rainParticleSystem.shape;
            // Set the scale to match the terrain's x (width) and z (length). The y value is kept as is.
            rainShape.scale = new Vector3(terrainSize.x, rainShape.scale.y, terrainSize.z);
        }
    }

    /// <summary>
    /// Sets the current weather and updates the particle systems.
    /// </summary>
    public void SetWeather(WeatherType newWeather)
    {
        currentWeather = newWeather;
        UpdateWeather();
    }

    /// <summary>
    /// Activates or deactivates particle systems based on the current weather.
    /// </summary>
    void UpdateWeather()
    {
        // Turn off the rain effect by default.
        if (rainParticleSystem != null)
        {
            rainParticleSystem.gameObject.SetActive(false);
            rainParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        // Activate the appropriate weather effect.
        switch (currentWeather)
        {
            case WeatherType.Clear:
                // No particle effect for clear weather.
                break;
            case WeatherType.Rain:
                if (rainParticleSystem != null)
                {
                    rainParticleSystem.gameObject.SetActive(true);
                    rainParticleSystem.Play();
                }
                break;
        }
    }
}