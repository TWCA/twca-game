using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCanvas : MonoBehaviour
{
    public static InventoryCanvas Instance { get; private set; }
    private ItemNameText itemNameText;
    private HorizontalLayoutGroup itemLayout;
    private InventoryItem selectedInventoryItemBox; // The InventoryItem UI element that is selected
    public event Action SelectedInventoryItemBoxChanged;

    InventoryCanvas() {}

    // Start is called before the first frame update
    void Awake()
    {
        itemNameText = GetComponentInChildren<ItemNameText>();
        itemLayout = GetComponentInChildren<HorizontalLayoutGroup>();

        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    * Adds a new object to the inventory bar
    */
    public void AddUIObject(Transform uiObject, GameObject prefab) {
        uiObject.SetParent(itemLayout.transform, false);

        InventoryItem newInventoryItem = uiObject.GetComponent<InventoryItem>();
        newInventoryItem.PickupObjectPrefab = prefab;

        itemNameText.SetText(uiObject.name);
    }

    /*
    * Shows the item as selected in the inventory bar
    */
    public void SetSelectedInventoryItemBox(InventoryItem inventoryItem) {
        selectedInventoryItemBox = inventoryItem;

        if (inventoryItem != null) {
            itemNameText.SetText(inventoryItem.name);
        }

        SelectedInventoryItemBoxChanged.Invoke();
    }

    public InventoryItem GetSelectedInventoryBox() {
        return selectedInventoryItemBox;
    }
}
