using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionController : MonoBehaviour
{
    public static TransitionController Instance { get; private set; }
    public GameObject screenFadeObject;
    private ScreenFade screenFade;

    private PathNetwork pathNetwork;

    TransitionController()
    {
        pathNetwork = PathNetwork.Instance;
    }

    private void Awake()
    {
        Instance = this;

        screenFade = screenFadeObject.GetComponent<ScreenFade>();
    }

    public void SwitchScenes(string sceneName) {
        screenFade.FadeIn();


        // AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        // operation.allowSceneActivation = false;
    }

    public void RegisterLevelPortal(Vector2 levelPortalPosition) {
        Debug.Log(levelPortalPosition);

        (float nearestNodeDistance, int nearestNode) = pathNetwork.NearestNode(levelPortalPosition);
        pathNetwork.ForkNode(nearestNode, levelPortalPosition);
        // pathNetwork.
    }
}
