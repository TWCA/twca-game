using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleManager : MonoBehaviour
{
    public static SubtitleManager Instance { get; private set; }
    public float TextSpeed = 3.0f;

    [SerializeField] private GameObject SubtitleRoot;
    [SerializeField] private Image Background;
    [SerializeField] private Text Name;
    [SerializeField] private Text Message;

    private bool showingMessage = false;
    private float currentAlpha = 0;
    private float totalMessageTime = 0;
    private float currentMessageTime = 0;
    private string subtitleName;
    private string subtitleMessage;

    private SettingsLoader settingsLoader;
    private SettingsLoader.Setting subtitlesSetting;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        SettingsLoader settingsLoader = SettingsLoader.Instance;
        subtitlesSetting = settingsLoader.GetSetting("Subtitles");
    }

    void Update()
    {
        float targetAlpha = showingMessage ? 1 : 0;
        float fadeRate = showingMessage ? 5.0f : 1.0f;
        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.deltaTime * fadeRate);

        Color color = new Color(1, 1, 1, currentAlpha);
        Background.color = color;
        Name.color = color;
        Message.color = color;

        if (showingMessage)
        {
            currentMessageTime = Mathf.MoveTowards(currentMessageTime, totalMessageTime, Time.deltaTime);

            if (currentMessageTime >= totalMessageTime)
                showingMessage = false;

            float progress = Mathf.Min(1, currentMessageTime / totalMessageTime * TextSpeed);
            int displayedLength = (int)(subtitleMessage.Length * progress);

            Name.text = subtitleName;
            Message.text = subtitleMessage.Substring(0, displayedLength);
        }
    }
    
    private void LateUpdate()
    {
        MoveToCamera();
    }

    public void ShowMessage(string name, string message, float time)
    {
        if (subtitlesSetting.Get() <= 0.5f) return;

        showingMessage = true;
        currentMessageTime = 0;
        totalMessageTime = time;
        subtitleName = "(" + name + ")";
        subtitleMessage = message;
    }

    /**
     * Move the dialog UI to th main camera's position.
     */
    private void MoveToCamera()
    {
        GameObject camera = GameObject.FindWithTag("MainCamera");
        Vector3 position = camera.transform.position;
        position.z = SubtitleRoot.transform.position.z;
        SubtitleRoot.transform.position = position;
    }
}