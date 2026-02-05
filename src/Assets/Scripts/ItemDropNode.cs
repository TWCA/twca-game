using UnityEngine;

public class ItemDropNode : MonoBehaviour
{
    public AllowDenyList AllowDeny;
    public GameObject ActiveItem;
    private CircleCollider2D circleCollider;
    private InventorySystem inventorySystem;
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

        inventorySystem = InventorySystem.Instance;
        transform.localScale = new Vector3(circleCollider.radius, circleCollider.radius, 1);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ItemIncoming(GameObject prefab) {
        if (ActiveItem != null) {
            // Call some abitrary function that runs when one item is dragged onto the other
            ActiveItem.GetComponent<PickupObject>().DraggedOnto(prefab);
        } else {
            ActiveItem = inventorySystem.HeldItem;
            inventorySystem.HeldItem = null;
        }
    }

    void OnMouseDown() {
        if (ActiveItem != null) {
            inventorySystem.CreatePickupObject(ActiveItem);
            ActiveItem = null;
        } else {
            Debug.Log("No active item in this node");
        }
    }
}
