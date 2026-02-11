using UnityEngine;
using UnityEngine.InputSystem;

public class PickupObject : MonoBehaviour
{
    public bool AllowStacking = true; // Can the object be stacked? No support for stack size maximums for now, not sure if needed.

    [System.NonSerialized]
    public GameObject PickupObjectPrefab;
    private InventorySystem inventorySystem;
    private InputAction clickAction;

    // Start is called before the first frame update
    void Start()
    {
        inventorySystem = InventorySystem.Instance;

        clickAction = InputSystem.actions.FindAction("Click");

        SetToMousePosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (clickAction.WasReleasedThisFrame()) {
            ItemRelease();
        }

        SetToMousePosition();
    }

    /*
    * Decide what to do with an item when it is released by the player based on the current context
    */
    void ItemRelease() {
        ItemDropNode itemDropNode = RaycastManager.IsComponentBelowMouse<ItemDropNode>();
        if (itemDropNode != null) {
            bool accepted = itemDropNode.ItemIncoming(PickupObjectPrefab);

            if (!accepted) {
                inventorySystem.AddItem(PickupObjectPrefab);
            }
        } else {
            inventorySystem.AddItem(PickupObjectPrefab);
        }

        inventorySystem.HeldItem = null;
        Destroy(gameObject);
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
