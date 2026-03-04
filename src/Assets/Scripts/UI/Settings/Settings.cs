using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : SubMenu
{
    public string MasterVolumePrefKey = "MasterVolumePref";
    public Button BackButton, SaveButton;
    public Slider MasterVolumeSlider;
    protected override void OnEnable()
    {
        base.OnEnable();

        HookButtons();
    }

    private void LoadMasterVolume() {
        float MasterVolumeSetting;

        if (PlayerPrefs.HasKey(MasterVolumePrefKey)) {
            MasterVolumeSetting = PlayerPrefs.GetFloat(MasterVolumePrefKey);
        } else {
            MasterVolumeSetting = 50;
        }

        MasterVolumeSlider.value = MasterVolumeSetting;
    }

    private void HookButtons() {
        MenuController menuController = MenuController.Instance;

        HookButton(BackButton, menuController.GoBack);
        HookButton(SaveButton, SaveClick);
    }

    private void SaveClick() {
        PlayerPrefs.SetFloat(MasterVolumePrefKey, MasterVolumeSlider.value);
    }
}
