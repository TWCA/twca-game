using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationBubble : MonoBehaviour
{
    [SerializeField] private List<Sprite> icons = new List<Sprite>();

    [SerializeField] private Text titleText;
    [SerializeField] private Text bodyText;
    [SerializeField] private Image iconImage;

    public void SetMessage(string title, string body)
    {
        if (title == "Readit")
        {
            iconImage.sprite = icons[0];
        }
        else if (title == "Instancegram")
        {
            iconImage.sprite = icons[1];
        }
        else if (title == "News")
        {
            iconImage.sprite = icons[2];
        }
        else
        {
            Debug.LogWarning("Can't find app icon: " + title);
        }


        titleText.text = title;
        bodyText.text = body;

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }
}