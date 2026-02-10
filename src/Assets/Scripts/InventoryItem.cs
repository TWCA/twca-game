using UnityEngine.EventSystems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerDownHandler
{
    public int ItemCount = 1; // Starts at 1 by default
    public GameObject CountText;
    public GameObject PickupObjectPrefab;
    private InventorySystem inventorySystem;
    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        inventorySystem = InventorySystem.Instance;

        image = GetComponent<Image>();
        image.sprite = PickupObjectPrefab.GetComponent<SpriteRenderer>().sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    * Updates the number on the actual UI element to reflect how many items are in a stack
    */
    void UpdateText() {
        if (ItemCount <= 0) {
            Destroy(gameObject);
        }

        if (CountText != null) {
            TextMeshProUGUI countTextMesh = CountText.GetComponent<TextMeshProUGUI>();

            countTextMesh.text = $"{ItemCount}";
        }
    }

    /*
    * Create an object when clicking on the inventory item.
    */
    public void OnPointerDown(PointerEventData eventData) {
        GameObject newObject = inventorySystem.CreatePickupObject(PickupObjectPrefab);

        if (newObject) {
            ItemCount--;
            UpdateText();
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
