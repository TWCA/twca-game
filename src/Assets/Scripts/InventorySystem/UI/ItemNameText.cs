using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemNameText : MonoBehaviour
{
    public float ShowTime = 3f;
    private Text textComponent;
    private FadeManager fadeManager;

    // Start is called before the first frame update
    void Start()
    {
        textComponent = GetComponent<Text>();
        fadeManager = GetComponent<FadeManager>();

        fadeManager.SetAlpha(0);
        fadeManager.FadeInTime = 0.5f;
        fadeManager.FadeOutTime = 0.5f;
    }

    void Update() {
        if (!fadeManager.HasReachedTarget())
        {
            textComponent.color = fadeManager.GetRGBAatTime(new Color(0.8f, 0.8f, 0.8f), Time.deltaTime);
        }
    }

    /*
    * Sets the text and plays the fade in and fade out animation of the text
    */
    private IEnumerator SetTextInternal(string newText) {
        textComponent.text = newText;

        fadeManager.FadeOut();

        yield return new WaitForSeconds(ShowTime);

        fadeManager.FadeIn();
    }

    /*
    * Sets the text (usually an item name)
    */
    public void SetText(string newText)
    {
        StopAllCoroutines();
        StartCoroutine(SetTextInternal(newText));
    }
}
