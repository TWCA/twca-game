using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class BarkManager : MonoBehaviour
{
    public static BarkManager Instance { get; private set; }

    [FormerlySerializedAs("CurrentLevel")] [SerializeField]
    private Level currentLevel;

    private float TimeOnLevel = 0;
    private float TimeSinceDialog = 0;
    private float TimeSinceBark = 0;
    private float timeSinceTimeTravel = 0;
    private int barksOnLevel = 0;
    private int dialogProgress = 0;
    private Dictionary<string, int> PlayedBarks = new Dictionary<string, int>();

    // story flags
    [SerializeField] private bool kibbleCollected = false;
    [SerializeField] private bool samFed = false;
    [SerializeField] private int timesTimeTraveled = 0;
    [SerializeField] private bool keyCollected = false;
    [SerializeField] private bool gateUnlocked = false;
    [SerializeField] private bool triedToFillInFuture = false;
    [SerializeField] private bool failedJump = false;
    [SerializeField] private bool jumpedDown = false;

    [SerializeField] private bool jumpedUp = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // update the current level and kill ourselves because there is already an instance
            Instance.OnLevelChange(currentLevel);
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
        TimeManager.Instance.onTimeChanged += () =>
        {
            timeSinceTimeTravel = 0;
            timesTimeTraveled++;
        };
    }

    void FixedUpdate()
    {
        TimeOnLevel += Time.deltaTime;
        TimeSinceBark += Time.deltaTime;
        timeSinceTimeTravel += Time.deltaTime;

        if (DialogManager.Instance.IsDialogRunning())
        {
            TimeSinceDialog = 0;
        }
        else
        {
            TimeSinceDialog += Time.deltaTime;
        }

        switch (currentLevel)
        {
            case Level.RobinsRoom:
                if (TimeSinceDialog > 15.0 && dialogProgress >= 1)
                    SuggestBark("bark_feed_sam");
                break;

            case Level.Kitchen:
                if (!kibbleCollected)
                {
                    if (TimeSinceDialog > 15.0)
                        SuggestBark("bark_kibble");
                }
                else if (!samFed)
                {
                    if (TimeSinceDialog > 15.0)
                        SuggestBark("bark_bowl");
                }

                break;

            case Level.Level1:
                if (timesTimeTraveled <= 1)
                {
                    // have not traveled in level
                    if (timeSinceTimeTravel > 15.0)
                        SuggestBark("bark_reception");
                }
                else
                {
                    // have traveled in level
                    if (timeSinceTimeTravel > 45.0 && TimeSinceDialog > 30.0)
                        SuggestBark("bark_reception");
                }

                break;

            case Level.Level2:
                if (!keyCollected)
                {
                    if (TimeOnLevel > 60.0)
                        SuggestBark("bark_gate_locked");
                    else if (TimeOnLevel > 30.0)
                        SuggestBark("bark_gate", RepeatMode.Disabled);
                }
                else if (keyCollected && !gateUnlocked)
                {
                    if (TimeOnLevel > 240.0)
                        SuggestBark("bark_gate_locked");
                    else if (TimeOnLevel > 120.0)
                        SuggestBark("bark_gate", RepeatMode.Disabled);
                }

                break;

            case Level.Level3:
                if (TimeOnLevel > 60.0 && !TimeManager.Instance.IsFuture())
                    SuggestBark("bark_fire_spread", RepeatMode.Disabled);

                break;

            case Level.Descent:
                if (TimeOnLevel > 25.0 && !jumpedDown)
                    SuggestBark("bark_could_jump");

                break;

            // Level4,
            // Level5,
            // Reunited,
            // Return,
            // Ending
        }
    }

    /**
     * Checks if it's going to be annoying to play a bark
     * If it's not annoying plays it
     */
    private bool SuggestBark(string knot, RepeatMode repeatMode = RepeatMode.Discouraged)
    {
        if (DialogManager.Instance.IsDialogRunning()) return false;

        int timesPlayed = PlayedBarks.GetValueOrDefault(knot);

        if (timesPlayed >= 3 && barksOnLevel > 0)
            return false; // repeated too many times

        if (timesPlayed == 0)
        {
            // allow instantly unless we have played multiple lines have played in this level
            if (barksOnLevel >= 2)
            {
                if (TimeSinceBark < 5.0)
                    return false;
                else if (TimeSinceBark < 2.0)
                    return false;
            }
        }
        else // repeating line 
        {
            if (repeatMode == RepeatMode.Encouraged)
            {
                // repeating but that's allowed
                if (TimeSinceBark < 3.0)
                    return false;
            }
            else if (repeatMode == RepeatMode.Disabled)
            {
                return false;
            }
            else // repeatMode == RepeatMode.Discouraged
            {
                if (barksOnLevel >= 2) // 2+ barks on this level
                {
                    if (TimeSinceBark < 120.0)
                        return false;
                }
                else if (barksOnLevel == 1) // 1 bark on this level
                {
                    if (TimeSinceBark < 30.0)
                        return false;
                }
                else // 0 barks on level, only repeated in another level
                {
                    if (TimeSinceBark < 15.0)
                        return false;
                }
            }
        }

        DialogManager.Instance.StartDialogHeadless(knot);
        PlayedBarks.Add(knot, PlayedBarks.GetValueOrDefault(knot) + 1);
        TimeSinceBark = 0;
        barksOnLevel++;
        return true;
    }

    private void OnLevelChange(Level level)
    {
        currentLevel = level;
        Instance.TimeOnLevel = 0;
        Instance.TimeSinceDialog = 0;
        TimeSinceBark = 0;
        barksOnLevel = 0;
        dialogProgress = 0;
        timeSinceTimeTravel = 0;
    }

    public void OnCollectedItem(GameObject node, GameObject item)
    {
        if (item.name == "Kibble")
            kibbleCollected = true;

        if (item.name == "Key")
            keyCollected = true;
    }

    public void OnPlacedItem(GameObject node, GameObject item)
    {
        if (node.name == "DogBowlDropNode" && item.name == "Kibble")
            samFed = true;

        if (node.name == "GateNode" && item.name == "Key")
            gateUnlocked = true;

        if (node.name == "UnevenBucketNode" && item.name == "BucketEmpty")
            SuggestBark("bark_uneven_ground");

        if (node.name == "FillUpPointFuture" && item.name == "BucketEmpty")
        {
            if (!triedToFillInFuture)
                SuggestBark("bark_slow_fill");
            else
                SuggestBark("bark_slow_fill_hours");

            triedToFillInFuture = true;
        }
    }

    public void OnPlacedItemFailed(GameObject node, GameObject item)
    {
        // TODO
    }

    public void OnDialogTriggered()
    {
        dialogProgress++;
    }

    public void OnNearObstacle(GameObject agent, string name)
    {
        if (agent.tag == "Player" && name == "bush")
            SuggestBark("bark_avoid_fire");
    }

    public void OnJumped(GameObject agent)
    {
        if (currentLevel == Level.Return)
            jumpedUp = true;
        else
            jumpedDown = true;
    }

    public void OnJumpedFailed(GameObject agent)
    {
        if (!failedJump)
            SuggestBark("bark_big_jump", RepeatMode.Encouraged);
        else
            SuggestBark("bark_jump_momentum", RepeatMode.Encouraged);

        failedJump = true;
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

    private enum RepeatMode
    {
        Encouraged,
        Discouraged,
        Disabled
    }
}