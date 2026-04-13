using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance { get; private set; }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private string playOnOpening;
    [SerializeField] private float playOnOpenVolume = 1.0f;

    private bool fadingOut = false;
    private string nextTrack = "";
    private float nextTrackVolume;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        // play on whatever is the valid instance is now
        if (playOnOpening.Trim().Length > 0)
            Instance.PlayOnce(playOnOpening, playOnOpenVolume);
    }

    public void Update()
    {
        if (fadingOut)
        {
            musicSource.volume = Mathf.Lerp(musicSource.volume, 0, Time.deltaTime * 2);
            if (musicSource.volume < Time.deltaTime * 2)
            {
                fadingOut = false;
                Debug.Log("Stoped fading out");
                musicSource.Stop();
                if (nextTrack.Trim().Length > 0)
                {
                    PlayOnce(nextTrack, nextTrackVolume);
                    Debug.Log("Playing Next Track: "+ nextTrack);
                    nextTrack = "";
                }
            }
        }
    }

    public void PlayOnce(string filePath, float volume = 1.0f)
    {
        if (musicSource.isPlaying)
        {
            fadingOut = true;
            nextTrack = filePath;
            Debug.Log("source playing");
            nextTrackVolume = volume;
            return;
            
        }
        AudioClip clip = null;
        clip = Resources.Load<AudioClip>(filePath.Trim());

        if (clip == null)
        {
            Debug.LogWarning("Failed to load voice clip from path: " + filePath);
            return;
        }else{
            Debug.Log("Clip loaded.");
        }

        musicSource.clip = clip;
        musicSource.loop = false;
        musicSource.volume = volume;
        musicSource.Play();
    }

    public void loopTrack()
    {
        musicSource.loop = true;
        musicSource.Play();
    }

    public void stopPlayer()
    {
        fadingOut = true;
        musicSource.loop = false;
    }
}