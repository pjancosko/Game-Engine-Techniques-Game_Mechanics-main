using UnityEngine;
using System.Collections;

public enum WeatherType
{
    Clear,
    Rain,
    Snow
}

public class WeatherManager : MonoBehaviour
{
    [Header("Particle Systems")]
    public ParticleSystem snowParticleSystem;
    public ParticleSystem rainParticleSystem;

    [Header("Weather Settings")]
    public WeatherType currentWeather = WeatherType.Clear;
    [Tooltip("Time in seconds between weather updates.")]
    public float weatherUpdateInterval = 10f;

    void Start()
    {
        // Initialize the weather when the scene starts.
        UpdateWeather();
        // Start the coroutine to update weather automatically.
        StartCoroutine(WeatherCycle());
    }

    /// <summary>
    /// Coroutine that periodically updates the weather condition.
    /// </summary>
    IEnumerator WeatherCycle()
    {
        while (true)
        {
            // Wait for the specified interval before changing the weather.
            yield return new WaitForSeconds(weatherUpdateInterval);

            // Randomly select a new weather condition.
            WeatherType newWeather = (WeatherType)Random.Range(0, System.Enum.GetValues(typeof(WeatherType)).Length);
            SetWeather(newWeather);
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
        // Turn off both weather effects by default.
        if (snowParticleSystem != null)
        {
            snowParticleSystem.gameObject.SetActive(false);
            snowParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
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
            case WeatherType.Snow:
                if (snowParticleSystem != null)
                {
                    snowParticleSystem.gameObject.SetActive(true);
                    snowParticleSystem.Play();
                }
                break;
        }
    }
}
