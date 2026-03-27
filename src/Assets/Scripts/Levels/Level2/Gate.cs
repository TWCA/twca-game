using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] private ItemDropNode gateDropNode;
    [SerializeField] private SpriteRenderer gateSpriteRenderer;
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;
    [SerializeField] private string requiredItemName = "Key";
    [SerializeField] private string gatePathName = "Level2Gate";
    [SerializeField] private bool unlockPast = true;
    [SerializeField] private bool unlockFuture = true;

    private bool unlocked = false;

    private void Start()
    {
        if (gateDropNode == null)
        {
            Debug.LogError($"{name}: gateDropNode is not assigned.");
            return;
        }

        gateDropNode.ItemPlaced += OnItemPlaced;

        if (gateSpriteRenderer != null && closedSprite != null)
        {
            gateSpriteRenderer.sprite = closedSprite;
        }
    }

    private void OnDestroy()
    {
        if (gateDropNode != null)
        {
            gateDropNode.ItemPlaced -= OnItemPlaced;
        }
    }

    private void OnItemPlaced()
    {
        if (unlocked)
            return;

        if (gateDropNode.ActiveItem == null)
            return;

        string placedItemName = gateDropNode.ActiveItem.name;

        if (placedItemName != requiredItemName)
            return;

        int gatePath = PathNetwork.Instance.GetNamedPath(gatePathName);
        if (gatePath == -1)
        {
            Debug.LogWarning($"{name}: Could not find path named '{gatePathName}'.");
            return;
        }

        if (unlockPast)
        {
            PathNetwork.Instance.SetPathPastTraversable(gatePath, true);
        }

        if (unlockFuture)
        {
            PathNetwork.Instance.SetPathFutureTraversable(gatePath, true);
        }

        unlocked = true;

        if (gateSpriteRenderer != null && openSprite != null)
        {
            gateSpriteRenderer.sprite = openSprite;
        }

        Debug.Log($"{name}: Gate unlocked with item '{placedItemName}'.");
    }
}