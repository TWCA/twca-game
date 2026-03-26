using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using JetBrains.Annotations;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }
    private bool isWaitingForTrigger = false;
    private const float DEFAULT_DELAY = 1.3f;
    public TextAsset inkJson;
    public Transform historyContent;
    public ScrollRect historyScrollRect;
    public GameObject messageBubblePrefab;
    public GameObject notificationBubblePrefab;
    public Transform choicesRoot;
    public GameObject choiceButtonPrefab;
    public Text timeText;

    public GameObject DialogRoot;

    private Story story;
    private bool isRunning = false;
    private System.Action onDialogFinished;


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(Instance.gameObject);
        else
            DontDestroyOnLoad(gameObject);

        Instance = this;

        if (inkJson == null)
            Debug.LogError("DialogManager: inkJson is not assigned!");
        else
            story = new Story(inkJson.text);
    }

    private void LateUpdate()
    {
        if (DialogRoot.activeSelf)
        {
            MoveToCamera();
        }
    }

    /**
     * Opens the story to knot and opens the UI.
     * If you want to play dialog without the UI, call StartDialogHeadless()
     */
    

    private void DisplayDialogLine(string line, List<string> tags)
{
    if (line.Length > 0 && DialogRoot.activeSelf)
    {
        string appTitle = GetNotificationAppTitle(tags);

        if (appTitle == null)
        {
            bool isPlayer = IsTaggedPlayer(tags);
            AddMessage(line, isPlayer);
        }
        else
        {
            AddNotification(appTitle, line);
        }
    }

    HandleVoiceTags(tags);
}
    
    public void StartDialog(string knot, System.Action onFinished = null)
    {
        OpenToKnot(knot, onFinished);
        
        DialogRoot.SetActive(true);
        UpdateDisabledBehaviours();

        if (TimeManager.Instance.IsFuture())
            timeText.text = "9:43pm";
        else
            timeText.text = "11:20am";

        ClearMessages();
        
        ContinueStory();
    }

    /**
     * Opens the story to knot *without* opening the UI.
     * If you want to play dialog in the UI, call StartDialog()
     */


    public void ResumeDialogue()
{
    if (!isWaitingForTrigger) return;

    isWaitingForTrigger = false;

    ContinueStory();
}


    public void StartDialogHeadless(string knot, System.Action onFinished = null)
    {
        OpenToKnot(knot, onFinished);
        ContinueStory();
    }
    private float ExtractDelayFromTags(List<string> tags)
{
    foreach (string tag in tags)
    {
        if (tag.StartsWith("delay:"))
        {
            string value = tag.Replace("delay:", "").Trim();

            if (float.TryParse(value, out float delay))
            {
                return delay;
            }
        }
    }

    return -1f; // means no delay found
}

    /**
     * Opens the story to a knot, handling the onFinished callback.
     */
    private void OpenToKnot(string knot, System.Action onFinished)
    {
        // stop old running story knot to open this one
        if (isRunning)
            EndDialog();
        
        if (inkJson == null)
        {
            Debug.LogError("DialogManager: inkJson is not assigned!");
            return;
        }

        if (!string.IsNullOrEmpty(knot))
        {
            story.ChoosePathString(knot);
            AudioManager.Instance.HalfAll();
        }
        else
            Debug.LogError("No knot location passed");

        if (!story.canContinue)
        {
            Debug.LogError("Knot doesn't have any content");
            onFinished?.Invoke();
            return;
        }

        isRunning = true;
        onDialogFinished = onFinished;
    }

    public void EndDialog()
    {
        isRunning = false;
        DialogRoot.SetActive(false);
        AudioManager.Instance.FullAll();
        onDialogFinished?.Invoke();
        onDialogFinished = null;

        UpdateDisabledBehaviours();
    }

    /**
     * Displays any remaining dialog lines, and the displays a choice.
     */
    private void ContinueStory()
{
    if (isWaitingForTrigger) return;

    if (!story.canContinue)
    {
        if (story.currentChoices.Count > 0)
            RefreshChoices();
        else
            EndDialog();
        return;
    }

    string line = story.Continue().Trim();
    List<string> tags = story.currentTags;

    float delay = ExtractDelayFromTags(tags);
    if (delay < 0) delay = DEFAULT_DELAY;

    bool waitForTrigger = tags.Exists(tag => tag.ToLower() == "waitfortrigger");

    DisplayDialogLine(line, tags);
    ClearChoices();

    if (waitForTrigger)
    {
        isWaitingForTrigger = true;
        return;
    }
    Debug.Log("Delay added: " + delay);
    VAManager.Instance.EnqueueDelay(delay);

    VAManager.Instance.OnQueueEmpty(() =>
    {
        if (isWaitingForTrigger) return;

        if (story.canContinue)
            ContinueStory();
        else if (story.currentChoices.Count > 0)
            RefreshChoices();
        else if (DialogRoot.activeSelf)
            AddChoiceButton("(Put Down Phone)", EndDialog);
        else
            EndDialog();
    });
}
    /**
     * Checks if the dialog line is tagged as coming from the player.
     * If so it should be displayed as so in the messaging UI.
     */
    private bool IsTaggedPlayer(List<string> tags)
    {
        /*
         * Example tags:
         * #Robin
         * #Friend
         */
        if (tags.Contains("Robin"))
        {
            return true;
        }
        else if (tags.Contains("Friend"))
        {
            return false;
        }
        else
        {
            if (DialogRoot.activeSelf)
                Debug.LogWarning("Dialog line was not tagged with #Robin or #Friend, assuming line is from friend");
            return false;
        }
    }

    /**
    * Checks if the dialog line is tagged with a voice line.
    * If this is the case the VA line should be played alongside the text.
    */
    private void HandleVoiceTags(List<string> tags)
    {
        /*
         * Example tags:
         * #Voice:VA/Notifications/EdmontonValleyZoo
         * #Voice:VA/InterLevel/GreatToHear
         */
        foreach (string tag in tags)
        {
            if (tag.StartsWith("Voice:"))
            {
                VAManager.Instance.Enqueue(tag.Replace("Voice:", "").Trim());
            }
            else if (tag == "IgnoreNextVoice")
            {
                VAManager.Instance.IgnoreNextEnqueue();
            }
        }
    }

    /**
     * Checks if the dialog line is tagged with the name of an app.
     * If this is the case the dialog should be displayed as a notification.
     */
    private string GetNotificationAppTitle(List<string> tags)
    {
        /*
         * Example tags:
         * #Notification:Readit
         * #Notification:Instancegram
         */
        foreach (string tag in tags)
        {
            if (tag.StartsWith("Notification:"))
            {
                return tag.Replace("Notification:", "").Trim();
            }
        }

        return null;
    }

    /**
     * Replaces old choices with the current choices from the story.
     */
    private void RefreshChoices()
    {
        ClearChoices();
        foreach (Choice choice in story.currentChoices)
        {
            AddChoiceButton(choice.text, () =>
            {
                story.ChooseChoiceIndex(choice.index);
                ContinueStory();
            });
        }
    }
    
    /**
     * Disables player movement if the UI is open.
     * Enables player movement if the UI is closed.
     */
    private void UpdateDisabledBehaviours()
    {
        bool active = DialogRoot.activeSelf;
        
        GameObject player = GameObject.FindWithTag("Player");
        PlayerControl playerControl = player.GetComponent<PlayerControl>();

        playerControl.enabled = !active;

        if (active)
            playerControl.StopInPlace();
    }

    /**
     * Adds a choice button to the UI for the player to click.
     */
    private void AddChoiceButton(string text, UnityAction callback)
    {
        GameObject buttonObject = Instantiate(choiceButtonPrefab, choicesRoot);
        Button button = buttonObject.GetComponent<Button>();
        Text label = buttonObject.GetComponentInChildren<Text>();

        label.text = text;
        button.onClick.AddListener(callback);
    }


    /**
     * Adds a conversation message to the screen.
     * This is displayed after any previous messages.
     */
    private void AddMessage(string text, bool isPlayer)
    {
        GameObject obj = Instantiate(messageBubblePrefab, historyContent);
        MessageBubble bubble = obj.GetComponent<MessageBubble>();
        bubble.SetMessage(text, isPlayer);

        Canvas.ForceUpdateCanvases();
        historyScrollRect.verticalNormalizedPosition = 0f;
    }

    /**
     * Adds a notification message to the screen from an app.
     * This is displayed after any previous messages.
     * The appTitle argument also selects which app icon to use.
     */
    private void AddNotification(string appTitle, string body)
    {
        GameObject obj = Instantiate(notificationBubblePrefab, historyContent);
        NotificationBubble bubble = obj.GetComponent<NotificationBubble>();
        bubble.SetMessage(appTitle, body);

        Canvas.ForceUpdateCanvases();
        historyScrollRect.verticalNormalizedPosition = 0f;
    }
    
    /**
     * Removes all the option buttons presented to the player.
     */
    private void ClearChoices()
    {
        for (int i = choicesRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(choicesRoot.GetChild(i).gameObject);
        }
    }

    /**
     * Removes all the previous messages displayed in the UI.
     */
    private void ClearMessages()
    {
        for (int i = historyContent.childCount - 1; i >= 0; i--)
        {
            Destroy(historyContent.GetChild(i).gameObject);
        }
    }
    
    /**
     * Move the dialog UI to th main camera's position.
     */
    private void MoveToCamera()
    {
        GameObject camera = GameObject.FindWithTag("MainCamera");
        Vector3 position = camera.transform.position;
        position.z = DialogRoot.transform.position.z;
        DialogRoot.transform.position = position;
    }
}
