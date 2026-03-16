using System;
using Unity.VisualScripting;
using UnityEngine;

public class RiverManager : MonoBehaviour
{
    public static RiverManager Instance { get; private set; }
    public RuntimeAnimatorController RiverNightP1, RiverNightP2, RiverNightP3;
    public Animator NightAnimator;
    public ItemDropNode FirstRiverBlock;
    public ItemDropNode SecondRiverBlock;
    private int CurrentNightPhase = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        TimeManager.Instance.onTimeChanged += () =>
        {
            IncrementNightPhase();
        };
    }

    public bool IncrementNightPhase()
    {
        PathNetwork pathNetwork = PathNetwork.Instance;

        switch (CurrentNightPhase)
        {
            case 0:
                if (FirstRiverBlock.ActiveItem == null)
                {
                    return false;
                }

                NightAnimator.runtimeAnimatorController = RiverNightP2;

                TimeObject timeObject = FirstRiverBlock.AddComponent<TimeObject>();
                timeObject.SetSituation(TimeObject.Situation.PastWithoutFuture);
                pathNetwork.SetPathFutureTraversable(pathNetwork.GetNamedPath("nightP2"), true);

                break;
            case 1:
                if (SecondRiverBlock.ActiveItem == null)
                {
                    return false;
                }

                NightAnimator.runtimeAnimatorController = RiverNightP3;

                Destroy(SecondRiverBlock.gameObject);
                pathNetwork.SetPathFutureTraversable(pathNetwork.GetNamedPath("nightP2"), false);

                pathNetwork.SetPathFutureTraversable(pathNetwork.GetNamedPath("nightP3-A"), true);
                pathNetwork.SetPathFutureTraversable(pathNetwork.GetNamedPath("nightP3-B"), true);

                break;
        }

        // NightAnimator.StartPlayback();

        CurrentNightPhase = Math.Clamp(CurrentNightPhase + 1, 0, 2);

        return true;
    }
}