using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Settings : SubMenu
{
    public Button BackButton, SaveButton;
    public Slider MasterVolumeSlider;
    public Text MasterVolumeText;
    private SettingsLoader settingsLoader;
    private SettingsLoader.Setting masterVolumeSetting;
    protected override void OnEnable()
    {
        base.OnEnable();

        settingsLoader = SettingsLoader.Instance;
        masterVolumeSetting = settingsLoader.GetSetting("MasterVolume");

        MasterVolumeSlider.value = masterVolumeSetting.Get();

        // This seems cursed but may I present to you the Unity docs: https://docs.unity3d.com/2018.3/Documentation/ScriptReference/UI.Slider-onValueChanged.html
        MasterVolumeSlider.onValueChanged.AddListener(delegate {MasterVolumeAdjusted(); });

        MasterVolumeAdjusted();
        HookButtons();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        MasterVolumeSlider.onValueChanged.RemoveAllListeners();
    }

    private void HookButtons() {
        MenuController menuController = MenuController.Instance;

        HookButton(BackButton, menuController.GoBack);
        HookButton(SaveButton, SaveClick);
    }

    private void SaveClick() {
        masterVolumeSetting.Set(MasterVolumeSlider.value);
    }

    private void MasterVolumeAdjusted() {
        MasterVolumeText.text = $"{MasterVolumeSlider.value}%";
    }
}
