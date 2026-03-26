using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverDynamicAudio : MonoBehaviour
{
    [SerializeField] private AudioSource CalmRiver, intenseRiver;
    private GameObject player;
    private Vector2 pos;
    private float maxVol = 1;
    private bool started = false;

    // Start is called before the first frame update
    void Start()
    {
        pos = gameObject.transform.position;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        float lighting = TimeManager.Instance.GetLightingTime();
        if (AudioManager.Instance.halfVol)
        {
            maxVol = 0.5f;
        }
        else
        {
            maxVol = 1;
        }

        if (lighting <= 0.2)
        {
            playRiverPast();
            stopRiverFuture();
        }
        else if (lighting < 0.8)
        {
            stopRiverPast();
            stopRiverFuture();
        }
        else
        {
            stopRiverPast();
            playRiverFuture();
        }
        if (AudioManager.Instance.fadeout)
        {

            if (!started)
            {
                StartCoroutine(FadeOut(CalmRiver, 2f));
                StartCoroutine(FadeOut(intenseRiver, 2f));
                started = true;
            }

        }
        else
        {
            float distance = Vector2.Distance(pos, player.transform.position);
            float volume = Mathf.Clamp(1 - (distance / 300), 0, maxVol);
            CalmRiver.volume = volume;
            intenseRiver.volume = volume;
        }
    }


    public void playRiverPast()
    {
        CalmRiver.loop = true;
        if (!CalmRiver.isPlaying)
            CalmRiver.Play();
    }

    public void stopRiverPast()
    {
        CalmRiver.Pause();
    }
    public void playRiverFuture()
    {
        intenseRiver.loop = true;
        if (!intenseRiver.isPlaying)
            intenseRiver.Play();
    }

    public void stopRiverFuture()
    {
        intenseRiver.Pause();
    }

    public static IEnumerator FadeOut(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }
        audioSource.volume = 0;
        audioSource.Stop();
    }
}
