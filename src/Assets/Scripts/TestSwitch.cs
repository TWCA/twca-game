using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestSwitch : TimeObject
{
    private bool switched = false;
    private Collider2D interactCollider;
    private GameObject hinge;
    private InputAction interactAction;

    public new void Start()
    {
        base.Start();

        interactCollider = GetComponent<Collider2D>();
        hinge = transform.Find("hinge").gameObject;
        interactAction = InputSystem.actions.FindAction("Interact");

        interactAction.performed += ToggleIfNearPlayer;
        interactAction.Enable();
    }

    /**
     * Checks if the player is in range, and toggles the switch if so
     */
    private void ToggleIfNearPlayer(InputAction.CallbackContext _)
    {
        List<Collider2D> overlappingList = new List<Collider2D>();
        interactCollider.OverlapCollider(new ContactFilter2D().NoFilter(), overlappingList);

        bool inRange = false;
        foreach (Collider2D overlapping in overlappingList)
        {
            if (overlapping.gameObject.name == "Player")
            {
                inRange = true;
                break;
            }
        }

        if (inRange)
            SetSwitched(!switched);
    }

    /**
     * Sets if the switch is toggled
     */
    private void SetSwitched(bool switched)
    {
        this.switched = switched;

        if (switched)
            hinge.transform.rotation = Quaternion.Euler(0, 0, -30);
        else
            hinge.transform.rotation = Quaternion.Euler(0, 0, 30);
        
        // make sure the future version matches the past
        AffectFuture(Affect.TestSwitched, switched);
    }

    /**
     * Handles the past version of this switch affecting this future version
     */
    protected override void OnPastAffect(Affect type, object data)
    {
        if (type == Affect.TestSwitched)
        {
            SetSwitched((bool)data);
        }
    }
}