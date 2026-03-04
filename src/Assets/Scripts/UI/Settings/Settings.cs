using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : SubMenu
{
    public Button BackButton, SaveButton;
    public Slider MasterVolumeSlider;
    private SettingsLoader settingsLoader;
    private SettingsLoader.Setting masterVolumeSetting;
    protected override void OnEnable()
    {
        base.OnEnable();

        settingsLoader = SettingsLoader.Instance;
        masterVolumeSetting = settingsLoader.GetSetting("MasterVolume");

        MasterVolumeSlider.value = masterVolumeSetting.Get();

        HookButtons();
    }

    private void HookButtons() {
        MenuController menuController = MenuController.Instance;

        HookButton(BackButton, menuController.GoBack);
        HookButton(SaveButton, SaveClick);
    }

    private void SaveClick() {
        masterVolumeSetting.Set(MasterVolumeSlider.value);
    }
}
