using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    public event Action PlayerTouched;
    private bool ignore;

    // Start is called before the first frame update
    void Start()
    {
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponent<PlayerControl>() && !ignore) {
            Debug.LogWarning("a");
            PlayerTouched.Invoke();
            ignore = true;
        }
    }

    void OnTriggerExit2D() {
        ignore = false;
    }
}
