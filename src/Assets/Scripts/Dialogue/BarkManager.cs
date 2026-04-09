using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarkManager : MonoBehaviour
{
    public static BarkManager Instance { get; private set; }

    [SerializeField] private Level CurrentLevel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // update the current level and kill ourselves because there is already an instance
            Instance.CurrentLevel = this.CurrentLevel;
            Destroy(this.gameObject);
        }
        else
        {
            // we are the only one, we become the instance
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }

    void Start()
    {
    }

    void Update()
    {
    }

    private enum Level
    {
        None,
        RobinsRoom,
        Kitchen,
        WalkWithSam,
        Level1,
        Level2,
        Level3,
        Descent,
        Level4,
        Level5,
        Reunited,
        Return,
        Ending
    }
}