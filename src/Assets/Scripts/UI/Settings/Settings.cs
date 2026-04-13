using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Settings : SubMenu
{
    public Button BackButton;
    public Slider MasterVolumeSlider;
    public Text MasterVolumeText;
    public Toggle SubtitlesToggle;
    
    private SettingsLoader settingsLoader;
    private SettingsLoader.Setting masterVolumeSetting;
    private SettingsLoader.Setting subtitlesSetting;

    protected override void OnEnable()
    {
        base.OnEnable();

        settingsLoader = SettingsLoader.Instance;
        masterVolumeSetting = settingsLoader.GetSetting("MasterVolume");
        subtitlesSetting = settingsLoader.GetSetting("Subtitles");

        MasterVolumeSlider.value = masterVolumeSetting.Get();
        SubtitlesToggle.isOn = subtitlesSetting.Get() > 0.5f;

        // This seems cursed but may I present to you the Unity docs: https://docs.unity3d.com/2018.3/Documentation/ScriptReference/UI.Slider-onValueChanged.html
        MasterVolumeSlider.onValueChanged.AddListener(delegate { MasterVolumeAdjusted(); });
        MasterVolumeAdjusted();

        SubtitlesToggle.onValueChanged.AddListener(delegate { SubtitlesAdjusted(); });
        SubtitlesAdjusted();

        HookButtons();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        MasterVolumeSlider.onValueChanged.RemoveAllListeners();
        SubtitlesToggle.onValueChanged.RemoveAllListeners();
    }

    private void HookButtons()
    {
        MenuController menuController = MenuController.Instance;

        HookButton(BackButton, menuController.GoBack);
    }

    private void MasterVolumeAdjusted()
    {
        MasterVolumeText.text = $"{MasterVolumeSlider.value}%";
        masterVolumeSetting.Set(MasterVolumeSlider.value);
    }

    private void SubtitlesAdjusted()
    {
        subtitlesSetting.Set(SubtitlesToggle.isOn ? 1 : 0);
    }
}