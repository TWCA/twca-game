using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    
    public delegate void OnTimeChanged();
    [HideInInspector] public OnTimeChanged onTimeChanged;

    [SerializeField] public Material paletteSwapMaterial;
    [SerializeField] private bool isFuture = false;
    [SerializeField] private float futureRainStrength = 0;
    [SerializeField] private float futureLightingTime = 1;
    
    private LinkedList<TimeObject> timeObjects = new LinkedList<TimeObject>();

    private void Awake()
    {
        Instance = this;
    }
    
    void Update()
    {
        paletteSwapMaterial.SetFloat("_LightingTime", GetLightingTime());
    }

    /**
     * Registers a TimeObject component in the current scene.
     * Called automatically by TimeObject.Start().
     */
    public void RegisterTimeObject(TimeObject timeObject)
    {
        timeObjects.AddLast(timeObject);
    }
    
    /**
     * Reminds time objects to update if they activate or not.
     * This must be done because deactivated objects don't run Update() or FixedUpdate().
     */
    private void UpdateTimeObjectsActive()
    {
        foreach (TimeObject timeObject in timeObjects)
        {
            // this is *not* a null check, it checks if the object has been destroyed, I wish I was joking
            if (timeObject == null)
                // remove destroyed object
                timeObjects.Remove(timeObject);
            else
                timeObject.UpdateActive();
        }
    }
    
    /**
     * Toggles if the game is in the future.
     */
    public void ToggleTime()
    {
        SetTime(!isFuture);
    }

    /**
     * Sets if the game is in the future.
     */
    public void SetTime(bool isFuture)
    {
        this.isFuture = isFuture;

        onTimeChanged();
        UpdateTimeObjectsActive();
    }

    /**
     * Checks if the game is in the future.
     */
    public bool IsFuture()
    {
        return isFuture;
    }

    /**
     * Gets the current strength of rain falling
     */
    public float GetRainStrength()
    {
        if (isFuture)
            return futureRainStrength;
        else
            return 0;
    }

    /**
     * Gets the current shader lighting time between 0 and 1
     * 0   => day
     * 0.5 => dawn/dusk
     * 1   => night
     */
    public float GetLightingTime()
    {
        if (isFuture)
            return futureLightingTime;
        else
            return 0;
    }
}