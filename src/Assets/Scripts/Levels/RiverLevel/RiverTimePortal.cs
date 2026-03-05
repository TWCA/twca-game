using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverTimePortal : TimePortal
{
    private RiverManager riverManager;

    void Start() {
        riverManager = RiverManager.Instance;
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        // base.OnTriggerEnter2D(collision);
        TimeManager.Instance.ToggleTime();
        riverManager.IncrementNightPhase();
    }
}
