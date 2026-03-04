using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VAManager : MonoBehaviour
{
    [SerializeField] public AudioSource VA;
    private List<AudioClip> queue = new List<AudioClip>();
    private bool ignoringNextEnqueue;
    


    public static VAManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        Queue();
    }

    public void Queue()
    {
        if (queue.Count != 0 && !VA.isPlaying)
        {
            VA.PlayOneShot(queue[0]);
            queue.RemoveAt(0);
        }
    }

    public void Enqueue(string filepath)
    {
        if (ignoringNextEnqueue)
        {
            ignoringNextEnqueue = false;
            return;
        }
        
        AudioClip audioClip = Resources.Load<AudioClip>(filepath);
        if (audioClip == null)
            Debug.Log("Failed to load voice clip from path: " + filepath);
        else
            queue.Add(audioClip);
    }

    public void IgnoreNextEnqueue()
    {
        ignoringNextEnqueue = true;
    }
}