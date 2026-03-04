using UnityEngine;
using UnityEngine.UI;

public class MessageBubble : MonoBehaviour
{
    [SerializeField] private Text messageText;
    [SerializeField] private LayoutGroup layout;
    [SerializeField] private Image background;
    
    [SerializeField] private Sprite playerBackground;
    [SerializeField] private Sprite friendBackground;
    
    private const int defaultPadding = 4;
    private const int extraPadding = 60;
        

    public void SetMessage(string text, bool isPlayer)
    {
        messageText.text = text;
        layout.childAlignment = isPlayer ? TextAnchor.UpperRight : TextAnchor.UpperLeft;

        if (isPlayer)
        {
            layout.padding.right = defaultPadding;
            layout.padding.left = defaultPadding + extraPadding;
            background.sprite = playerBackground;
        }
        else
        {
            layout.padding.right = defaultPadding + extraPadding;
            layout.padding.left = defaultPadding;
            background.sprite = friendBackground;
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }
}
