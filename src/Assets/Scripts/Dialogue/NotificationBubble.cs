using UnityEngine;
using UnityEngine.UI;

public class NotificationBubble : MonoBehaviour
{
    [SerializeField] private Text titleText;
    [SerializeField] private Text bodyText;

    public void SetMessage(string title, string body)
    {
        titleText.text = title;
        bodyText.text = body;
        
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }
}