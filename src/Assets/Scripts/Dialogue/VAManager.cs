using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VAManager : MonoBehaviour
{
    [SerializeField] public AudioSource vaAudioSource;

    // Command Queue (supports audio + delay)
    private Queue<VACommand> queue = new Queue<VACommand>();

    private bool ignoringNextEnqueue = false;
    private bool isProcessing = false;

    private List<UnityAction> queueEmptyActions = new List<UnityAction>();

    public static VAManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    // =========================
    // COMMAND STRUCTURE
    // =========================
    public class VACommand
    {
        public enum CommandType
        {
            PlayAudio,
            Delay
        }

        public CommandType Type;
        public AudioClip Clip;
        public float Delay;

        public VACommand(AudioClip clip)
        {
            Type = CommandType.PlayAudio;
            Clip = clip;
        }

        public VACommand(float delay)
        {
            Type = CommandType.Delay;
            Delay = delay;
        }
    }

    // =========================
    // PUBLIC API
    // =========================

    public void Enqueue(string filepath)
    {
        if (ignoringNextEnqueue)
        {
            ignoringNextEnqueue = false;
            return;
        }

        AudioClip audioClip = Resources.Load<AudioClip>(filepath);

        if (audioClip == null)
        {
            Debug.LogError("Failed to load voice clip from path: " + filepath);
            return;
        }

        queue.Enqueue(new VACommand(audioClip));
        StartQueue();
    }

    public void EnqueueDelay(float seconds)
    {
        queue.Enqueue(new VACommand(seconds));
        StartQueue();
    }

    public void IgnoreNextEnqueue()
    {
        ignoringNextEnqueue = true;
    }

    public void OnQueueEmpty(UnityAction callback)
    {
        if (!isProcessing)
            callback();
        else
            queueEmptyActions.Add(callback);
    }

    // =========================
    // CORE PROCESSING
    // =========================

    private void StartQueue()
    {
        if (!isProcessing)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        isProcessing = true;

        while (queue.Count > 0)
        {
            VACommand cmd = queue.Dequeue();

            if (cmd.Type == VACommand.CommandType.PlayAudio)
            {
                vaAudioSource.PlayOneShot(cmd.Clip);

                // Wait for clip to finish
                yield return new WaitForSeconds(cmd.Clip.length);
            }
            else if (cmd.Type == VACommand.CommandType.Delay)
            {
                yield return new WaitForSeconds(cmd.Delay);
            }
        }

        isProcessing = false;

        // Run all queue-empty callbacks safely
        List<UnityAction> remainingActions = queueEmptyActions;
        queueEmptyActions = new List<UnityAction>();

        foreach (UnityAction action in remainingActions)
        {
            action();
        }
    }
}