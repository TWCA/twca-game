using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    public int ItemCount = 1;
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

    void UpdateText() {
        if (ItemCount <= 0) {
            Destroy(gameObject);
        }

        if (CountText != null) {
            TextMeshProUGUI countTextMesh = CountText.GetComponent<TextMeshProUGUI>();

            countTextMesh.text = $"{ItemCount}";
        }
    }

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

    public void UpdateItemCount(int newCount) {
        ItemCount = newCount;

        UpdateText();
    }

    public int GetItemCount() {
        return ItemCount;
    }
}
