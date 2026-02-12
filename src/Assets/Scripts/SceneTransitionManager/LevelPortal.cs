using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPortal : MonoBehaviour
{
    public string LevelToLoad;
    private TransitionController transitionController;

    void Start()
    {
        transitionController = TransitionController.Instance;
        transitionController.RegisterLevelPortal(transform.position);
    }

    // Handle when the player reaches the level portal
    void OnCollisionEnter2D(Collision2D collision)
    {
        transitionController.SwitchScenes(LevelToLoad);
    }
}
