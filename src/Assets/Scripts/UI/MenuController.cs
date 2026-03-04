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

    /*
    * Shows the subMenu specified next
    */
    public void ShowNext(SubMenu subMenu) {
        if (openSubmenus.Count > 0) {
            SubMenu current = openSubmenus.Peek();
            current.SetVisible(false);
        }

        openSubmenus.Push(subMenu);
        subMenu.SetVisible(true);
    }

    /*
    * Shows the first submenu of a type next
    */
    public void ShowNext<T>(T def = default) {
        SubMenu requestedSubmenu = GetSubmenuTracked<T>();

        if (requestedSubmenu) {
            ShowNext(requestedSubmenu);
        }
    }

    /*
    * Goes back to the last menu
    */
    public void GoBack() {
        SubMenu current = openSubmenus.Pop();
        current.SetVisible(false);

        SubMenu next = openSubmenus.Peek();
        next.SetVisible(true);
    }

    /*
    * Horrific function that finds first menu of a type
    */
    private SubMenu GetSubmenuTracked<T>() {
        foreach (SubMenu subMenu in trackedSubmenus) {
            if (subMenu is T specificSubMenu) {
                return specificSubMenu as SubMenu;
            }
        }

        return null;
    }

    /*
    * Hides all menus except for the first one tracked
    * For example, we want to show the main menu on startup but none of the others
    */
    private void HideUnopened() {
        for (int i = 0; i < trackedSubmenus.Length; i++)
        {
            SubMenu subMenu = trackedSubmenus[i];
            subMenu.SetVisible(i == 0);
        }
    }
}
