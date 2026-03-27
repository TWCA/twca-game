using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCanvas : MonoBehaviour
{
    private ItemNameText itemNameText;

    // Start is called before the first frame update
    void Start()
    {
        itemNameText = GetComponentInChildren<ItemNameText>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddUIObject(Transform UIObject) {
        UIObject.SetParent(transform, false);
    }
}
