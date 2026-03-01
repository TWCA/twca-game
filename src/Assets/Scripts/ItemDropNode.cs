using System;
using UnityEngine;

public class ItemDropNode : MonoBehaviour
{
    public AllowDenyList AllowDeny;
    public GameObject ActiveItem;
    public Material SelectedMaterial;
    private CircleCollider2D circleCollider;
    private InventorySystem inventorySystem;
    private SpriteRenderer spriteRenderer;
    private PlayerDetector playerDetector;
    private Material originalMaterial;
    private Renderer materialRenderer;

    // Events for level code (like the river system) to interact with
    public event Action ItemPlaced;
    public event Action ItemRemoved;

    /*
    * Runs some logic that sets up the ItemDropNode
    */
    private void Initialize() {
        circleCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerDetector = GetComponentInChildren<PlayerDetector>();
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
        InitializeSprite();

        inventorySystem = InventorySystem.Instance;
        materialRenderer = GetComponent<Renderer>();
        originalMaterial = materialRenderer.material;

        playerDetector.PlayerTouched += () => {
            InteractedWith(playerDetector.TouchingPlayer);
        };
    }

    // Update is called once per frame
    void Update()
    {
        // Handle when the player is still in the collider and picks up the item
        // (otherwise InteractedWith() wouldn't be called since it only is called once when the player enters the collider)
        if (playerDetector.TouchingPlayer != null && inventorySystem.CarriedItem) {
            InteractedWith(playerDetector.TouchingPlayer);
        }
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

    /*
    * Handles when an item is dragged and dropped over a node
    */
    public bool ItemIncoming(GameObject prefab) {
        // Do we even allow this item in this node?
        if (AllowDeny.IsItemAllowed(prefab.name)) {
            if (ActiveItem != null) {
                // Call some abitrary function that runs when one item is dragged onto the other
                ActiveItem.GetComponent<PickupObject>().DraggedOnto(prefab);
            } else {
                inventorySystem.CarriedItem = prefab;
                inventorySystem.MouseItem = null;
            }

            inventorySystem.TargetDropNode = this;

            return true;
        } else {
            Debug.Log("No, you cannot put that item there.");

            return false;
        }
    }

    /*
    * Handles when the player enters the region where they can affect the item
    */
    public void InteractedWith(PlayerControl player) {
        if (ActiveItem != null) {
            inventorySystem.AddItem(ActiveItem);
            ClearActiveItem();

            player.StopInPlace();
        } else if (inventorySystem.CarriedItem && inventorySystem.TargetDropNode == this) {
            SetActiveItem(inventorySystem.CarriedItem);

            inventorySystem.CarriedItem = null;
            inventorySystem.RemoveItem(ActiveItem);

            player.StopInPlace();
        }

        InitializeSprite();
    }

    /*
    * Sets the active item
    */
    private void SetActiveItem(GameObject item) {
        ActiveItem = item;
        ItemPlaced?.Invoke();
    }

    /*
    * Clears the active item
    */
    private void ClearActiveItem() {
        ActiveItem = null;
        ItemRemoved?.Invoke();
    }

    void OnMouseEnter() {
        materialRenderer.material = SelectedMaterial;
    }

    void OnMouseExit()
    {
        materialRenderer.material = originalMaterial;
    }
}
