using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : SubMenu
{
    public Button playButton, settingsButton, exitButton;
    public Text versionText;
    public string targetScene;

    protected override void OnEnable()
    {
        base.OnEnable();

        versionText.text = "Version " + Application.version;

        HookButtons();
    }

    private void HookButtons() {
        HookButton(playButton, PlayClick);
        HookButton(settingsButton, SettingsClick);
        HookButton(exitButton, ExitClick);
    }

    private void PlayClick() {
        SceneManager.LoadScene(targetScene);
    }

    private void SettingsClick() {
        MenuController menuController = MenuController.Instance;

        menuController.ShowNext<Settings>();
    }

    private void ExitClick() {
        // Source for unity editor exit https://gamedevbeginner.com/how-to-quit-the-game-in-unity/
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        
        Application.Quit();
    }
}
