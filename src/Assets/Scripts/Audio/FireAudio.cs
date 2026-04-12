using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAudio : MonoBehaviour
{
    private AudioSource burnSource;
    private bool stillBurning { get; set; }
    [SerializeField] private AudioClip putOut;
    private ItemDropNode node;
    private bool playedOneShot = false;

    // Start is called before the first frame update
    void Start()
    {
        burnSource = GetComponent<AudioSource>();
        stillBurning = true;
        node = GetComponent<ItemDropNode>();
    }

    // Update is called once per frame
    void Update()
    {
        if (node.used == true && stillBurning)
        {
            stillBurning = false;
        }
        if (TimeManager.Instance.GetLightingTime() <= 0.2)
        {

            if (playedOneShot == false && stillBurning && !burnSource.isPlaying)
            {
                burnSource.Play();
            }
            if (playedOneShot == false && !stillBurning)
            {
                burnSource.Stop();
                burnSource.PlayOneShot(putOut);
                playedOneShot = true;
            }
        } else
        {
            burnSource.Stop();
        }
    }
}
