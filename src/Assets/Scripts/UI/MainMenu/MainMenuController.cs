using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    private VisualElement visualElement;
    private Button playButton, settingsButton, exitButton;

    // Start is called before the first frame update
    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();

        visualElement = uiDocument.rootVisualElement;

        playButton = visualElement.Q<Button>("playbutton");
        settingsButton = visualElement.Q<Button>("optionsbutton");
        exitButton = visualElement.Q<Button>("exitbutton");

        HookButtons();
    }

    private void HookButtons() {
        playButton.clicked += PlayClick;
        settingsButton.clicked += SettingsClick;
        exitButton.clicked += ExitClick;
    }

    private void PlayClick() {
        Debug.Log("Play clicked");

        SceneManager.LoadScene("level1");
    }

    private void SettingsClick() {
        Debug.Log("Settings clicked");
    }

    private void ExitClick() {
        Debug.Log("Exit clicked");

        // Source for unity editor exit https://gamedevbeginner.com/how-to-quit-the-game-in-unity/
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        
        Application.Quit();
    }
}
