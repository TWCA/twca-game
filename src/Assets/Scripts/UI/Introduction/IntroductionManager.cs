using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class IntroductionManager : MonoBehaviour
{
    public string MainMenuSceneName;
    public float BeforeTime = 1;
    public float TransitionTime = 1;
    public float ShowTime = 3;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform) {
            child.gameObject.SetActive(false);
        }

        StartCoroutine(SceneProgression());
    }

    IEnumerator SceneProgression() {
        Transform[] children = transform.Cast<Transform>().ToArray();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = children[i];

            GameObject childObject = child.gameObject;
            UIDocument childUIDocument = childObject.GetComponent<UIDocument>();

            if (childUIDocument != null)
            {
                childObject.SetActive(true);

                VisualElement visualElement = childUIDocument.rootVisualElement;
                VisualElement animateComponent = visualElement.Q("animatecomponent");

                animateComponent.AddToClassList("hidden");

                yield return new WaitForSeconds(BeforeTime);

                animateComponent.RemoveFromClassList("hidden");

                yield return new WaitForSeconds(ShowTime);

                animateComponent.AddToClassList("hidden");

                yield return new WaitForSeconds(TransitionTime);

                if (i < transform.childCount - 1) {
                    childObject.SetActive(false);
                }
            }
        }

        SceneManager.LoadScene("MainMenu");
    }
}
