using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    public event Action PlayerTouched;
    public event Action PlayerLeft;
    [NonSerialized] public bool TouchingPlayer;

    // Start is called before the first frame update
    void Start()
    {
    }

    private bool IsPlayer(Collider2D other) {
        return other.CompareTag("Player");
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (IsPlayer(other) && !TouchingPlayer) {
            TouchingPlayer = true;

            PlayerTouched?.Invoke();
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        Debug.Log(other);
        if (IsPlayer(other)) {
            TouchingPlayer = false;

            PlayerLeft?.Invoke();
        }
    }
}
