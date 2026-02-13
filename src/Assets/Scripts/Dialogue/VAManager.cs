using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VAManager : MonoBehaviour
{
    [SerializeField] public AudioSource VA;
    public List<AudioClip> Scene1Robin;
    public List<AudioClip> Scene1Friend;
    private List<AudioClip> queue = new List<AudioClip>();
    private AudioClip lastQueue;

    
    public static VAManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        lastQueue = Scene1Friend[0];
    }

    // Update is called once per frame
    void Update()
    {
        Queue();
    }

    public void startScene()
    {
        VA.PlayOneShot(Scene1Friend[0]);

    }
    public void Queue()
    {
        if (!(queue.Count == 0 ) && !VA.isPlaying)
        {
            //lastQueue = Scene1Robin[0];
            if ((lastQueue == Scene1Friend[6] || lastQueue == Scene1Friend[8]) && queue[0] == Scene1Friend[7])
            {
                queue.RemoveAt(0);
                return;
            }
            VA.PlayOneShot(queue[0]);
            lastQueue = queue[0];
            queue.RemoveAt(0);
            
        }
    }

    public void Enqueue(string tag)
    {
        if (tag == "R1B")
        {
            queue.Add(Scene1Robin[0]);
        } 
        if (tag == "R1C")
        {
            queue.Add(Scene1Robin[1]);
        }
        if (tag == "R2A")
        {
            queue.Add(Scene1Robin[2]);
        }
        if (tag == "R2B")
        {
            queue.Add(Scene1Robin[3]);
        }
        if (tag == "R3A")
        {
            queue.Add(Scene1Robin[4]);
        }
        if (tag == "RF2")
        {
            queue.Add(Scene1Robin[5]);
        }
        if (tag == "F2A")
        {
            queue.Add(Scene1Friend[1]);
        }
        if (tag == "F2B")
        {
            queue.Add(Scene1Friend[2]);
            queue.Add(Scene1Friend[6]);
        }
        if (tag == "F3A")
        {
            queue.Add(Scene1Friend[3]);
        }
        if (tag == "F3B")
        {
            queue.Add(Scene1Friend[4]);
        }
        if (tag == "F4A")
        {
            queue.Add(Scene1Friend[5]);
            queue.Add(Scene1Friend[8]);
        }
        
        if (tag == "FF")
        {
            queue.Add(Scene1Friend[7]);
        }
    }
}
