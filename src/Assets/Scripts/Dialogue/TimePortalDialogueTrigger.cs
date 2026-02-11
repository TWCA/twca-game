using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimePortalDialogueTrigger : MonoBehaviour
{
    public DialogManager dialogManager;

    private void Start()
    {
        if (dialogManager != null && dialogManager.DialogRoot != null)
            dialogManager.DialogRoot.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            dialogManager.StartDialog("checkup_text");
        }
    }
}

