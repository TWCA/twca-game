using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuController : MonoBehaviour
{
    public static MenuController Instance { get; private set; }

    public Stack<SubMenu> openSubmenus = new Stack<SubMenu>();
    public SubMenu[] trackedSubmenus;

    // Start is called before the first frame update
    MenuController()
    {
    }

    private void Awake()
    {
        Instance = this;

        openSubmenus.Push(trackedSubmenus[0]);

        HideUnopened();
    }

    public void ShowNext(SubMenu subMenu) {
        Debug.Log(subMenu);

        if (openSubmenus.Count > 0) {
            SubMenu current = openSubmenus.Peek();
            current.SetVisible(false);
        }

        openSubmenus.Push(subMenu);
        subMenu.SetVisible(true);
    }

    public void ShowNext<T>(T def = default) {
        SubMenu requestedSubmenu = GetSubmenuTracked<T>();

        if (requestedSubmenu) {
            ShowNext(requestedSubmenu);
        }
    }

    public void GoBack() {
        SubMenu current = openSubmenus.Pop();
        current.SetVisible(false);

        Debug.Log(current);

        SubMenu next = openSubmenus.Peek();
        next.SetVisible(true);

        Debug.Log(next);
    }

    private SubMenu GetSubmenuTracked<T>() {
        foreach (SubMenu subMenu in trackedSubmenus) {
            if (subMenu is T specificSubMenu) {
                return specificSubMenu as SubMenu;
            }
        }

        return null;
    }

    private void HideUnopened() {
        for (int i = 1; i < trackedSubmenus.Length; i++)
        {
            SubMenu subMenu = trackedSubmenus[i];
            subMenu.SetVisible(false);
        }
    }
}
