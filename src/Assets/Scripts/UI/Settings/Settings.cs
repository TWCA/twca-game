using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Settings : SubMenu
{
    private Button backButton;
    // Start is called before the first frame update
    protected override void OnEnable()
    {
        base.OnEnable();

        backButton = visualElement.Q<Button>("backbutton");

        HookButtons();
    }

    private void HookButtons() {
        HookButton(backButton, GoBack);
    }
}
