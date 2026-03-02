using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class IntroductionManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform) {
            child.gameObject.SetActive(false);
        }

        StartCoroutine(SceneProgression());
    }

    IEnumerator SceneProgression() {
        foreach (Transform child in transform) {
            GameObject childObject = child.gameObject;
            UIDocument childUIDocument = childObject.GetComponent<UIDocument>();

            if (childUIDocument != null) {
                VisualElement visualElement = childUIDocument.rootVisualElement;

                childObject.SetActive(true);

                yield return new WaitForSeconds(3);

                childObject.SetActive(false);
            }
        }
    }
}
