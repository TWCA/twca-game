using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PathFollower pathFollower;
    private InputAction moveAction;
    private InputAction jumpAction;
    
    // Start is called before the first frame update
    void Start()
    {
        pathFollower = GetComponent<PathFollower>();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movement = moveAction.ReadValue<Vector2>();
        pathFollower.WalkOnPath((Vector2)transform.position + movement);
    }
}
