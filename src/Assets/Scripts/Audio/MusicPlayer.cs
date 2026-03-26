using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private static MusicPlayer _instance;

    public static MusicPlayer Instance
    {
        get { return _instance; }
    }
    [SerializeField] private AudioSource musicSource;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        _instance = this;
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
            Debug.Log("Failed to load voice clip from path: " + FilePath);
        musicSource.PlayOneShot(clip);
    }

}