using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class TimePortal : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
         if (collision.gameObject.name == "Player")
        {
            TimeManager.Instance.ToggleTime();
        }
    }
       
}

