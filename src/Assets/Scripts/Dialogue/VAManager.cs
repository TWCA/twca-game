using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VAManager : MonoBehaviour
{
    [SerializeField] public AudioSource VA;
    public List<AudioClip> Scene1;
    private List<AudioClip> queue = new List<AudioClip>();
    //private AudioClip lastQueue;

    
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
        if (!(queue.Count == 0 ) && !VA.isPlaying)
        { 
            VA.PlayOneShot(queue[0]);
            //lastQueue = queue[0];
            queue.RemoveAt(0);
        }
    }

    public void Enqueue(string tag)
    {
        foreach (AudioClip clip in Scene1)
        {
            if (clip.name == tag)
            {
                queue.Add(clip);
            }
        }
    }
}
