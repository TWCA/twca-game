using UnityEngine;

public class PickupObject : MonoBehaviour
{
    public bool BeingCarried = false;
    private GameObject canvas;
    private InventorySystem inventorySystem;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("InventoryCanvas");

        inventorySystem = InventorySystem.Instance;

        SetToMousePosition();
    }

    // Update is called once per frame
    void Update()
    {
        SetToMousePosition();
    }

    void OnMouseDown() {
        if (BeingCarried) {
            BeingCarried = false;
        } else {
            inventorySystem.AddItem(name);

            Destroy(gameObject);
        }
    }

    private void SetToMousePosition() {
        if (BeingCarried) {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 finalPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            finalPosition.z = 0;

            transform.position = finalPosition;
        }
    }
}
