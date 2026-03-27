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
    private string nextTrack;
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
                musicSource.Stop();
                if (nextTrack.Trim().Length > 0)
                {
                    PlayOnce(nextTrack, nextTrackVolume);
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
            nextTrackVolume = volume;
            return;
        }
        
        AudioClip clip = Resources.Load<AudioClip>(filePath.Trim());

        if (clip == null)
        {
            Debug.LogWarning("Failed to load voice clip from path: " + filePath);
            return;
        }

        musicSource.clip = clip;
        musicSource.loop = false;
        musicSource.volume = volume;
        musicSource.Play();
    }
}