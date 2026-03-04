using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsLoader : MonoBehaviour
{
    public static SettingsLoader Instance { get; private set; }
    [SerializeField]
    public Setting[] settings;

    SettingsLoader() {}

    void Awake() {
        Instance = this;

        foreach (Setting setting in settings) {
            setting.Load();
        }
    }

    public Setting GetSetting(string name) {
        foreach (Setting setting in settings) {
            if (setting.Name == name) {
                return setting;
            }
        }

        return null;
    }

    /*
    * Only support for floats for now, I will fix later with generics
    */
    [Serializable]
    public class Setting {
        public string Name;
        public float DefaultValue;
        public event Action Updated;
        private float value;

        Setting() {

        }

        public void Load() {
            if (PlayerPrefs.HasKey(Name)) {
                value = PlayerPrefs.GetFloat(Name);
            } else {
                value = DefaultValue;
            }
        }

        public void Set(float newValue) {
            value = newValue;
            PlayerPrefs.SetFloat(Name, newValue);
            Updated.Invoke();
        }

        public float Get() {
            return value;
        }
    }
}
