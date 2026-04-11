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

        var main = rainParticleSystem.main;
        main.playOnAwake = false;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        emissionModule = rainParticleSystem.emission;
        StopAndClearRain();

        AnchorRainInWorld();
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
    }

    private void AnchorRainInWorld()
    {
        CameraFollow cameraFollow = GetComponentInParent<CameraFollow>();
        if (cameraFollow == null) return;

        transform.SetParent(null, true);

        Vector2 boundsCenter = (cameraFollow.minBounds + cameraFollow.maxBounds) * 0.5f;
        Vector2 boundsSize = cameraFollow.maxBounds - cameraFollow.minBounds;

        Camera camera = cameraFollow.GetComponent<Camera>();
        if (camera != null && camera.orthographic)
        {
            boundsSize.x += camera.orthographicSize * camera.aspect * 2f;
            boundsSize.y += camera.orthographicSize * 2f;
        }

        transform.position = new Vector3(boundsCenter.x, boundsCenter.y, transform.position.z);

        var shape = rainParticleSystem.shape;
        shape.scale = new Vector3(boundsSize.x, boundsSize.y, shape.scale.z);
    }

    private void StopAndClearRain()
    {
        emissionModule.rateOverTime = 0f;

        if (rainParticleSystem.isPlaying || rainParticleSystem.particleCount > 0)
            rainParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void UpdateRain()
    {
        if (TimeManager.Instance == null)
        {
            StopAndClearRain();
            return;
        }

        float strength = Mathf.Clamp01(TimeManager.Instance.GetRainStrength());

        if (strength > 0f)
        {
            emissionModule.rateOverTime = maxEmissionRate * strength;

            if (!rainParticleSystem.isPlaying)
                rainParticleSystem.Play();
        }
        else
        {
            StopAndClearRain();
        }
    }
}
