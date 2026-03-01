using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    public event Action PlayerTouched;
    [NonSerialized] public PlayerControl TouchingPlayer;

    // Start is called before the first frame update
    void Start()
    {
    }

    void OnTriggerEnter2D(Collider2D other) {
        PlayerControl playerControl = other.GetComponent<PlayerControl>();
        if (playerControl && TouchingPlayer == null) {
            TouchingPlayer = playerControl;
            PlayerTouched.Invoke();
        }
    }

    void OnTriggerExit2D() {
        TouchingPlayer = null;
    }
}
