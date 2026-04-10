using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using UnityEngine.Events;

public enum Character
{
    Robin,
    Mom,
    Sam,
    Francis,
    Lorenzo,
    Police,
    None
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
    public float defaultTextDelay = 0.9f;
    public float defaultNonTextDelay = 0.2f;

    public GameObject DialogRoot;

    private Story story;
    private GameObject sam;

    private bool isRunning = false;
    private bool isPhoneUp = false;
    private int waitingForTriggerCount = 0;
    private float visualOffset = 270;
    private bool areBehavioursDisabled = false;

    private System.Action onDialogFinished;
    private float delayAfterFinish;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        if (inkJson == null)
            Debug.LogError("DialogManager: inkJson is not assigned!");
        else
            story = new Story(inkJson.text);
    }

    private void Update()
    {
        if (isPhoneUp)
        {
            // lerp movement
            visualOffset = Mathf.Lerp(visualOffset, 0, Time.deltaTime * 5.0f);
            // linear movement
            visualOffset = Mathf.MoveTowards(visualOffset, 0, Time.deltaTime * 70.0f);
        }
        else
        {
            // lerp movement
            visualOffset = Mathf.Lerp(visualOffset, 270, Time.deltaTime * 5.0f);
            // linear movement
            visualOffset = Mathf.MoveTowards(visualOffset, 270, Time.deltaTime * 70.0f);
        }

        DialogRoot.SetActive(visualOffset < 270);

        if (DialogRoot.activeSelf)
        {
            float time = TimeManager.Instance.GetLightingTime();
            float startHour = 9.72f;
            float endHour = 23.33f;
            float currentHour = Mathf.Lerp(startHour, endHour, time);

            string postfix = "am";
            if (currentHour >= 13)
            {
                currentHour -= 12;
                postfix = "pm";
            }

            float currentMinute = (currentHour - Mathf.Floor(currentHour)) * 60;

            currentHour = Mathf.Floor(currentHour);
            currentMinute = Mathf.Floor(currentMinute);

            timeText.text = currentHour + ":" + currentMinute + postfix;
        }
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
    public void StartDialog(string knot, System.Action onFinished = null, float _delayAfterFinish = 0)
    {
        delayAfterFinish = _delayAfterFinish;
        OpenToKnot(knot, onFinished);
        OpenPhoneUI();
        DisableBehaviours();
        ContinueStoryWithDelays();
    }

    /**
     * Opens the story to knot *without* opening the UI.
     * If you want to play dialog in the UI, call StartDialog()
     */
    public void StartDialogHeadless(string knot, System.Action onFinished = null, float _delayAfterFinish = 0)
    {
        delayAfterFinish = _delayAfterFinish;
        OpenToKnot(knot, onFinished);
        ContinueStoryWithDelays();
    }

    /**
     * Makes the phone UI visible
     */
    private void OpenPhoneUI()
    {
        visualOffset = 270f;
        isPhoneUp = true;
        MoveToCamera();
        ClearMessages();

        AudioManager.Instance.PlayNotification();
    }

    public void ResumeDialogue()
    {
        waitingForTriggerCount -= 1;
        if (waitingForTriggerCount == 0)
            VAManager.Instance.OnQueueEmpty(ContinueStoryWithDelays);
    }

    /**
     * Makes the phone UI invisible
     */
    private void ClosePhoneUI()
    {
        isPhoneUp = false;
    }

    /**
     * Opens the story to a knot, handling the onFinished callback.
     */
    private void OpenToKnot(string knot, System.Action onFinished)
    {
        // stop old running story knot to open this one
        if (isRunning)
            EndDialogInstantly();

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
        ClearChoices();

        isRunning = false;

        DialogRoot.SetActive(false);
        AudioManager.Instance.FullAll();

        StartCoroutine(EndDialogCoroutine());
    }

    private IEnumerator EndDialogCoroutine()
    {
        onDialogFinished?.Invoke();
        onDialogFinished = null;

        yield return new WaitForSeconds(delayAfterFinish);

        EndDialogInstantly();
    }

    public void EndDialogInstantly()
    {
        onDialogFinished?.Invoke();
        onDialogFinished = null;

        isRunning = false;

        ClosePhoneUI();
        AudioManager.Instance.FullAll();

        EnableBehaviours();
    }

    private void ContinueStoryWithDelays()
    {
        ContinueStory(false);
    }

    private void ContinueStoryWithoutDelays()
    {
        ContinueStory(true);
    }

    /**
     * Displays any remaining dialog lines, and the displays a choice.
     */
    private void ContinueStory(bool skipNextDelay)
    {
        if (waitingForTriggerCount > 0) return;

        if (!story.canContinue)
        {
            if (story.currentChoices.Count > 0)
            {
                RefreshChoices();
            }
            else if (DialogRoot.activeSelf)
            {
                ClearChoices();
                AddChoiceButton("(Put Down Phone)", EndDialog);
            }
            else
            {
                EndDialog();
            }

            return;
        }

        string line = story.Continue().Trim();
        List<string> tags = story.currentTags;

        // handle line
        HandleDialogControl(tags, skipNextDelay);
        HandleVoiceTags(tags);

        // display when audio starts
        VAManager.Instance.OnAudioStarted(() => DisplayDialogLine(line, tags));

        // continue when finished
        VAManager.Instance.OnQueueEmpty(ContinueStoryWithDelays);

        if (!HasChoiceContaining("(Skip)"))
        {
            ClearChoices();
            AddChoiceButton("(Skip)", () =>
            {
                // act as if the line has started
                VAManager.Instance.RunAudioStartedCallbacks();

                // cancel our callback after the line
                VAManager.Instance.CancelOnQueueEmpty(ContinueStoryWithDelays);

                // clear the queue
                VAManager.Instance.ClearQueue();

                // continue story, skipping the next delay
                ContinueStoryWithoutDelays();
            });
        }
    }

    /**
     * Displays a dialog as a message or notification.
     * Also triggers voice acting lines to play.
     */
    private void DisplayDialogLine(string line, List<string> tags)
    {
        if (line.Length > 0 && isPhoneUp)
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
    private void HandleDialogControl(List<string> tags, bool skipNextDelay)
    {
        if (tags.Exists(tag => tag.ToLower() == "waitfortrigger"))
            waitingForTriggerCount++;

        if (tags.Contains("openPhone"))
            OpenPhoneUI();

        if (tags.Contains("closePhone"))
            ClosePhoneUI();

        if (tags.Contains("disableBehaviours"))
            DisableBehaviours();

        if (tags.Contains("enableBehaviours"))
            EnableBehaviours();

        if (tags.Contains("notificationSound"))
            AudioManager.Instance.PlayNotification();

        if (tags.Contains("earlyFinishedCallback"))
        {
            onDialogFinished?.Invoke();
            onDialogFinished = null;
        }

        if (tags.Contains("disableSam"))
        {
            sam = GameObject.FindGameObjectWithTag("Sam");
            sam.SetActive(false);
        }

        if (tags.Contains("enableSam"))
            sam.SetActive(true);

        if (tags.Contains("ReturnToMainMenu"))
            StartCoroutine(TransitionController.Instance.SwitchScenes("MainMenu", ""));

        if (!skipNextDelay)
        {
            // find and queue delay
            string appTitle = GetNotificationAppTitle(tags);
            Character character = GetCharacterTag(tags);

            if (!(character == Character.Robin && isPhoneUp) && appTitle == null)
            {
                Nullable<float> delay = ExtractDelayFromTags(tags);

                float defaultDelay = isPhoneUp ? defaultTextDelay : defaultNonTextDelay;
                VAManager.Instance.EnqueueDelay(delay ?? defaultDelay);
            }
        }
    }

    private Nullable<float> ExtractDelayFromTags(List<string> tags)
    {
        foreach (string tag in tags)
        {
            if (tag.ToLower().StartsWith("delay:"))
            {
                string value = tag.ToLower().Replace("delay:", "").Trim();

                if (float.TryParse(value, out float delay))
                {
                    return delay;
                }
            }
        }

        return null; // means no delay found
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

        return Character.None;
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
                ClearChoices();
                ContinueStoryWithDelays();
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

    private bool HasChoiceContaining(string query)
    {
        for (int i = choicesRoot.childCount - 1; i >= 0; i--)
        {
            GameObject button = choicesRoot.GetChild(i).gameObject;
            Text label = button.GetComponentInChildren<Text>();
            if (label.text.Contains(query))
                return true;
        }

        return false;
    }

    /**
     * Adds a conversation message to the screen.
     * This is displayed after any previous messages.
     */
    private void AddMessage(string text, Character character)
    {
        if (!isPhoneUp) return;

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
        if (!isPhoneUp) return;

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
        position.y -= visualOffset;
        DialogRoot.transform.position = position;
    }
}