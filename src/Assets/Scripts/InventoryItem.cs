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
    private SpriteRenderer pickupObjectSpriteRenderer;
    private Image backgroundImage;
    // private Outline selectedOutline;

    // Start is called before the first frame update
    void Start()
    {
        inventorySystem = InventorySystem.Instance;

        pickupObjectSpriteRenderer = PickupObjectPrefab.GetComponent<SpriteRenderer>();
        backgroundImage = GetComponent<Image>();

        SpriteImage.sprite = pickupObjectSpriteRenderer.sprite;
        SpriteImage.color = pickupObjectSpriteRenderer.color;

        inventorySystem.SelectedInventoryItemBoxChanged += SelectedBoxUpdated;
    }

    void OnDestroy() {
        inventorySystem.SelectedInventoryItemBoxChanged -= SelectedBoxUpdated;
    }

    void SelectedBoxUpdated() {
        if (inventorySystem.GetSelectedInventoryBox() == this) {
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
        backgroundImage.sprite = SelectedItemBoxSprite;

        GameObject newObject = inventorySystem.CreatePickupObject(PickupObjectPrefab);

        if (newObject) {
            UpdateText();
            inventorySystem.SetSelectedInventoryItemBox(this);
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
