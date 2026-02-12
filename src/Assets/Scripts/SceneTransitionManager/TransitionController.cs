using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class TransitionController : MonoBehaviour
{
    public static TransitionController Instance { get; private set; }
    public GameObject screenFadeObject;

    // Changes the amount of time fading in and out takes
    // WARNING: you need to edit this AND the fade.uxml animation time to the time you desire in ms
    // This is because there is no way to change the fade.uxml animation time at runtime afaik
    public int FadeDelay = 2000;
    
    // Changes how long the system waits at the beginning of the game before fading away from the black screen
    // You want to have a reasonable delay because sometimes the window appears a bit after the animation has completed
    public int StartDelay = 1000;
    private PathNetwork pathNetwork;
    private UIDocument screenFadeUIDocument;
    private VisualElement screenFadeVisualElement;


    TransitionController() { }

    private void Awake()
    {
        Instance = this;

        screenFadeUIDocument = screenFadeObject.GetComponent<UIDocument>();
        screenFadeVisualElement = screenFadeUIDocument.rootVisualElement;
    }

    async void Start() {
        screenFadeVisualElement.AddToClassList("fade");

        // wait and then fade out
        await Task.Delay(StartDelay);
        await FadeOut();
    }

    public async void SwitchScenes(string sceneName) {
        await FadeIn();

        // Load desired scene
        SceneManager.LoadSceneAsync(sceneName);
    }

    // Creates a node in the path network corresponding to the position of the level portal
    public void RegisterLevelPortal(Vector2 levelPortalPosition) {
        pathNetwork = PathNetwork.Instance;

        (float nearestNodeDistance, int nearestNode) = pathNetwork.NearestNode(levelPortalPosition);
        pathNetwork.ForkNode(nearestNode, levelPortalPosition);
    }

    // Does the fade in sceen effect
    public async Task FadeIn() {
        screenFadeVisualElement.RemoveFromClassList("no-fade");
        screenFadeVisualElement.AddToClassList("fade");

        await Task.Delay(FadeDelay);
    }

    // Does the fade out screen effect
    public async Task FadeOut() {
        screenFadeVisualElement.RemoveFromClassList("fade");
        screenFadeVisualElement.AddToClassList("no-fade");

        await Task.Delay(FadeDelay);
    }
}
