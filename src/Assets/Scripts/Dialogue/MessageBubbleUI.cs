using UnityEngine;
using UnityEngine.UI;

public class MessageBubbleUI : MonoBehaviour
{
    public Text messageText;
    public Image avatarImage;
    public HorizontalLayoutGroup rowLayout;

    public Sprite playerAvatar;
    public Sprite npcAvatar;
    [Header("Avatar Sizes")]
    public Vector2 playerAvatarSize = new Vector2(96, 96);
    public Vector2 npcAvatarSize = new Vector2(56, 56);
    public void SetMessage(string text, bool isPlayer)
    {
        if (messageText != null) messageText.text = text;

        if (avatarImage != null)
        {
            avatarImage.sprite = isPlayer ? playerAvatar : npcAvatar;
            RectTransform art = avatarImage.rectTransform;
            art.sizeDelta = isPlayer ? playerAvatarSize : npcAvatarSize;
            var le = avatarImage.GetComponent<LayoutElement>();
            if (le == null) le = avatarImage.gameObject.AddComponent<LayoutElement>();
            le.preferredWidth  = art.sizeDelta.x;
            le.preferredHeight = art.sizeDelta.y;
            le.flexibleWidth = 0;
            le.flexibleHeight = 0;
        }
        if (rowLayout != null)
            rowLayout.childAlignment = isPlayer ? TextAnchor.UpperRight : TextAnchor.UpperLeft;
        if (avatarImage != null)
        {
            if (isPlayer)
                avatarImage.transform.SetSiblingIndex(transform.childCount - 1);
            else
                avatarImage.transform.SetSiblingIndex(0);
        }
        int outerPadding = 80;
        int innerPadding = 10;
        if (!isPlayer)
        {
            rowLayout.padding.left = 20;
            rowLayout.padding.right = outerPadding;
        }
        else
        {
            rowLayout.padding.left = outerPadding;
            rowLayout.padding.right = 20;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }
}
