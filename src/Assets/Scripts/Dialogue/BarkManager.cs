using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class BarkManager : MonoBehaviour
{
    public static BarkManager Instance { get; private set; }

    [SerializeField] private Level CurrentLevel;

    private float TimeOnLevel = 0;
    private float TimeSinceDialog = 0;
    private float TimeSinceBark = 0;
    private int BarksOnLevel = 0;
    private int DialogProgress = 0;
    private Dictionary<string, int> PlayedBarks = new Dictionary<string, int>();

    // story flags
    [SerializeField] private bool kibbleCollected = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // update the current level and kill ourselves because there is already an instance
            Instance.OnLevelChange(CurrentLevel);
            Destroy(this.gameObject);
        }
        else
        {
            // we are the only one, we become the instance
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }

    void Update()
    {
        TimeOnLevel += Time.deltaTime;
        TimeSinceBark += Time.deltaTime;

        if (DialogManager.Instance.IsDialogRunning())
        {
            TimeSinceDialog = 0;
        }
        else
        {
            TimeSinceDialog += Time.deltaTime;
        }

        switch (CurrentLevel)
        {
            case Level.RobinsRoom:
                if (TimeSinceDialog > 15.0 && DialogProgress >= 1)
                    SuggestBark("bark_feed_sam");
                break;

            case Level.Kitchen:
                if (!kibbleCollected)
                {
                    if (TimeSinceDialog > 15.0)
                        SuggestBark("bark_kibble");
                }
                else
                {
                    if (TimeSinceDialog > 15.0)
                        SuggestBark("bark_bowl");
                }

                break;
            // WalkWithSam,
            // Level1,
            // Level2,
            // Level3,
            // Descent,
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

        if (timesPlayed >= 3 && BarksOnLevel > 0)
            return false; // repeated too many times

        if (timesPlayed == 0)
        {
            // allow instantly unless we have played multiple lines have played in this level
            if (BarksOnLevel >= 2)
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
                if (BarksOnLevel >= 2) // 2+ barks on this level
                {
                    if (TimeSinceBark < 120.0)
                        return false;
                }
                else if (BarksOnLevel == 1) // 1 bark on this level
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
        BarksOnLevel++;
        return true;
    }

    private void OnLevelChange(Level level)
    {
        CurrentLevel = level;
        Instance.TimeOnLevel = 0;
        Instance.TimeSinceDialog = 0;
        TimeSinceBark = 0;
        BarksOnLevel = 0;
        DialogProgress = 0;
    }

    public void OnCollectedItem(GameObject node, GameObject item)
    {
        if (item.name == "Kibble")
            kibbleCollected = true;
    }

    public void OnPlacedItem(GameObject node, GameObject item)
    {
        // TODO
    }

    public void OnPlacedItemFailed(GameObject node, GameObject item)
    {
        // TODO
    }

    public void OnDialogTriggered()
    {
        DialogProgress++;
    }

    public void OnJumped(GameObject agent)
    {
        // TODO
    }

    public void OnJumpedFailed(GameObject agent)
    {
        // TODO
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