using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryScript : MonoBehaviour
{
    public int ItemMax = 5;
    public int Padding = 1;
    public GameObject templateItem;
    // public GameObject[] items;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Transform GetExistingItem(GameObject prefab) {
        foreach (Transform child in transform.GetChild(0)) {
            InventoryItem childInventoryItem = child.GetComponent<InventoryItem>();
            GameObject childPrefab = childInventoryItem.PickupObjectPrefab;

            if (childPrefab == prefab) {
                return child;
            }
        }

        return null;
    }

    public void AddItem(string itemName) {
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/Objects/{itemName}");
        Transform existingItem = GetExistingItem(prefab);

        if (existingItem != null) {
            InventoryItem existingInventoryItem = existingItem.GetComponent<InventoryItem>();
            existingInventoryItem.UpdateItemCount(existingInventoryItem.GetItemCount() + 1);
        } else {
            Transform newItem = Instantiate(templateItem.transform, templateItem.transform.position, Quaternion.identity);
            UnityEditor.GameObjectUtility.SetParentAndAlign(newItem.gameObject, transform.GetChild(0).gameObject);

            InventoryItem newInventoryItem = newItem.GetComponent<InventoryItem>();

            newInventoryItem.PickupObjectPrefab = prefab;
        }
    }
}
