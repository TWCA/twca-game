using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

public class RiverManager : MonoBehaviour
{
    public static RiverManager Instance { get; private set; }
    public AnimatorController RiverNightP1, RiverNightP2, RiverNightP3;
    public Animator NightAnimator;
    public ItemDropNode FirstRiverBlock;
    public ItemDropNode SecondRiverBlock;
    private int CurrentNightPhase = 0;

    void Awake() {
        Instance = this;
    }

    public bool IncrementNightPhase() {
        PathNetwork pathNetwork = PathNetwork.Instance;

        switch (CurrentNightPhase) {
            case 0:
                if (FirstRiverBlock.ActiveItem == null) {
                    return false;
                }
                
                NightAnimator.runtimeAnimatorController = RiverNightP2;

                Destroy(FirstRiverBlock.gameObject);
                pathNetwork.SetPathFutureTraversable(pathNetwork.GetNamedPath("nightP2"), true);

                break;
            case 1:
                if (SecondRiverBlock.ActiveItem == null) {
                    return false;
                }

                NightAnimator.runtimeAnimatorController = RiverNightP3;

                Destroy(SecondRiverBlock.gameObject);
                pathNetwork.SetPathFutureTraversable(pathNetwork.GetNamedPath("nightP2"), false);

                break;
        }

        // NightAnimator.StartPlayback();

        CurrentNightPhase = Math.Clamp(CurrentNightPhase + 1, 0, 2);

        return true;
    }
}
