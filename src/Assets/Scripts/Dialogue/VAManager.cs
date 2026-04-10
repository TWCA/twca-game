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
    private float queueDelayTime = 0;
    
    private List<UnityAction> queueEmptyActions = new List<UnityAction>();
    private List<UnityAction> audioStartedActions = new List<UnityAction>();

    public static VAManager Instance { get; private set; }

    public void Awake()
    {
        Instance = this;
    }

    public void Update()
    {
        // wait for delays
        if (queueDelayTime > 0)
        {
            queueDelayTime -= Time.deltaTime;
            return;
        }

        // otherwise, dequeue a command
        if (queue.Count > 0)
        {
            RunNextCommand();
            return;
        }

        // otherwise, run all queue-empty callbacks safely
        RunEmptyQueueCallbacks();
    }

    private void RunNextCommand()
    {
        VACommand cmd = queue.Dequeue();

        if (cmd.Type == VACommand.CommandType.PlayAudio)
        {
            vaAudioSource.PlayOneShot(cmd.Clip);
            queueDelayTime += cmd.Clip.length;
            RunAudioStartedCallbacks();
        }
        else if (cmd.Type == VACommand.CommandType.Delay)
        {
            queueDelayTime += cmd.Delay;
        }
    }

    public void RunEmptyQueueCallbacks()
    {
        List<UnityAction> remainingActions = queueEmptyActions;
        queueEmptyActions = new List<UnityAction>();

        foreach (UnityAction action in remainingActions)
            action();
    }
    
    public void RunAudioStartedCallbacks()
    {
        List<UnityAction> remainingActions = audioStartedActions;
        audioStartedActions = new List<UnityAction>();

        foreach (UnityAction action in remainingActions)
            action();
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
        {
            Debug.LogError("Failed to load voice clip from path: " + filepath);
            return;
        }

        queue.Enqueue(new VACommand(audioClip));
    }

    public void EnqueueDelay(float seconds)
    {
        queue.Enqueue(new VACommand(seconds));
    }

    public void IgnoreNextEnqueue()
    {
        ignoringNextEnqueue = true;
    }

    public void ClearQueue()
    {
        queueDelayTime = 0;
        vaAudioSource.Stop();
        queue.Clear();
        RunEmptyQueueCallbacks();
    }

    public bool IsQueueEmpty()
    {
        return queueDelayTime <= 0 && queue.Count == 0;
    }

    public void OnQueueEmpty(UnityAction callback)
    {
        queueEmptyActions.Add(callback);
    }
    
    public void CancelOnQueueEmpty(UnityAction callback)
    {
        queueEmptyActions.Remove(callback);
    }
    
    public void OnAudioStarted(UnityAction callback)
    {
        audioStartedActions.Add(callback);
    }
    
    public void CancelOnAudioStarted(UnityAction callback)
    {
        audioStartedActions.Remove(callback);
    }
    
    private class VACommand
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
}