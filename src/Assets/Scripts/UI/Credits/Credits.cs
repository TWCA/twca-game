using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Credits : SubMenu
{
    public Button BackButton;
    protected override void OnEnable()
    {
        base.OnEnable();
        HookButtons();
    }

    private void HookButtons() {
        MenuController menuController = MenuController.Instance;

        HookButton(BackButton, menuController.GoBack);
    }
}
