using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogPlayer : MonoBehaviour
{
    public float delaySeconds;
    public bool headless;
    public string knot;
    
    private bool played = false;
    
    void Update()
    {
        delaySeconds -= Time.deltaTime;
        if (delaySeconds <= 0 && !played)
        {
            if (headless)
                DialogManager.Instance.StartDialogHeadless(knot);
            else
                DialogManager.Instance.StartDialog(knot);
            
            played = true;
            delaySeconds = float.PositiveInfinity;
        }
    }
}
