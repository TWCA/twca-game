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
    public AudioClip insects, rain;
    private Vector3 previousPosition;
    private RectTransform rect;
    

    void Awake()
    {
        _instance = this;
        rect = GetComponent<RectTransform>();
        playInsects();
        playRain();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (rect != null && rect.position != previousPosition && !stepSource.isPlaying)
        {
            playSteps();
            // Debug.Log("Playing step");
        }
        previousPosition = rect.position;
        //if (!insectSource.isPlaying)
        //{
        //    playInsects();
        //}
        //if (!rainSource.isPlaying)
        //{
        //    playRain();
        //}
    }

    public void playSteps()
    {
        AudioClip step = steps[Random.Range(0, steps.Count)];
        stepSource.PlayOneShot(step);
    }
    
    public void playInsects()
    {
        insectSource.clip = insects;
        insectSource.loop = true;
        //insectSource.PlayOneShot(insects);
        insectSource.Play();
        insectSource.volume = 0.6f;
    }
    
    public void playRain()
    {
        insectSource.clip = rain;
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
