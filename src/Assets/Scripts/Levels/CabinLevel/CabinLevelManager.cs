using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabinLevelManager : MonoBehaviour
{
    public ItemDropNode DogBowl;

    // Start is called before the first frame update
    void Start()
    {
        DogBowl.ItemPlaced += OnBowlFilled;
    }

    void OnBowlFilled() {
        PathNetwork pathNetwork = PathNetwork.Instance;

        pathNetwork.SetPathPastTraversable(pathNetwork.GetNamedPath("bowlfilled"), true);
        DialogManager.Instance.StartDialogHeadless("feedsam");
    }
}
