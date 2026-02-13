using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;

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
    //private bool isRunning;
    public GameObject DialogRoot;
    private System.Action onDialogFinished;
    public Behaviour[] disableWhileDialog;
    [SerializeField] public VAManager vaManager;


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(Instance.gameObject);
        else
            DontDestroyOnLoad(gameObject);

        Instance = this;
    }

    private void SetMovementEnabled(bool enabled)
    {
        if (disableWhileDialog == null) return;
        foreach (var b in disableWhileDialog)
        {
            if (b != null) b.enabled = enabled;
        }
    }

    public void StartDialog(string knot = "checkup_text", System.Action onFinished = null)
    {
        Debug.Log("StartDialog called!");
        if (inkJson == null)
        {
            Debug.LogError("DialogManager: inkJson is not assigned!");
            return;
        }
        vaManager.startScene();
        onDialogFinished = onFinished;
        story = new Story(inkJson.text);
        if (!string.IsNullOrEmpty(knot))
            story.ChoosePathString(knot);
        //isRunning = true;
        if (DialogRoot != null) DialogRoot.SetActive(true);
        onDialogFinished = onFinished;
        ClearChoices();
        ContinueStory();
        if (DialogRoot != null) DialogRoot.SetActive(true);
        SetMovementEnabled(false);
    }

    public void EndDialog()
    {
        //isRunning = false;
        story = null;
        ClearChoices();
        if (DialogRoot != null) DialogRoot.SetActive(false);
        var cb = onDialogFinished;
        onDialogFinished = null;
        cb?.Invoke();
        if (DialogRoot != null) DialogRoot.SetActive(false);
        SetMovementEnabled(true);
    }

    private void ContinueStory()
    {
        if (story == null) return;
        ClearChoices();
        while (story.canContinue)
        {
            string line = story.Continue().Trim();
            if (string.IsNullOrEmpty(line)) continue;
            bool isPlayer = false;
            List<string> tags = story.currentTags;

            // Check if there are at least two tags
            if (tags.Count >= 2)
            {
                string firstTag = tags[0];
                string secondTag = tags[1]; // This is your second tag
                Debug.Log("Second tag: " + secondTag);
                if (firstTag == "Robin")
                {
                    isPlayer = true;
                    vaManager.Enqueue(secondTag);
                    //break;
                }
                if (firstTag == "Friend")
                {
                    vaManager.Enqueue(secondTag);
                }

                //foreach (var tag in story.currentTags)
                //{
                //    if (tag == "Robin")
                //    {
                //        isPlayer = true;

                //       break;
                //   }
            }

                AddMessage(line, isPlayer);
        }

        RefreshChoices();
        Canvas.ForceUpdateCanvases();
        if (historyScrollRect != null)
            historyScrollRect.verticalNormalizedPosition = 0f;
    }

    private void RefreshChoices()
    {
        ClearChoices();
        List<Choice> choices = story.currentChoices;
        int count = Mathf.Min(4, choices.Count);
        for (int i = 0; i < count; i++)
        {
            Choice choice = choices[i];
            int choiceIndex = i;
            GameObject btnObj = Instantiate(choiceButtonPrefab, choicesRoot);
            Button btn = btnObj.GetComponent<Button>();
            Text label = btnObj.GetComponentInChildren<Text>();
            if (label != null) label.text = choice.text;
            btn.onClick.AddListener(() =>
            {
                story.ChooseChoiceIndex(choice.index);
                ContinueStory();
            });
        }

        if (choices.Count == 0 && !story.canContinue)
        {
            EndDialog();
        }
    }


    private void AddMessage(string text, bool isPlayer)
    {
        GameObject obj = Instantiate(messageBubblePrefab, historyContent);
        var bubble = obj.GetComponent<MessageBubble>();
        if (bubble != null)
            bubble.SetMessage(text, isPlayer);
        else
            Debug.LogWarning("Message bubble prefab missing MessageBubble script.");
    }

    private void ClearChoices()
    {
        for (int i = choicesRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(choicesRoot.GetChild(i).gameObject);
        }
    }
}