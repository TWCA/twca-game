using System;
using System.Collections;
using UnityEngine;

public class ItemDropNode : MonoBehaviour
{
    public AllowDenyList AllowDeny;
    public GameObject ActiveItem;
    public Material SelectedMaterial;
    public SpriteRenderer SpriteRenderer;
    public SpriteRenderer HoverCircle;
    public PlayerDetector InteractPlayerDetector;
    public PlayerDetector NotifyPlayerDetector;

    // You may leave these two fields blank and it will just use the sprite the item has
    // These fields are good for something like the dog food bowl where it goes from empty to full
    public Sprite EmptySpriteOverride; // What does the item drop node look like when there is no item in it
    public Sprite ActiveSpriteOverride; // What does the item drop nodel look like when there is an item in it

    public bool SingleUse; // Can this drop node only be used once?

    private CircleCollider2D circleCollider;
    private InventorySystem inventorySystem;
    private Renderer materialRenderer;
    private bool used; // Used for keeping track of if the drop node was used if SingleUse is true

    // Events for level code (like the river system) to interact with
    public event Action ItemPlaced;
    public event Action ItemRemoved;

    public float BaseAlpha = 0f;
    public float NotifyAlpha = 0.2f;
    public float HoverAlpha = 0.8f;

    /*
    * Runs some logic that sets up the ItemDropNode
    */
    private void Initialize() {
        circleCollider = GetComponent<CircleCollider2D>();
        materialRenderer = SpriteRenderer.GetComponent<Renderer>();
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
        SetAlpha(BaseAlpha);

        inventorySystem = InventorySystem.Instance;
    }

    void Awake() {
        Initialize();
        InitializeSprite();

        InteractPlayerDetector.PlayerTouched += InteractedWith;
        NotifyPlayerDetector.PlayerTouched += NearbyNotifyEntered;
        NotifyPlayerDetector.PlayerLeft += NearbyNotifyExited;

        materialRenderer.material = SelectedMaterial;
    }

    void OnDisable() {
        InteractPlayerDetector.PlayerTouched -= InteractedWith;
        NotifyPlayerDetector.PlayerTouched -= NearbyNotifyEntered;
        NotifyPlayerDetector.PlayerLeft -= NearbyNotifyExited;
    }

    // Update is called once per frame
    void Update()
    {
        // Handle when the player is still in the collider and picks up the item
        // (otherwise InteractedWith() wouldn't be called since it only is called once when the player enters the collider)
        if (InteractPlayerDetector.TouchingPlayer && (inventorySystem.CarriedItem || inventorySystem.TargetDropNode == this)) {
            InteractedWith();
        }

        HoverCircle.gameObject.SetActive(InventorySystem.Instance.HasMouseItem);
    }

    public void InitializeSprite() {
        if (ActiveItem != null) {
            SpriteRenderer activeItemSpriteRenderer = ActiveItem.GetComponent<SpriteRenderer>();

            if (ActiveSpriteOverride != null) {
                SpriteRenderer.sprite = ActiveSpriteOverride;
            } else {
                SpriteRenderer.sprite = activeItemSpriteRenderer.sprite;
                SpriteRenderer.color = activeItemSpriteRenderer.color;
            }
        } else {
            if (EmptySpriteOverride != null) {
                SpriteRenderer.sprite = EmptySpriteOverride;
            } else {
                SpriteRenderer.sprite = null;
            }
        }
    }

    /*
    * Handles when an item is dragged and dropped over a node
    */
    public bool ItemIncoming(GameObject prefab) {
        // Do we even allow this item in this node?
        if (AllowDeny.IsItemAllowed(prefab.name) && !(SingleUse && used)) {
            if (ActiveItem != null) {
                // Call some abitrary function that runs when one item is dragged onto the other
                // ActiveItem.GetComponent<PickupObject>().DraggedOnto(prefab);

                // Disabled item mixing for vertical slice
                // Its producing some issues that will be tackled for beta
                return false;
            } else {
                inventorySystem.CarriedItem = prefab;
                inventorySystem.HasMouseItem = false;
            }

            if (SingleUse) {
                used = true;
            }

            MarkUsed();

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
    public void InteractedWith() {
        PlayerControl player = PlayerControl.Instance;

        if (inventorySystem.TargetDropNode == this) {
            if (ActiveItem != null) {
                StartCoroutine(TriggerInteractAnimation(() =>
                    {
                        inventorySystem.AddItem(ActiveItem);
                        ClearActiveItem();
                        InitializeSprite();
                    }));

                MarkUsed();
            } else if (inventorySystem.CarriedItem) {
                SetActiveItem(inventorySystem.CarriedItem);

                StartCoroutine(TriggerInteractAnimation(() =>
                    {
                        inventorySystem.RemoveItem(ActiveItem);
                        
                        InitializeSprite();
                    }));

                MarkUsed();
            }

            if (SingleUse) {
                used = true;
            }

            player.StopInPlace();
            inventorySystem.Cancel();
        }
    }

    /*
     * Makes the player run the interact animation
     */
    public IEnumerator TriggerInteractAnimation(Action callback)
    {
        Animator animator = GameObject.FindWithTag("Player").GetComponent<Animator>();
        
        animator.SetBool("interacting", true);
        
        yield return new WaitForSeconds(0.8f);
        
        callback();
        animator.SetBool("interacting", false);
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

    /*
    * Sets the transparency of the outline material
    */
    private void SetAlpha(float alpha) {
        materialRenderer.material.SetColor("_Color", new Color(1f, 1f, 1f, alpha));
    }

    /*
    * Gets the alpha/transparency of the outline material
    */
    private float GetAlpha()
    {
        return materialRenderer.material.GetColor("_Color").a;
    }
    
    /*
    * Marks this item as used if it is single use
    */
    private void MarkUsed() {
        if (SingleUse) {
            used = true;
        }
    }

    void OnMouseEnter() {
        if (ActiveItem != null) {
            materialRenderer.material = SelectedMaterial;
            SetAlpha(HoverAlpha);
        }
    }

    void OnMouseExit()
    {
        if (NotifyPlayerDetector.TouchingPlayer) {
            SetAlpha(NotifyAlpha);
        } else {
            SetAlpha(BaseAlpha);
        }
    }

    void OnMouseUp() {
        if (inventorySystem.TargetDropNode == null && !(SingleUse && used)) {
            inventorySystem.TargetDropNode = this;
        }
    }

    /*
    * Occurs when the player is close enough to the trigger that causes the hint outline to show
    */
    void NearbyNotifyEntered() {
        if (GetAlpha() != HoverAlpha) {
            SetAlpha(NotifyAlpha);
        }
    }

    /*
    * Occurs when the player is far enough from the trigger that causes the hint outline to hide/reset
    */
    void NearbyNotifyExited() {
        if (GetAlpha() != HoverAlpha) {
            SetAlpha(BaseAlpha);
        }
    }
}
