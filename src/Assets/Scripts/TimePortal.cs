using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class TimePortal : MonoBehaviour
{
    public string dialogKnot;
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DialogManager.Instance.StartDialog(dialogKnot, TimeManager.Instance.ToggleTime);
        }
    }
}
