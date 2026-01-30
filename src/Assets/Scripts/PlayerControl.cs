using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    private PathFollower pathFollower;

    private InputAction moveAction, jumpAction, clickAction, pointAction;

    public void Start()
    {
        pathFollower = GetComponent<PathFollower>();

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        clickAction = InputSystem.actions.FindAction("Click");
        pointAction = InputSystem.actions.FindAction("Point");

        clickAction.performed += OnClick;
        clickAction.Enable();
    }

    public void Update()
    {
        Vector2 movement = moveAction.ReadValue<Vector2>();
        
        if(!movement.Equals(Vector2.zero))
            pathFollower.StopPathfinding();

        if (!pathFollower.IsPathfinding())
            pathFollower.WalkTowards(movement, Time.deltaTime);
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        Vector2 targetPosition = GetMouseWorldPosition();
        pathFollower.PathfindTo(targetPosition);
    }

    private Vector2 GetMouseWorldPosition()
    {
        Vector2 screenPoint = pointAction.ReadValue<Vector2>();

        Ray ray = Camera.main.ScreenPointToRay(screenPoint);
        Plane plane = new Plane(Vector3.forward, Vector3.zero);

        plane.Raycast(ray, out float dist);
        return ray.GetPoint(dist);
    }
}