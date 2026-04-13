using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationBubble : MonoBehaviour
{
    [SerializeField] private List<Sprite> icons = new List<Sprite>();

    [SerializeField] private Text titleText;
    [SerializeField] private Text bodyText;
    [SerializeField] private Image iconImage;

    public void SetMessage(string appTitle, string body)
    {
        if (appTitle == "Readit")
        {
            iconImage.sprite = icons[0];
        }
        else if (appTitle == "Instancegram")
        {
            iconImage.sprite = icons[1];
        }
        else if (appTitle == "News")
        {
            iconImage.sprite = icons[2];
        }
        else
        {
            Debug.LogWarning("Can't find app icon: " + appTitle);
        }


        titleText.text = appTitle;
        bodyText.text = body;

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }
}