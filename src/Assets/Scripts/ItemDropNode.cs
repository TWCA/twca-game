using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemDropNode : MonoBehaviour
{
    public AllowDenyList AllowDeny;
    public GameObject ActiveItem;
    private CircleCollider2D circleCollider;
    private InventorySystem inventorySystem;
    private bool mouseIn;
    private InputAction clickAction;
    void OnDrawGizmos()
    {
        circleCollider = GetComponent<CircleCollider2D>();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, circleCollider.radius);
    }

    // Start is called before the first frame update
    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        clickAction = InputSystem.actions.FindAction("Click");

        inventorySystem = InventorySystem.Instance;
        transform.localScale = new Vector3(circleCollider.radius, circleCollider.radius, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (clickAction.WasReleasedThisFrame() && mouseIn) {
            ItemIncoming();
        }
    }

    void ItemIncoming() {
        Debug.Log("DROP");
        // if (inventorySystem.HeldItem != null) {
        //     if (ActiveItem != null) {
        //         // do something
        //     } else {
        //         ActiveItem = inventorySystem.HeldItem;
        //         inventorySystem.HeldItem = null;
        //     }
        // }
    }

    void OnMouseDown() {
        inventorySystem.CreatePickupObject(ActiveItem);
        ActiveItem = null;
    }
}
