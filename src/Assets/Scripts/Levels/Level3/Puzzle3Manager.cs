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

    // Start is called before the first frame update
    void Start()
    {
        TimeManager.Instance.onTimeChanged += HandleTimeChanged;
        BurningBush.ItemPlaced += HandleFireExtinguished;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void HandleTimeChanged() {
        if (FillUpPointInitial.ActiveItem != null && TimeManager.Instance.IsFuture()) {
            FillUpPointFuture.ActiveItem = FullBucket;
            FillUpPointFuture.InitializeSprite();
            // FillUpPointInitial.ActiveItem = null;
            // FillUpPointInitial.InitializeSprite();
        }
    }

    void HandleFireExtinguished() {
        PathNetwork pathNetwork = PathNetwork.Instance;

        pathNetwork.SetPathPastTraversable(pathNetwork.GetNamedPath("bush"), true);
        BushFire.Stop();
    }
}
