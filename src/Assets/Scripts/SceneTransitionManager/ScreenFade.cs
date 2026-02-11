using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ScreenFade : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement visualElement;

    void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        visualElement = uiDocument.rootVisualElement;

        visualElement.AddToClassList("no-fade");
    }

    public void FadeIn() {   
        visualElement.RemoveFromClassList("no-fade");
        visualElement.AddToClassList("fade");
    }


}
