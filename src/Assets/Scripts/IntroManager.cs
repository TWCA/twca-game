using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroManager : MonoBehaviour
{
    public VideoPlayer twcaVideoPlayer;
    public SpriteRenderer twcaSprite;
    public SpriteRenderer hikeSprite;
    public float totalTwcaHoldTime = 0.5f;
    public float totalHikeHoldTime = 1.5f;
    public string targetScene;
    private float hikeHoldTime;

    private int progress;
    private float twcaHoldTime;

    private void Awake()
    {
        twcaSprite.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        hikeSprite.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        twcaVideoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "twcaLogo.webm"); 
    }

    private void Update()
    {
        switch (progress)
        {
            case 0:
                if (twcaVideoPlayer.time > 0 && !twcaVideoPlayer.isPlaying)
                {
                    twcaSprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    twcaVideoPlayer.gameObject.SetActive(false);
                    progress = 1;
                }

                break;
            case 1:
                twcaHoldTime += Time.deltaTime;

                if (twcaHoldTime > totalTwcaHoldTime) 
                    progress = 2;
                break;
            case 2:
                twcaSprite.color = new Color(1.0f, 1.0f, 1.0f,
                    Mathf.MoveTowards(twcaSprite.color.a, 0, Time.deltaTime * 0.8f));

                if (twcaSprite.color.a <= 0)
                    progress = 3;

                break;
            case 3:
                hikeSprite.color = new Color(1.0f, 1.0f, 1.0f,
                    Mathf.MoveTowards(hikeSprite.color.a, 1, Time.deltaTime* 0.8f));

                if (hikeSprite.color.a >= 1)
                    progress = 4;

                break;
            case 4:
                hikeHoldTime += Time.deltaTime;
                if (hikeHoldTime > totalHikeHoldTime)
                    progress = 5;

                break;
            case 5:
                hikeSprite.color = new Color(1.0f, 1.0f, 1.0f,
                    Mathf.MoveTowards(hikeSprite.color.a, 0, Time.deltaTime* 0.8f));

                if (hikeSprite.color.a <= 0)
                    SceneManager.LoadScene(targetScene);
                break;
        }
    }
}