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

    public AudioSource stepSource, insectSource, rainSource, thunderSource, owls, chickadee;
    public List<AudioClip> steps;
    public List<AudioClip> stepsWet;
    public List<AudioClip> timeJump;
    public List<AudioClip> InventoryClick;
    public List<AudioClip> Notifications;
    private Vector3 previousPosition;
    private bool rainStarted = false;
    

    void Awake()
    {
        _instance = this;
        playRain();
    }

    // Update is called once per frame
    void Update()
    {
        bool isFuture = TimeManager.Instance.IsFuture();
        
        if (transform.position != previousPosition && !stepSource.isPlaying)
        {
            playSteps(isFuture);
            // Debug.Log("Playing step");
        }
        previousPosition = transform.position;

        if (isFuture && !rainStarted)
        {
            playTimeForward();
            stopRain();
            playThunder();
            playOwls();
            playInsects();
            //stopChickadee();
            rainStarted = true;
        }
        if (isFuture && rainStarted)
        {
            playTimeBackward();
            stopThunder();
            stopOwls();
            stopInsect();
            playRain();
            
            //playChickadee();
            rainStarted = false;
        }
    }

    public void playSteps(bool isFuture)
    {
        AudioClip step;
        if (isFuture)
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

    public void playThunder()
    {
        thunderSource.loop = true;
        //rainSource.PlayOneShot(rain);
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
        insectSource.PlayOneShot(time);
    }
    public void playTimeBackward()
    {
        AudioClip time = timeJump[1];
        rainSource.PlayOneShot(time);
    }

    public void playInventory()
    {
        AudioClip inventory = InventoryClick[0];
        insectSource.PlayOneShot(inventory);
    }
    public void playOwls()
    {
        owls.loop = true;
        owls.Play();
    }
    public void stopOwls()
    {
        owls.Pause();
    }

    public void playChickadee()
    {
        chickadee.loop = true;
        chickadee.Play();
    }
    public void stopChickadee()
    {
        chickadee.Pause();
    }
    public void playNotification()
    {
        AudioClip notif = Notifications[0];
        stepSource.PlayOneShot(notif);
    }
}
