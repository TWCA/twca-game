using UnityEngine.EventSystems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class InventoryItem : MonoBehaviour, IPointerDownHandler
{
    public int ItemCount = 1; // Starts at 1 by default
    public Text CountText;
    public Sprite NonSelectedItemBoxSprite;
    public Sprite SelectedItemBoxSprite;
    [NonSerialized] public GameObject PickupObjectPrefab;
    public Image SpriteImage;
    private InventorySystem inventorySystem;
    private InventoryCanvas inventoryCanvas;
    private SpriteRenderer pickupObjectSpriteRenderer;
    private Image backgroundImage;

    // Start is called before the first frame update
    void Start()
    {
        inventorySystem = InventorySystem.Instance;
        inventoryCanvas = InventoryCanvas.Instance;

        pickupObjectSpriteRenderer = PickupObjectPrefab.GetComponent<SpriteRenderer>();
        backgroundImage = GetComponent<Image>();

        SpriteImage.sprite = pickupObjectSpriteRenderer.sprite;
        SpriteImage.color = pickupObjectSpriteRenderer.color;

        inventoryCanvas.SelectedInventoryItemBoxChanged += SelectedBoxUpdated;

        CountText.enabled = PickupObjectPrefab.GetComponent<PickupObject>().AllowStacking;
    }

    void OnDestroy() {
        inventoryCanvas.SelectedInventoryItemBoxChanged -= SelectedBoxUpdated;
    }

    void SelectedBoxUpdated() {
        if (inventoryCanvas.GetSelectedInventoryBox() == this) {
            backgroundImage.sprite = SelectedItemBoxSprite;
        } else {
            backgroundImage.sprite = NonSelectedItemBoxSprite;
        }
    }

    /*
    * Updates the number on the actual UI element to reflect how many items are in a stack
    */
    void UpdateText() {
        if (ItemCount <= 0) {
            Destroy(gameObject);
        }

        CountText.text = $"{ItemCount}";
    }

    /*
    * Create an object when clicking on the inventory item.
    */
    public void OnPointerDown(PointerEventData eventData) {
        if (inventorySystem.HasMouseItem == false) {
            backgroundImage.sprite = SelectedItemBoxSprite;

            GameObject newObject = inventorySystem.CreatePickupObject(PickupObjectPrefab);

            if (newObject) {
                UpdateText();
                inventoryCanvas.SetSelectedInventoryItemBox(this);
            }
        }
    }

    /*
    * Sets internal values and text to a new item count
    */
    public void UpdateItemCount(int newCount) {
        ItemCount = newCount;

        UpdateText();
    }
}
