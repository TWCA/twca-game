using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SubMenu : MonoBehaviour
{
    protected VisualElement visualElement;
    protected MenuController menuController;
    private Dictionary<Button, Action> hooks;

    public SubMenu() {}

    /*
    * Get our references when the menu is enabled
    */
    protected virtual void OnEnable() {
        UIDocument uiDocument = GetComponent<UIDocument>();

        visualElement = uiDocument.rootVisualElement;

        hooks = new Dictionary<Button, Action>();
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
    protected void HookButton(Button button, Action listener) {
        button.clicked += listener;

        hooks.Add(button, listener);
    }

    /*
    * Removes all button event references
    */
    private void UnhookAllButtons() {
        if (hooks != null) {
            foreach (KeyValuePair<Button, Action> pair in hooks) {
                pair.Key.clicked -= pair.Value;
            }

            hooks.Clear();
        }
    }
}
