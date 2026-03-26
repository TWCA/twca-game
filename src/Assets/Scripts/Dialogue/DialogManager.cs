using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using JetBrains.Annotations;
using UnityEngine.Events;
using UnityEngine.Rendering;

public enum Character
{
    Robin,
    Mom,
    Sam,
    Francis,
    Lorenzo,
    Police
}

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }
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
    private bool areBehavioursDisabled = false;

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
    public void StartDialog(string knot, System.Action onFinished = null)
    {
        OpenToKnot(knot, onFinished);
        OpenPhoneUI();
        DisableBehaviours();
        ContinueStory();
    }

    /**
     * Opens the story to knot *without* opening the UI.
     * If you want to play dialog in the UI, call StartDialog()
     */
    public void StartDialogHeadless(string knot, System.Action onFinished = null)
    {
        OpenToKnot(knot, onFinished);
        ContinueStory();
    }

    /**
     * Makes the phone UI visible
     */
    private void OpenPhoneUI()
    {
        DialogRoot.SetActive(true);

        if (TimeManager.Instance.IsFuture())
            timeText.text = "9:43pm";
        else
            timeText.text = "11:20am";

        ClearMessages();

        AudioManager.Instance.PlayNotification();
    }
    
    /**
     * Makes the phone UI invisible
     */
    private void ClosePhoneUI()
    {
        DialogRoot.SetActive(false);
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
        ClosePhoneUI();
        AudioManager.Instance.FullAll();

        onDialogFinished?.Invoke();
        onDialogFinished = null;
        
        EnableBehaviours();
    }

    /**
     * Displays any remaining dialog lines, and the displays a choice.
     */
    private void ContinueStory()
    {
        string line = story.Continue().Trim();
        List<string> tags = story.currentTags;
        
        HandleDialogControl(tags);
        DisplayDialogLine(line, tags);

        ClearChoices();

        // Wait for the VA line to stop playing
        VAManager.Instance.OnQueueEmpty(() =>
        {
            if (story.canContinue)
                ContinueStory(); // There are more lines to get...
            else if (story.currentChoices.Count > 0)
                RefreshChoices();
            else if (DialogRoot.activeSelf)
                AddChoiceButton("(Put Down Phone)", EndDialog);
            else
            {
                EndDialog();
            }
        });
    }

    /**
     * Displays a dialog as a message or notification.
     * Also triggers voice acting lines to play.
     */
    private void DisplayDialogLine(string line, List<string> tags)
    {
        if (line.Length > 0 && DialogRoot.activeSelf)
        {
            string appTitle = GetNotificationAppTitle(tags);

            if (appTitle == null)
            {
                Character character = GetCharacterTag(tags);
                AddMessage(line, character);
            }
            else
            {
                AddNotification(appTitle, line);
            }
        }
    }

    /**
     * Handle control related tags, such as voice and UI control
     */
    private void HandleDialogControl(List<string> tags)
    {
        HandleVoiceTags(tags);

        if (tags.Contains("openPhone"))
            OpenPhoneUI();
        
        if (tags.Contains("closePhone"))
            ClosePhoneUI();
        
        if (tags.Contains("disableBehaviours"))
            DisableBehaviours();
        
        if (tags.Contains("enableBehaviours"))
            EnableBehaviours();

        if (tags.Contains("NotificationSound"))
            AudioManager.Instance.PlayNotification();

        if (tags.Contains("earlyFinishedCallback"))
        {
            onDialogFinished?.Invoke();
            onDialogFinished = null;
        }
    }

    /**
     * Checks if the dialog line is tagged as coming from the player.
     * If so it should be displayed as so in the messaging UI.
     */
    private Character GetCharacterTag(List<string> tags)
    {
        /*
         * Example tags:
         * #Robin
         * #Friend
         */
        if (tags.Contains("Robin"))
            return Character.Robin;

        if (tags.Contains("Sam"))
            return Character.Sam;

        if (tags.Contains("Mom"))
            return Character.Mom;

        if (tags.Contains("Francis"))
            return Character.Francis;

        if (tags.Contains("Lorenzo"))
            return Character.Lorenzo;

        if (tags.Contains("Police"))
            return Character.Police;

        if (DialogRoot.activeSelf)
            Debug.LogWarning("Dialog line was not tagged with any names, assuming Robin");
        return Character.Robin;
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
     * Disables things like player movement while using the phone
     */
    public void DisableBehaviours()
    {
        areBehavioursDisabled = true;
        UpdateDisabledBehaviours();
    }

    /**
     * Enables normal behavour
     */
    public void EnableBehaviours()
    {
        areBehavioursDisabled = false;
        UpdateDisabledBehaviours();
    }

    /**
     * Disables player movement if needed.
     */
    private void UpdateDisabledBehaviours()
    {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerControl playerControl = player.GetComponent<PlayerControl>();

        playerControl.enabled = !areBehavioursDisabled;

        if (areBehavioursDisabled)
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
    private void AddMessage(string text, Character character)
    {
        GameObject obj = Instantiate(messageBubblePrefab, historyContent);
        MessageBubble bubble = obj.GetComponent<MessageBubble>();
        bubble.SetMessage(text, character);

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