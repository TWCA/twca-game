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
    public GameObject MouseItem; // The item that appears where the mouse is
    public GameObject CarriedItem; // The item that the character is bringing to the node
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
    * returns true if it was a success (if stacking is prevented or not)
    */
    public bool AddItem(GameObject prefab) {
        if (inventoryUIObject.childCount >= ItemMax) {
            Debug.Log($"Inventory reached max size of {ItemMax}!");
        }

        Item existingItem = GetExistingItem(prefab.name);

        if (existingItem != null) {
            PickupObject pickupObject = prefab.GetComponent<PickupObject>();

            // Don't update the item count or actually pick up the item if we don't allow stacking
            if (pickupObject.AllowStacking == false)
            {
                return false;
            }

            // Update the count on the UI object
            InventoryItem existingInventoryItem = existingItem.uiObject.GetComponent<InventoryItem>();
            existingInventoryItem.UpdateItemCount(existingInventoryItem.ItemCount + 1);
        } else {
            Item newItem = new Item(prefab);
            items.Add(newItem);
        }

        return true;
    }

    /*
    * Creates a pickupobject (an object that you move around with your mouse from place to place)
    */
    public GameObject CreatePickupObject(GameObject prefab) {
        Transform newObject = Instantiate(prefab.transform, prefab.transform.position, Quaternion.identity);

        if (newObject) {
            newObject.name = prefab.name;
            newObject.GetComponent<PickupObject>().PickupObjectPrefab = prefab;

            MouseItem = newObject.gameObject;

            return newObject.gameObject;
        }

        return null;
    }

    /*
    * Object that represents an Item in the inventory and its UI elements
    * Used for properly insantiating items in the scene
    */
    private class Item
    {
        public string name;
        public Transform uiObject;

        public Item(GameObject prefab) {
            InventorySystem inventorySystem = Instance;

            // Create UI object and set it to proper position
            Transform newItemUIObject = Instantiate(inventorySystem.TemplateItem.transform, inventorySystem.TemplateItem.transform.position, Quaternion.identity);
            newItemUIObject.gameObject.transform.SetParent(inventorySystem.inventoryUIObject.gameObject.transform, false);

            InventoryItem newInventoryItem = newItemUIObject.GetComponent<InventoryItem>();

            newInventoryItem.PickupObjectPrefab = prefab;

            this.name = prefab.name;
            this.uiObject = newItemUIObject;
        }
    }
}
