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

        playerDetector.PlayerTouched += InteractedWith;
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
                inventorySystem.CarriedItem = prefab;
                inventorySystem.MouseItem = null;
            }

            return true;
        } else {
            Debug.Log("No, you cannot put that item there.");

            return false;
        }
    }

    public void InteractedWith() {
        Debug.Log("B");
        if (ActiveItem != null) {
            Debug.Log("Active item");
            inventorySystem.AddItem(ActiveItem);
            ActiveItem = null;
        } else if (inventorySystem.CarriedItem) {
            Debug.Log("No active item");
            ActiveItem = inventorySystem.CarriedItem;
            inventorySystem.CarriedItem = null;
        }

        InitializeSprite();
    }

    void OnMouseEnter() {
        materialRenderer.material = SelectedMaterial;
    }

    void OnMouseExit()
    {
        materialRenderer.material = originalMaterial;
    }
}
