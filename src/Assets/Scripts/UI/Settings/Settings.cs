using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Settings : SubMenu
{
    public string MasterVolumePrefKey = "MasterVolumePref";
    private Button backButton;
    private SliderInt masterVolume;
    protected override void OnEnable()
    {
        base.OnEnable();

        backButton = visualElement.Q<Button>("backbutton");
        masterVolume = visualElement.Q<SliderInt>("mastervolume");

        HookButtons();
        HookSliders();
    }

    private void HookButtons() {
        MenuController menuController = MenuController.Instance;

        Debug.Log(menuController);

        HookButton(backButton, menuController.GoBack);
    }

    private void HookSliders() {
        masterVolume.RegisterCallback<ChangeEvent<int>>(OnMasterVolumeChanged);
    }

    private void OnMasterVolumeChanged(ChangeEvent<int> changeEvent) {
        Debug.Log("new value");
        Debug.Log(changeEvent.newValue);
        PlayerPrefs.SetInt(MasterVolumePrefKey, changeEvent.newValue);
    }
}
