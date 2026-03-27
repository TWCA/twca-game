using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class TimePortal : MonoBehaviour
{
    public string dialogKnot;
    public bool headless;
    
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (headless)
                DialogManager.Instance.StartDialogHeadless(dialogKnot, TimeManager.Instance.ToggleTime, 1.1f);
            else
                DialogManager.Instance.StartDialog(dialogKnot, TimeManager.Instance.ToggleTime, 1.1f);
        }
    }
}
