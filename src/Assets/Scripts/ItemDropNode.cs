using UnityEngine;

public class ItemDropNode : MonoBehaviour
{
    public AllowDenyList AllowDeny;
    public GameObject ActiveItem;
    private CircleCollider2D circleCollider;
    private InventorySystem inventorySystem;
    private SpriteRenderer spriteRenderer;
    private PlayerDetector playerDetector;

    /*
    * Runs some logic that sets up the ItemDropNode
    */
    private void Initialize() {
        circleCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerDetector = GetComponentInChildren<PlayerDetector>();

        playerDetector.PlayerTouched += InteractedWith;

        InitializeSprite();
    }

    /*
    * Used to draw some editor effects for easier use
    */
    void OnDrawGizmos()
    {
        Initialize();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, circleCollider.radius * 2);
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();

        inventorySystem = InventorySystem.Instance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void InitializeSprite() {
        if (ActiveItem != null) {
            SpriteRenderer activeItemSpriteRenderer = ActiveItem.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = activeItemSpriteRenderer.sprite;
            spriteRenderer.color = activeItemSpriteRenderer.color;
        } else {
            spriteRenderer.sprite = null;
        }
    }

    public bool ItemIncoming(GameObject prefab) {
        // Do we even allow this item in this node?
        if (AllowDeny.IsItemAllowed(prefab.name)) {
            if (ActiveItem != null) {
                // Call some abitrary function that runs when one item is dragged onto the other
                ActiveItem.GetComponent<PickupObject>().DraggedOnto(prefab);
            } else {
                ActiveItem = prefab;
                inventorySystem.HeldItem = null;

                InitializeSprite();
            }

            return true;
        } else {
            Debug.Log("No, you cannot put that item there.");

            return false;
        }
    }

    void InteractedWith() {
        if (ActiveItem != null) {
            // inventorySystem.CreatePickupObject(ActiveItem);
            inventorySystem.AddItem(ActiveItem);
            ActiveItem = null;

            InitializeSprite();
        } else {
            Debug.Log("No active item in this node");
        }
    }
}
