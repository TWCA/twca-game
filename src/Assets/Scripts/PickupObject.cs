using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    public bool BeingCarried = false;
    private GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("InventoryCanvas");
    }

    // Update is called once per frame
    void Update()
    {
        if (BeingCarried) {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 finalPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            finalPosition.z = 0;

            transform.position = finalPosition;
        }
    }

    void OnMouseDown() {
        if (BeingCarried) {
            BeingCarried = false;
        } else if (canvas != null) {
            InventoryScript inventory = canvas.GetComponent<InventoryScript>();
            inventory.AddItem(name);

            Destroy(gameObject);
        }
    }
}
