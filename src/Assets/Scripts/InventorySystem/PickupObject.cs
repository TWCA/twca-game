using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickupObject : MonoBehaviour
{
    public bool AllowStacking = true; // Can the object be stacked? No support for stack size maximums for now, not sure if needed.
    public string NiceName; // The name of the item that faces the user
    public Sprite AlternateGroundSprite;
    
    [NonSerialized] public GameObject PickupObjectPrefab;
    private InventorySystem inventorySystem;
    private InputAction clickAction;
    private PlayerControl player;

    // Start is called before the first frame update
    void Start()
    {
        inventorySystem = InventorySystem.Instance;
        player = PlayerControl.Instance;

        clickAction = InputSystem.actions.FindAction("Click");

        SetToMousePosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (clickAction.WasPressedThisFrame()) {
            ItemRelease();
        }

        SetToMousePosition();
    }

    /*
    * Decide what to do with an item when it is released by the player based on the current context
    */
    void ItemRelease() {
        ItemDropNode itemDropNode = RaycastManager.IsComponentBelowMouse<ItemDropNode>();
        if (itemDropNode != null && inventorySystem.CarriedItem == null) {
            bool accepted = itemDropNode.ItemIncoming(PickupObjectPrefab);

            if (!accepted) {
                inventorySystem.AddItem(PickupObjectPrefab);
            } else {
                player.PathfindTo(itemDropNode.gameObject.transform.position);
            }
        }

        inventorySystem.DeletePickupObject(this);
    }

    /*
    * Moves the object to the mouse current position in world space
    */
    private void SetToMousePosition() {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 finalPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        finalPosition.z = 0;

        transform.position = finalPosition;
    }

    /*
    * Called when one item is dragged onto this item
    * Basically what happens when two items interact
    */
    public virtual void DraggedOnto(GameObject otherObject) {
        Debug.Log("Some item was dragged onto another.");
    }
}
