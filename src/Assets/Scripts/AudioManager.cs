using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    public static AudioManager Instance
    {
        get { return _instance; }
    }

    // oneShotSource shouldn't be messed with. Just leave it at full volume to play random ones-hot sounds.
    public AudioSource oneShotSource, insectSource, rainSource, thunderSource, owlsSource, chickadeeSource;
    public List<AudioClip> steps;
    public List<AudioClip> stepsWet;
    public List<AudioClip> timeJump;
    public List<AudioClip> InventoryClick;
    public List<AudioClip> Notifications;
    private Vector3 previousPosition;


    void Awake()
    {
        _instance = this;
        playRain();
    }

    void Start()
    {
        TimeManager.Instance.onTimeChanged += () =>
        {
            // the time just changed this frame
            bool isFuture = TimeManager.Instance.IsFuture();
            Debug.Log(isFuture);
            if (isFuture)
                playTimeForward();
            else
                playTimeBackward();
        };
    }

    // Update is called once per frame
    void Update()
    {
        // check if we are moving at more than 1 pixel per second
        bool movingAtSignificantSpeed = Vector2.Distance(transform.position, previousPosition) > Time.deltaTime;
        if (movingAtSignificantSpeed)
            playSteps();
        previousPosition = transform.position;

        float rainStrength = TimeManager.Instance.GetRainStrength();
        if (rainStrength <= 0) // no rain
        {
            stopRain();
            stopThunder();
            playInsects();
        }
        else if (rainStrength > 0 && rainStrength <= 0.5) // light rain
        {
            playRain();
            stopThunder();
            stopInsect();
        }
        else // if rainStrength > 0.5 // heavy rain
        {
            playRain();
            playThunder();
            stopInsect();
        }
        
        float lighting = TimeManager.Instance.GetLightingTime();
        if (lighting <= 0.2) // day
        {
            playChickadee();
            stopOwls();
        }
        else if (lighting < 0.8) // dawn
        {
            playChickadee();
            playOwls();
        }
        else // if rainStrength >= 0.8 // night
        {
            stopChickadee();
            playOwls();
        }
    }

    public void playSteps()
    {
        bool isFuture = TimeManager.Instance.IsFuture();

        AudioClip step;
        if (isFuture)
        {
            step = stepsWet[Random.Range(0, stepsWet.Count)];
        }
        else
        {
            step = steps[Random.Range(0, steps.Count)];
        }

        if (!oneShotSource.isPlaying)
            oneShotSource.PlayOneShot(step);
    }

    public void playInsects()
    {
        insectSource.loop = true;
        
        if (!insectSource.isPlaying)
            insectSource.Play();
        
        insectSource.volume = 0.6f;
    }

    public void playRain()
    {
        rainSource.loop = true;
        
        if (!rainSource.isPlaying)
            rainSource.Play();
    }

    public void stopRain()
    {
        rainSource.Pause();
    }

    public void playThunder()
    {
        thunderSource.loop = true;
        
        if (!thunderSource.isPlaying)
            thunderSource.Play();
    }

    public void stopThunder()
    {
        thunderSource.Pause();
    }

    public void stopInsect()
    {
        insectSource.Pause();
    }

    public void playTimeForward()
    {
        AudioClip time = timeJump[0];
        oneShotSource.PlayOneShot(time);
    }

    public void playTimeBackward()
    {
        AudioClip time = timeJump[1];
        oneShotSource.PlayOneShot(time);
    }

    public void playInventory()
    {
        AudioClip inventory = InventoryClick[0];
        insectSource.PlayOneShot(inventory);
    }

    public void playOwls()
    {
        // TODO: this plays over and over again annoyingly, it should have random gaps in between
        owlsSource.loop = true;

        if (!owlsSource.isPlaying)
            owlsSource.Play();
    }

    public void stopOwls()
    {
        owlsSource.Pause();
    }

    public void playChickadee()
    {
        // TODO: this plays over and over again annoyingly, it should have random gaps in between
        chickadeeSource.loop = true;
        
        if (!chickadeeSource.isPlaying)
            chickadeeSource.Play();
    }

    public void stopChickadee()
    {
        chickadeeSource.Pause();
    }

    public void playNotification()
    {
        AudioClip notif = Notifications[0];
        oneShotSource.PlayOneShot(notif);
    }
}