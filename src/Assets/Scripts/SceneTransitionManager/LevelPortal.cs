using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPortal : MonoBehaviour
{
    private TransitionController transitionController;
    void OnDrawGizmos() {
    }

    // Start is called before the first frame update
    void Start()
    {
        transitionController = TransitionController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        transitionController.SwitchScenes("test");
    }
}
