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
        if (isRunning)
        {
            MoveToCamera();
        }
    }

    private void UpdateDisabledBehaviours()
    {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerControl playerControl = player.GetComponent<PlayerControl>();

        playerControl.enabled = !isRunning;

        if (isRunning)
            playerControl.StopInPlace();
    }

    public void StartDialog(string knot, System.Action onFinished = null)
    {
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
        DialogRoot.SetActive(true);

        onDialogFinished = onFinished;

        UpdateDisabledBehaviours();

        if (TimeManager.Instance.IsFuture())
            timeText.text = "9:43pm";
        else
            timeText.text = "11:20am";

        ClearMessages();
        ContinueStory();
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

    private void ContinueStory()
    {
        string line = story.Continue().Trim();
        List<string> tags = story.currentTags;

        if (line.Length > 0)
        {
            string notificationTitle = GetNotificationTitle(tags);

            if (notificationTitle == null)
            {
                bool isPlayer = IsTaggedPlayer(tags);
                AddMessage(line, isPlayer);
            }
            else
            {
                AddNotification(notificationTitle, line);
            }
        }

        HandleVoiceTags(tags);

        ClearChoices();
        VAManager.Instance.OnQueueEmpty(() =>
        {
            if (story.canContinue)
                ContinueStory();
            else if (story.currentChoices.Count > 0)
                RefreshChoices();
            else
                AddChoiceButton("(Put Down Phone)", EndDialog);
        });
    }

    private bool IsTaggedPlayer(List<string> tags)
    {
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
            Debug.LogWarning("Dialog line was not tagged with #Robin or #Friend, assuming line is from friend");
            return false;
        }
    }

    private void HandleVoiceTags(List<string> tags)
    {
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

    private string GetNotificationTitle(List<string> tags)
    {
        foreach (string tag in tags)
        {
            if (tag.StartsWith("Notification:"))
            {
                return tag.Replace("Notification:", "").Trim();
            }
        }

        return null;
    }

    private void RefreshChoices()
    {
        foreach (Choice choice in story.currentChoices)
        {
            AddChoiceButton(choice.text, () =>
            {
                story.ChooseChoiceIndex(choice.index);
                ContinueStory();
            });
        }
    }

    private void AddChoiceButton(string text, UnityAction callback)
    {
        GameObject buttonObject = Instantiate(choiceButtonPrefab, choicesRoot);
        Button button = buttonObject.GetComponent<Button>();
        Text label = buttonObject.GetComponentInChildren<Text>();

        label.text = text;
        button.onClick.AddListener(callback);
    }


    private void AddMessage(string text, bool isPlayer)
    {
        GameObject obj = Instantiate(messageBubblePrefab, historyContent);
        MessageBubble bubble = obj.GetComponent<MessageBubble>();
        bubble.SetMessage(text, isPlayer);

        Canvas.ForceUpdateCanvases();
        historyScrollRect.verticalNormalizedPosition = 0f;
    }

    private void AddNotification(string title, string body)
    {
        GameObject obj = Instantiate(notificationBubblePrefab, historyContent);
        NotificationBubble bubble = obj.GetComponent<NotificationBubble>();
        bubble.SetMessage(title, body);

        Canvas.ForceUpdateCanvases();
        historyScrollRect.verticalNormalizedPosition = 0f;
    }


    private void ClearChoices()
    {
        for (int i = choicesRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(choicesRoot.GetChild(i).gameObject);
        }
    }

    private void ClearMessages()
    {
        for (int i = historyContent.childCount - 1; i >= 0; i--)
        {
            Destroy(historyContent.GetChild(i).gameObject);
        }
    }

    private void MoveToCamera()
    {
        GameObject camera = GameObject.FindWithTag("MainCamera");
        Vector3 position = camera.transform.position;
        position.z = DialogRoot.transform.position.z;
        DialogRoot.transform.position = position;
    }
}