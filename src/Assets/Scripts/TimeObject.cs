using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Tracks how time affects an object. All time based items should extend this component.
 */
public class TimeObject : MonoBehaviour
{
    /**
     * A list of situations objects can be in time
     */
    public enum Situation
    {
        PastWithoutFuture, // exists in the past and has no future version
        PastWithFuture, // exists in the past, and is linked to a future version
        Future // exists in the future, may or may not have a past version
    }
    
    /**
     * A list of ways past objects can affect future objects, feel free to add when creating new items
     */
    public enum Affect
    {
        TestSwitched
    }

    /**
     * How the object exists relative to time.
     * If situation is set to PastWithFuture in the editor, and futureVersion is null then futureVersion will be set to a clone of this object on Start()
     */
    [SerializeField] private Situation situation;
    
    /**
     * The future version of an object in the past.
     * If situation is set to PastWithFuture in the editor, and futureVersion is null then futureVersion will be set to a clone of this object on Start()
     */
    [SerializeField] private GameObject futureVersion;
    
    public void Start()
    {
        TimeManager.Instance.RegisterTimeObject(this);
        
        // initializes a prefab to exist in the correct situation
        switch (situation)
        {
            case Situation.PastWithoutFuture:
            case Situation.Future:
                futureVersion = null;
                break;

            case Situation.PastWithFuture:
                if (futureVersion == null)
                {
                    futureVersion = Instantiate(gameObject);
                    TimeObject futureTime = futureVersion.GetComponent<TimeObject>();
                    futureTime.situation = Situation.Future;
                }

                break;
        }

        UpdateActive();
    }

    /**
     * Move game object into a new situation, destroying or creating a future version as needed.
     */
    public void SetSituation(Situation situation)
    {
        this.situation = situation;

        switch (situation)
        {
            case Situation.PastWithoutFuture:
            case Situation.Future:
                if (futureVersion != null)
                {
                    Destroy(futureVersion);
                    futureVersion = null;
                }

                break;

            case Situation.PastWithFuture:
                if (futureVersion == null)
                {
                    futureVersion = Instantiate(gameObject);

                    TimeObject futureTime = futureVersion.GetComponent<TimeObject>();
                    futureTime.situation = Situation.Future;
                }

                break;
        }

        UpdateActive();
    }

    /**
     * Sets this time object to active if it currently exists
     */
    public void UpdateActive()
    {
        gameObject.SetActive(ExistsNow());
    }

    /**
     * Check if the object exists in the current time.
     */
    public bool ExistsNow()
    {
        return TimeManager.Instance.IsFuture() == IsFuture();
    }
    
    /**
     * Check if this object exists in the future
     */
    public bool IsFuture()
    {
        return situation == Situation.Future;
    }

    /**
     * Affects the state of the future version of this object.
     * If there is no future version does nothing.
     */
    protected void AffectFuture(Affect type, System.Object data)
    {
        if (futureVersion == null) return;

        TimeObject futureTime = futureVersion.GetComponent<TimeObject>();
        futureTime.OnPastAffect(type, data);
    }

    /**
      * Hook for changes on the past version of this object.
      */
    protected virtual void OnPastAffect(Affect type, System.Object data)
    {
    }
}