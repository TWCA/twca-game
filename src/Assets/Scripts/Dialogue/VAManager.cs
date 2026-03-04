using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class VAManager : MonoBehaviour
{
    [FormerlySerializedAs("VA")] [SerializeField]
    public AudioSource VAaudioSource;

    private List<AudioClip> queue = new List<AudioClip>();
    private bool ignoringNextEnqueue = false;
    private List<UnityAction> queueEmptyActions = new List<UnityAction>();

    public static VAManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        RunQueue();
    }

    public void RunQueue()
    {
        if (VAaudioSource.isPlaying) return;

        if (queue.Count == 0)
        {
            // copy so that current actions can't add more actions
            List<UnityAction> remainingActions = queueEmptyActions;
            queueEmptyActions = new List<UnityAction>();

            // run actions
            foreach (UnityAction remainingAction in remainingActions)
                remainingAction();
        }
        else
        {
            VAaudioSource.PlayOneShot(queue[0]);
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

    public void OnQueueEmpty(UnityAction callback)
    {
        queueEmptyActions.Add(callback);
    }
}