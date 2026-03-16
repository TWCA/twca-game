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

    public static PlayerControl Instance { get; private set; }

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Start()
    {
        pathFollower = GetComponent<PathFollower>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        playerActionMap = InputSystem.actions.FindActionMap("Player");
        UIActionMap = InputSystem.actions.FindActionMap("UI");

        moveAction = playerActionMap.FindAction("Move");
        clickAction = playerActionMap.FindAction("Click");
        pointAction = UIActionMap.FindAction("Point");

        // pathfind when the mouse is clicked
        clickAction.performed += PathfindToMouse;
        clickAction.Enable();
    }
    
    private void OnDestroy()
    {
        clickAction.performed -= PathfindToMouse;
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void Update()
    {
        Vector2 inputDirection = Vector2.zero;
        bool pointerOverUI = EventSystem.current.IsPointerOverGameObject(); 
        InventorySystem inventorySystem = InventorySystem.Instance;
        
        // Toggle the player click action depending on the situation
        if (pointerOverUI || inventorySystem.CarriedItem != null || inventorySystem.MouseItem != null) {
            clickAction.Disable();
        } else {
            clickAction.Enable();
        }

        if (IsMovementAllowed())
            inputDirection = moveAction.ReadValue<Vector2>();

        // stop pathfinding path if manual input is entered
        if (!inputDirection.Equals(Vector2.zero))
        {
            pathFollower.StopPathfinding();

            // Reset bringing an item to a location if the player overrides
            inventorySystem.Cancel();
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
        if (!isActiveAndEnabled || !IsMovementAllowed()) return;
        Vector2 targetPosition = GetMouseWorldPosition();
        PathfindTo(targetPosition);
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

    private bool IsMovementAllowed() {
        return CanMove && animator.GetBool("interacting") == false;
    }

    /*
    * Stops all pathfinding and halts the player where they are
    */
    public void StopInPlace() {
        pathFollower.StopPathfinding();
    }

    public void PathfindTo(Vector2 location) {
        pathFollower.PathfindTo(location);
    }
}