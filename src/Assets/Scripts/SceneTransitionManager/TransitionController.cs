using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionController : MonoBehaviour
{
    public static TransitionController Instance { get; private set; }

    // Changes the amount of time fading in and out takes
    // WARNING: you need to edit this AND the fade.uxml animation time to the time you desire in ms
    // This is because there is no way to change the fade.uxml animation time at runtime afaik
    public float FadeOutDelay = 2f;

    // Changes how long the system waits at the beginning of the game before fading away from the black screen
    // You want to have a reasonable delay because sometimes the window appears a bit after the animation has completed
    public float FadeInDelay = 1f;

    private PathNetwork pathNetwork;
    private Image screenFadeImage;
    private float alpha;
    private float targetAlpha;
    private float alphaChangeTime;

    private void Awake()
    {
        Instance = this;

        screenFadeImage = GameObject.FindGameObjectWithTag("FadeTexture").GetComponent<Image>();
    }

    void Start()
    {
        screenFadeImage.color = new Color(0, 0, 0, 1.0f);
        alpha = 1;
        FadeIn();
    }

    void Update()
    {
        if (!alpha.Equals(targetAlpha))
        {
            alpha = Mathf.MoveTowards(alpha, targetAlpha, Time.deltaTime / alphaChangeTime);
            screenFadeImage.color = new Color(0, 0, 0, alpha);
        }
    }

    public async void SwitchScenes(string sceneName)
    {
        FadeOut();

        await Task.Delay((int)(FadeOutDelay * 1000));

        // Load desired scene
        SceneManager.LoadSceneAsync(sceneName);
    }

    // Creates a node in the path network corresponding to the position of the level portal & the exit node
    public void RegisterLevelPortal(Vector2 levelPortalPosition, Vector2 exitPosition)
    {
        pathNetwork = PathNetwork.Instance;

        (float _, int nearestNode) = pathNetwork.NearestNode(levelPortalPosition);
        int triggerNode = pathNetwork.ForkNode(nearestNode, levelPortalPosition);

        pathNetwork.ForkNode(triggerNode, exitPosition);
    }

    // Does the fade out screen effect
    public void FadeOut()
    {
        targetAlpha = 1;
        alphaChangeTime = FadeOutDelay;
    }

    // Does the fade in sceen effect
    public void FadeIn()
    {
        targetAlpha = 0;
        alphaChangeTime = FadeInDelay;
    }
}