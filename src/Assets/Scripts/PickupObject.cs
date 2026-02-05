using UnityEngine;
using UnityEngine.InputSystem;

public class PickupObject : MonoBehaviour
{
    public bool BeingCarried = false;
    public bool AllowStacking = true; // Can the object be stacked? No support for stack size maximums for now, not sure if needed.
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
    * Called when the object is actually clicked, basically tries to add to inventory
    */
    void OnMouseDown() {
        // BeingCarried = true;
        // if (BeingCarried) {
        //     BeingCarried = false;
        // } else {
        //     bool success = inventorySystem.AddItem(name);

        //     if (success) {
        //         Destroy(gameObject);
        //     }
        // }
    }

    void ItemRelease() {
        ItemDropNode itemDropNode = RaycastManager.IsComponentBelowMouse<ItemDropNode>();
        if (itemDropNode != null) {
            itemDropNode.ActiveItem = PickupObjectPrefab;
        } else {
            inventorySystem.AddItem(name);
        }

        inventorySystem.HeldItem = null;
        Destroy(gameObject);
    }

    /*
    * Moves the object to the mouse current position in world space
    */
    private void SetToMousePosition() {
        // if (BeingCarried) {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 finalPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        finalPosition.z = 0;

        transform.position = finalPosition;
        // }
    }
}
