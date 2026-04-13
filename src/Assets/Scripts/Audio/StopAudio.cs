using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopAudio : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MusicPlayer.Instance.stopPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
