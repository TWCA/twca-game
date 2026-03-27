using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance { get; private set; }

    [SerializeField] private AudioSource musicSource;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(Instance.gameObject);
        else
            DontDestroyOnLoad(gameObject);

        Instance = this;
    }

    public void Play(string FilePath)
    {
        AudioClip clip = Resources.Load<AudioClip>(FilePath.Trim());
        if (clip == null)
            Debug.Log("Failed to load voice clip from path: " + FilePath);
        else
        {
            musicSource.clip = Resources.Load<AudioClip>(FilePath);
            musicSource.Play();
        }
            
    }

    public void PlayOnce(string FilePath)
    {
        AudioClip clip = Resources.Load<AudioClip>(FilePath.Trim());
        
        if (clip == null)
            Debug.LogWarning("Failed to load voice clip from path: " + FilePath);
        else
            musicSource.PlayOneShot(clip);
    }

}