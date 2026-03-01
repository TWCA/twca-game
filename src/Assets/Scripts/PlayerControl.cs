using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public bool CanMove = true;
    private PathFollower pathFollower;
    private Animator animator;
    private SpriteRenderer sprite;

    private InputActionMap playerActionMap, UIActionMap;
    private InputAction moveAction, clickAction, pointAction;

    private InventorySystem inventorySystem;

    public void Start()
    {
        pathFollower = GetComponent<PathFollower>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        inventorySystem = InventorySystem.Instance;

        playerActionMap = InputSystem.actions.FindActionMap("Player");
        UIActionMap = InputSystem.actions.FindActionMap("UI");

        moveAction = playerActionMap.FindAction("Move");
        clickAction = playerActionMap.FindAction("Click");
        pointAction = UIActionMap.FindAction("Point");

        // pathfind when the mouse is clicked
        clickAction.performed += PathfindToMouse;
        clickAction.Enable();
    }

    public void Update()
    {
        Vector2 inputDirection = Vector2.zero;
        bool pointerOverUI = EventSystem.current.IsPointerOverGameObject();

        // Toggle the player click action depending on the situation
        if (pointerOverUI || inventorySystem.CarriedItem != null) {
            clickAction.Disable();
        } else {
            clickAction.Enable();
        }

        if (CanMove)
            inputDirection = moveAction.ReadValue<Vector2>();

        // stop pathfinding path if manual input is entered
        if (!inputDirection.Equals(Vector2.zero))
        {
            pathFollower.StopPathfinding();

            // Reset bringing an item to a location if the player overrides
            inventorySystem.TargetDropNode = null;
            inventorySystem.CarriedItem = null;
        }

        Vector2 movementDirection;
        if (pathFollower.IsPathfinding())
            // find our movement direction if we are pathfinding
            movementDirection = pathFollower.GetPathfindingDirection();
        else
            // try walking if we are not pathfinding already
            movementDirection = pathFollower.WalkTowards(inputDirection, Time.deltaTime);

        bool moving = !movementDirection.Equals(Vector2.zero);
        animator.SetBool("moving", moving);

        if (moving) // only update while moving
            sprite.flipX = movementDirection.x > 0;
    }

    /**
     * Start pathing to a location on the map.
     */
    private void PathfindToMouse(InputAction.CallbackContext context)
    {
        if (!isActiveAndEnabled || !CanMove) return;
        Vector2 targetPosition = GetMouseWorldPosition();
        pathFollower.PathfindTo(targetPosition);
    }

    /**
     * Get the world position of the mouse.
     */
    private Vector2 GetMouseWorldPosition()
    {
        Vector2 screenPoint = pointAction.ReadValue<Vector2>();

        Camera mainCamera = Camera.main;
        if (mainCamera == null) return default;

        Ray ray = Camera.main.ScreenPointToRay(screenPoint);
        Plane plane = new Plane(Vector3.forward, Vector3.zero);

        plane.Raycast(ray, out float dist);
        return ray.GetPoint(dist);
    }

    /*
    * Stops all pathfinding and halts the player where they are
    */
    public void StopInPlace() {
        pathFollower.StopPathfinding();
    }
}