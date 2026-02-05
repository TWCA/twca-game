using System.Collections.Generic;
using UnityEngine;

/**
 * A singleton that handles core player inventory interactions.
 * Keeps track of all items in the players inventory
**/
public class InventorySystem : MonoBehaviour
{
    public int ItemMax = 5;
    public int Padding = 1;
    public GameObject TemplateItem;
    private Transform inventoryUIObject;
    public static InventorySystem Instance { get; private set; }

    private List<Item> items;

    // Start is called before the first frame update
    InventorySystem()
    {
        items = new List<Item>();
    }

    private void Awake()
    {
        inventoryUIObject = transform.GetChild(0);
        Instance = this;
    }

    /*
    * Checks if an item is already in the inventory (for stacking)
    * Filters out and removes items that no longer have ui objects
    */
    private Item GetExistingItem(string otherItemName) {
        Item item = items.Find(item => item.name == otherItemName);

        if (item == null) {
            return null;
        } else if (item.uiObject != null) {
            return item;
        } else {
            items.Remove(item);
            return null;
        }
    }

    /*
    * Adds an item to the inventory (keeps track by name of item)
    */
    public void AddItem(string itemName) {
        if (inventoryUIObject.childCount >= ItemMax) {
            Debug.Log($"Inventory reached max size of {ItemMax}!");
        }

        GameObject prefab = Resources.Load<GameObject>($"Prefabs/Objects/{itemName}");
        Item existingItem = GetExistingItem(itemName);

        if (existingItem != null) {
            InventoryItem existingInventoryItem = existingItem.uiObject.GetComponent<InventoryItem>();
            existingInventoryItem.UpdateItemCount(existingInventoryItem.GetItemCount() + 1);
        } else {
            Item newItem = new Item(prefab, itemName);
            items.Add(newItem);
        }
    }

    private class Item
    {
        public string name;
        public Transform uiObject;

        public Item(GameObject prefab, string itemName) {
            InventorySystem inventorySystem = InventorySystem.Instance;

            Transform newItemUIObject = Instantiate(inventorySystem.TemplateItem.transform, inventorySystem.TemplateItem.transform.position, Quaternion.identity);
            UnityEditor.GameObjectUtility.SetParentAndAlign(newItemUIObject.gameObject, inventorySystem.inventoryUIObject.gameObject);

            InventoryItem newInventoryItem = newItemUIObject.GetComponent<InventoryItem>();

            newInventoryItem.PickupObjectPrefab = prefab;

            this.name = itemName;
            this.uiObject = newItemUIObject;
        }
    }
}
