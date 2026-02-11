using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance { get { return _instance; } }

    public AudioSource stepSource, insectSource, rainSource;
    public List<AudioClip> steps;
    public List<AudioClip> stepsWet;
    private Vector3 previousPosition;
    private RectTransform rect;
    private bool future;
    private bool rainStarted = false;
    [SerializeField] private GameObject admin;
    private TimeManager timeManager;
    

    void Awake()
    {
        _instance = this;
        rect = GetComponent<RectTransform>();
        timeManager = admin.GetComponent<TimeManager>();
        playInsects();

        
        future = timeManager.IsFuture();
        //timeManager = TimeManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        future = timeManager.IsFuture();
        if (rect != null && rect.position != previousPosition && !stepSource.isPlaying)
        {
            playSteps();
            Debug.Log("Playing step");
        }
        previousPosition = rect.position;

        if (future == true && !rainStarted )
        {
            playRain();
            rainStarted = true;
        }
        if (future == false && rainStarted)
        {
            stopRain();
            rainStarted = false;
        }
    }

    public void playSteps()
    {
        AudioClip step;
        if (future == true)
        {
            step = stepsWet[Random.Range(0, stepsWet.Count)];
        }
        else
        {
            step = steps[Random.Range(0, steps.Count)];
            
        }
        stepSource.PlayOneShot(step);
    }
    
    public void playInsects()
    {
        insectSource.loop = true;
        //insectSource.PlayOneShot(insects);
        insectSource.Play();
        insectSource.volume = 0.6f;
    }
    
    public void playRain()
    {
        rainSource.loop = true;
        //rainSource.PlayOneShot(rain);
        rainSource.Play();
        
    }

    public void stopRain()
    {
        rainSource.Pause();
    }
    
    public void stopInsect()
    {
        insectSource.Pause();
    }
}
