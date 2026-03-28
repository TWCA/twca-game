using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle3Manager : MonoBehaviour
{
    public ItemDropNode FillUpPointInitial;
    public ItemDropNode FillUpPointFilled;
    public ItemDropNode BurningBush;
    public TimePortal TimePortal;

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
            FillUpPointFilled.gameObject.SetActive(true);
            FillUpPointInitial.gameObject.SetActive(false);
        }
    }

    void HandleFireExtinguished() {
        PathNetwork pathNetwork = PathNetwork.Instance;

        pathNetwork.SetPathPastTraversable(pathNetwork.GetNamedPath("bush"), true);
    }
}
