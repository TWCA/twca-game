using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemNameText : MonoBehaviour
{
    public float ShowTime = 3f;
    private Text textComponent;
    private float showTimer;

    // Start is called before the first frame update
    void Start()
    {
        textComponent = GetComponent<Text>();
    }

    void Update() {
        showTimer += Time.deltaTime;
        
        if (showTimer >= ShowTime) {
            showTimer = 0;
            ClearText();
        }
    }

    private void FadeIn() {
        // TODO fade in
    }

    private void FadeOut() {
        // TODO fade out
    }

    public void ClearText() {
        textComponent.text = "";
    }

    public void SetText(string newText)
    {
        textComponent.text = newText;
        showTimer = 0;

        // TODO
        // FadeIn();
        // wait ShowTime
        // FadeOut();
    }
}
