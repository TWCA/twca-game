using UnityEngine.EventSystems;
using TMPro;
using UnityEngine;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    public int ItemCount = 1; // Starts at 1 by default
    public GameObject CountText;
    public GameObject PickupObjectPrefab;

    // Start is called before the first frame update
    void Start()
    {
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
    public void OnPointerClick(PointerEventData eventData) {
        Transform newObject = Instantiate(PickupObjectPrefab.transform, PickupObjectPrefab.transform.position, Quaternion.identity);

        if (newObject) {
            newObject.name = PickupObjectPrefab.name;

            PickupObject pickupObject = newObject.GetComponent<PickupObject>();
            pickupObject.BeingCarried = true;

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
