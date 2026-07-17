using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle3Manager : MonoBehaviour
{
    public ItemDropNode FillUpPointInitial;
    public ItemDropNode FillUpPointFuture;
    public ItemDropNode BurningBush;
    public GameObject FullBucket;
    public TimePortal TimePortal;
    public ParticleSystem BushFire;
    public GameObject fire;
    public AudioSource fireSource;
    public bool burning = true;
    public AudioClip PutOutClip;

    // Start is called before the first frame update
    void Start()
    {
        fireSource = fire.GetComponent<AudioSource>();

        TimeManager.Instance.onTimeChanged += HandleTimeChanged;
        BurningBush.ItemPlaced += HandleFireExtinguished;
        fireSource.loop = true;
        fireSource.Play();


    }

    // Update is called once per frame
    void Update()
    {
        if (TimeManager.Instance.GetLightingTime() >= 0.8)
        {
            if (burning)
            {
                fireSource.Pause();
                burning = false;
                Debug.Log("Pausing burning Audio");
            }
        } else {
            if (fire.GetComponent<ItemDropNode>().used != true)
            {
                if (!burning)
                {
                    fireSource.Play();
                    burning = true;
                    Debug.Log("playing burning audio");
                }
            }
        }
    }

    void HandleTimeChanged() {
        if (FillUpPointInitial.ActiveItem != null && TimeManager.Instance.IsFuture()) {
            FillUpPointFuture.ActiveItem = FullBucket;
            FillUpPointFuture.InitializeSprite();
        }
    }

    void HandleFireExtinguished() {
        PathNetwork pathNetwork = PathNetwork.Instance;

        pathNetwork.SetPathPastTraversable(pathNetwork.GetNamedPath("bush"), true);
        Destroy(BushFire);
        fireSource.PlayOneShot(PutOutClip);
        StartCoroutine(PlayEnd(fireSource));
        //fireSource.Pause();
        
        Debug.Log("Put Out Fire. Ending burning audio.");
    }

    public static IEnumerator PlayEnd(AudioSource source)
    {
        yield return new WaitForSeconds(3f);
        source.Pause();
    }
}
