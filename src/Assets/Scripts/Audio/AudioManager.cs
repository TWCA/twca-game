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
    public AudioSource oneShotSource, insectSource, rainSource, thunderSource, owlsSource, chickadeeSource, RiverSourcePast, RiverSourceFuture;
    public List<AudioClip> steps;
    public List<AudioClip> stepsWet;
    public List<AudioClip> timeJump;
    public List<AudioClip> InventoryClick;
    public List<AudioClip> Notifications;
    private Vector3 previousPosition;
    public List<AudioClip> Birds;
    private bool playingChickadee, playingOwls = false;

    void Awake()
    {
        _instance = this;
        FullAll();
        playRain();
        insectSource.volume = 0.6f;
    }

    void Start()
    {
        TimeManager.Instance.onTimeChanged += () =>
        {
            // the time just changed this frame
            bool isFuture = TimeManager.Instance.IsFuture();
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
        //Debug.Log(rainStrength);
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
        //Debug.Log(lighting);
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
        if (RiverSourceFuture != null && RiverSourcePast != null)
        {
            if (lighting <= 0.2)
            {
                playRiverPast();
                stopRiverFuture();
            }
            else if (lighting < 0.8)
            {
                stopRiverPast();
                stopRiverFuture();
            }
            else
            {
                stopRiverPast();
                playRiverFuture();
            }
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
        
        //insectSource.volume = 0.6f;
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
    public void playRiverPast()
    {
        RiverSourcePast.loop = true;
        if (!RiverSourcePast.isPlaying)
            RiverSourcePast.Play();
    }

    public void stopRiverPast()
    {
        RiverSourcePast.Pause();
    }
    public void playRiverFuture()
    {
        RiverSourceFuture.loop = true;
        if (!RiverSourceFuture.isPlaying)
            RiverSourceFuture.Play();
    }

    public void stopRiverFuture()
    {
        RiverSourceFuture.Pause();
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
        owlsSource.mute = false;

        if (!playingOwls)
            StartCoroutine(PlayWithGap(owlsSource));
    }

    public void stopOwls()
    {
        owlsSource.mute = true;
    }

    public void playChickadee()
    {
        chickadeeSource.mute = false;

        if (!playingChickadee)
            StartCoroutine(PlayWithGap(chickadeeSource));
    }

    public void stopChickadee()
    {
        chickadeeSource.mute = true;
    }

    public void playNotification()
    {
        AudioClip notif = Notifications[0];
        oneShotSource.PlayOneShot(notif);
    }

    private IEnumerator PlayWithGap(AudioSource aud)
    {
        if (aud == chickadeeSource)
        {
            playingChickadee = true;
            float rand = Random.Range(3f, 30);
            aud.PlayOneShot(Birds[0]);
            yield return new WaitForSeconds(rand);
            playingChickadee = false;
        } else if (aud == owlsSource)
        {
            playingOwls = true;
            float rand = Random.Range(9f, 15f);
            aud.PlayOneShot(Birds[1]);
            yield return new WaitForSeconds(rand);
            playingOwls = false;
        }
    }

    private void halfVolume(AudioSource aud)
    {
        aud.volume = 0.4f;
    }

    private void fullVolume(AudioSource aud)
    {
        aud.volume = 0.8f;
    }
    public void HalfAll()
    {
        insectSource.volume = 0.3f;
        halfVolume(rainSource);
        halfVolume(thunderSource);
        halfVolume(owlsSource);
        halfVolume(chickadeeSource);
        halfVolume(RiverSourcePast);
        halfVolume(RiverSourceFuture);
    }
    public void FullAll()
    {
        insectSource.volume = 0.6f;
        fullVolume(rainSource);
        fullVolume(thunderSource);
        fullVolume(owlsSource);
        fullVolume(chickadeeSource);
        fullVolume(RiverSourcePast);
        fullVolume(RiverSourceFuture);
    }
}