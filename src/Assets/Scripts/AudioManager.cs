using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance { get { return _instance; } }

    public AudioSource audioSource1 , birdsSource, insectSource, rainSource;
    public AudioClip insects, step1, step2, step3, step4, rain;
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
        
        if (rect != null && rect.position != previousPosition && !audioSource1.isPlaying)
        {
            playSteps();
            Debug.Log("Playing step");
            
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
        List<AudioClip> stepList = new List<AudioClip> { step1, step2, step3, step4 };
        AudioClip step = stepList[Random.Range(0, 3)];
        _instance.audioSource1.PlayOneShot(step);
    }
    public void playInsects()
    {
        insectSource.loop = true;
        //_instance.insectSource.PlayOneShot(insects);
        _instance.insectSource.Play();
        insectSource.volume = 0.6f;
    }
    public void playRain()
    {
        rainSource.loop = true;
        //_instance.rainSource.PlayOneShot(rain);
        _instance.rainSource.Play();
        
    }

    public void stopRain()
    {
        _instance.rainSource.Pause();
    }
    public void stopInsect()
    {
        _instance.insectSource.Pause();
    }
}
