using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : SubMenu
{
    private Button playButton, settingsButton, exitButton;

    protected override void OnEnable()
    {
        base.OnEnable();

        playButton = visualElement.Q<Button>("playbutton");
        settingsButton = visualElement.Q<Button>("optionsbutton");
        exitButton = visualElement.Q<Button>("exitbutton");

        HookButtons();
    }

    private void HookButtons() {
        HookButton(playButton, PlayClick);
        HookButton(settingsButton, SettingsClick);
        HookButton(exitButton, ExitClick);
    }

    private void PlayClick() {
        SceneManager.LoadScene("level1");
    }

    private void SettingsClick() {
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
