using UnityEngine;

public class RainController : MonoBehaviour
{
    [SerializeField] private ParticleSystem rainParticleSystem;
    [SerializeField] private float maxEmissionRate = 400f;

    private ParticleSystem.EmissionModule emissionModule;

    private void Awake()
    {
        if (rainParticleSystem == null)
            rainParticleSystem = GetComponent<ParticleSystem>();

        emissionModule = rainParticleSystem.emission;
    }

    private void OnEnable()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.onTimeChanged += OnTimeChanged;
    }

    private void OnDisable()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.onTimeChanged -= OnTimeChanged;
    }

    private void Start()
    {
        UpdateRain();
    }

    private void Update()
    {
        UpdateRain();
    }

    private void OnTimeChanged()
    {
        UpdateRain();

        if (TimeManager.Instance != null &&
            Mathf.Clamp01(TimeManager.Instance.GetRainStrength()) <= 0f)
        {
            rainParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    private void UpdateRain()
    {
        if (TimeManager.Instance == null) return;

        float strength = Mathf.Clamp01(TimeManager.Instance.GetRainStrength());
        emissionModule.rateOverTime = maxEmissionRate * strength;

        if (strength > 0f)
        {
            if (!rainParticleSystem.isPlaying)
                rainParticleSystem.Play();
        }
        else
        {
            if (rainParticleSystem.isPlaying)
                rainParticleSystem.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}