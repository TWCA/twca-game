using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SubMenu : MonoBehaviour
{
    private MenuController menuController;
    private Dictionary<Button, UnityAction> hooks;

    public SubMenu() {}

    /*
    * Get our references when the menu is enabled
    */
    protected virtual void OnEnable() {
        hooks = new Dictionary<Button, UnityAction>();
    }

    protected virtual void OnDisable() {
        UnhookAllButtons();
    }

    protected virtual void Start() {
        menuController = MenuController.Instance;
    }

    public void SetVisible(bool visibility) {
        gameObject.SetActive(visibility);
    }

    /*
    * Tracks a button event so we can clear them when we set inactive later
    */
    protected void HookButton(Button button, UnityAction listener) {
        if (button == null) {
            Debug.LogError("You didn't assign a button in the public attributes");
        } else {
            button.onClick.AddListener(listener);

            hooks.Add(button, listener);
        }
    }

    /*
    * Removes all button event references
    */
    private void UnhookAllButtons() {
        if (hooks != null) {
            foreach (KeyValuePair<Button, UnityAction> pair in hooks) {
                pair.Key.onClick.RemoveAllListeners();
            }

            hooks.Clear();
        }
    }
}
