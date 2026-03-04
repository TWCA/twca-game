using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }
    public TextAsset inkJson;
    public Transform historyContent;
    public ScrollRect historyScrollRect;
    public GameObject messageBubblePrefab;
    public Transform choicesRoot;
    public GameObject choiceButtonPrefab;
    private Story story;
    private bool isRunning = false;
    public GameObject DialogRoot;
    private System.Action onDialogFinished;
    public Behaviour[] disableWhileDialog;


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

    private void UpdateDisabledBehaviours()
    {
        foreach (var behaviour in disableWhileDialog)
        {
            if (behaviour != null)
                behaviour.enabled = !isRunning;
        }
    }

    public void StartDialog(string knot, System.Action onFinished = null)
    {
        if (inkJson == null)
        {
            Debug.LogError("DialogManager: inkJson is not assigned!");
            return;
        }

        if (!string.IsNullOrEmpty(knot))
            story.ChoosePathString(knot);
        else
            Debug.LogError("No knot location passed");

        isRunning = true;
        DialogRoot.SetActive(true);

        onDialogFinished = onFinished;

        UpdateDisabledBehaviours();

        if (story.canContinue)
        {
            ContinueStory();
        }
        else
        {
            Debug.LogError("Knot doesn't have any content");
            EndDialog();
        }
    }

    public void EndDialog()
    {
        isRunning = false;
        DialogRoot.SetActive(false);

        onDialogFinished?.Invoke();
        onDialogFinished = null;

        UpdateDisabledBehaviours();
    }

    private void ContinueStory()
    {
        string line = story.Continue().Trim();
        List<string> tags = story.currentTags;

        bool isPlayer = false;

        if (tags.Contains("Robin"))
        {
            tags.RemoveAll((string s) => s == "Robin");
            isPlayer = true;
        }
        else if (tags.Contains("Friend"))
        {
            tags.RemoveAll((string s) => s == "Friend");
            // isPlayer = false;
        }
        else
        {
            Debug.LogWarning("Dialog line was not tagged with #Robin or #Friend, assuming line is from friend");
        }

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
            else
            {
                Debug.LogWarning("Dialog line contains unknown tag: " + tag);
            }
        }

        AddMessage(line, isPlayer);

        ClearChoices();
        VAManager.Instance.OnQueueEmpty(() =>
        {
            if (story.canContinue)
                ContinueStory();
            else if (story.currentChoices.Count > 0)
                RefreshChoices();
            else
                EndDialog();
        });
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
        var bubble = obj.GetComponent<MessageBubble>();
        if (bubble != null)
            bubble.SetMessage(text, isPlayer);
        else
            Debug.LogWarning("Message bubble prefab missing MessageBubble script.");

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
}