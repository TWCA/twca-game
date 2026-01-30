using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    private PathFollower pathFollower;

    private InputAction moveAction, clickAction, pointAction;

    public void Start()
    {
        pathFollower = GetComponent<PathFollower>();

        moveAction = InputSystem.actions.FindAction("Move");
        clickAction = InputSystem.actions.FindAction("Click");
        pointAction = InputSystem.actions.FindAction("Point");

        // pathfind when the mouse is clicked
        clickAction.performed += PathfindToMouse;
        clickAction.Enable();
    }

    public void Update()
    {
        Vector2 movement = moveAction.ReadValue<Vector2>();
        
        // stop pathfinding path if manual input is entered
        if(!movement.Equals(Vector2.zero))
            pathFollower.StopPathfinding();

        // try walking if we are not pathfinding already
        if (!pathFollower.IsPathfinding())
            pathFollower.WalkTowards(movement, Time.deltaTime);
    }

    /**
     * Start pathing to a location on the map.
     */
    private void PathfindToMouse(InputAction.CallbackContext context)
    {
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
}