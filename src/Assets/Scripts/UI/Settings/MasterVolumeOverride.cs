using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterVolumeOverride : MonoBehaviour
{
    private SettingsLoader settingsLoader;
    private SettingsLoader.Setting masterVolumeSetting;

    void Start() {
        settingsLoader = SettingsLoader.Instance;

        masterVolumeSetting = settingsLoader.GetSetting("MasterVolume");

        OnMasterVolumeUpdated();

        masterVolumeSetting.Updated += OnMasterVolumeUpdated;
    }

    void OnMasterVolumeUpdated() {
        AudioListener.volume = masterVolumeSetting.Get() / 100f; // Almost went deaf before I divided this by 100
    }
}
