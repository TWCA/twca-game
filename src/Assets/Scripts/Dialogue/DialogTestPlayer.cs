using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTestPlayer : MonoBehaviour
{
    public float delaySeconds;
    public bool headless;
    public string knot;
    
    private bool played = false;
    
    void Update()
    {
        if (Time.timeSinceLevelLoad > delaySeconds && !played)
        {
            if (headless)
                DialogManager.Instance.StartDialogHeadless(knot);
            else
                DialogManager.Instance.StartDialog(knot);
            
            played = true;
        }
    }
}
