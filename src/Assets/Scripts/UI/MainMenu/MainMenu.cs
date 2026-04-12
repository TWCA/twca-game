using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : SubMenu
{
    public Button playButton, settingsButton, creditsButton, exitButton;
    public Text versionText;
    public string targetScene;

    protected override void OnEnable()
    {
        base.OnEnable();

        versionText.text = "Version " + Application.version;
        //MusicPlayer.Instance.loopTrack();
        HookButtons();
        
        if (PlayerPrefs.HasKey("justFinished") && PlayerPrefs.GetInt("justFinished") == 1)
        {
            PlayerPrefs.SetInt("justFinished", 0);
            DialogManager.Instance.StartDialogHeadless("after_credits");
            CreditsClick();
        }
    }

    private void HookButtons() {
        HookButton(playButton, PlayClick);
        HookButton(settingsButton, SettingsClick);
        HookButton(creditsButton, CreditsClick);
        HookButton(exitButton, ExitClick);
    }

    private void PlayClick() {
        StartCoroutine(TransitionController.Instance.SwitchScenes(targetScene, ""));
        MusicPlayer.Instance.stopPlayer();
    }

    private void SettingsClick() {
        MenuController menuController = MenuController.Instance;

        menuController.ShowNext<Settings>();
    }

    private void CreditsClick()
    {
        MenuController menuController = MenuController.Instance;

        menuController.ShowNext<Credits>();
    }

    private void ExitClick() {
        // Source for unity editor exit https://gamedevbeginner.com/how-to-quit-the-game-in-unity/
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        
        Application.Quit();
    }
}
