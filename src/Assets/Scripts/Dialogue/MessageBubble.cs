using UnityEngine;
using UnityEngine.UI;

public class MessageBubble : MonoBehaviour
{
    [SerializeField] private Text messageText;
    [SerializeField] private LayoutGroup layout;
    [SerializeField] private Image background;

    [SerializeField] private Sprite[] backgroundSprites;

    private const int defaultPadding = 4;
    private const int extraPadding = 60;


    public void SetMessage(string text, Character character)
    {
        messageText.text = text;

        if (character == Character.Robin)
        {
            layout.childAlignment = TextAnchor.UpperRight;
            layout.padding.right = defaultPadding;
            layout.padding.left = defaultPadding + extraPadding;
        }
        else
        {
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.padding.right = defaultPadding + extraPadding;
            layout.padding.left = defaultPadding;
        }

        switch (character)
        {
            case Character.Robin:
                background.sprite = backgroundSprites[5];
                break;
            case Character.Mom:
                background.sprite = backgroundSprites[2];
                break;
            case Character.Francis:
                background.sprite = backgroundSprites[0];
                break;
            case Character.Lorenzo:
                background.sprite = backgroundSprites[1];
                break;
            case Character.Police:
                background.sprite = backgroundSprites[3];
                break;
            case Character.Sam:
                // what the frick?
                Debug.LogWarning(
                    "The message bubble thinks Sam is messaging you. Yes, the Dog is messaging you. You've done goofed");
                background.sprite = backgroundSprites[4];
                break;
            default:
                background.sprite = backgroundSprites[4];
                break;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }
}