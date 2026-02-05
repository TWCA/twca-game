using UnityEngine;

public class PickupObject : MonoBehaviour
{
    public bool BeingCarried = false;
    public bool AllowStacking = true; // Can the object be stacked? No support for stack size maximums for now, not sure if needed.
    private InventorySystem inventorySystem;

    // Start is called before the first frame update
    void Start()
    {
        inventorySystem = InventorySystem.Instance;

        SetToMousePosition();
    }

    // Update is called once per frame
    void Update()
    {
        SetToMousePosition();
    }

    /*
    * Called when the object is actually clicked, basically tries to add to inventory
    */
    void OnMouseDown() {
        if (BeingCarried) {
            BeingCarried = false;
        } else {
            bool success = inventorySystem.AddItem(name);

            if (success) {
                Destroy(gameObject);
            }
        }
    }

    /*
    * Moves the object to the mouse current position in world space
    */
    private void SetToMousePosition() {
        if (BeingCarried) {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 finalPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            finalPosition.z = 0;

            transform.position = finalPosition;
        }
    }
}
