/** @addtogroup vlUnitySDK
 *  @{
 */

using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;

/// <summary>
///  This behaviour fires UnityEvents for VLWorkerBehaviour.OnTrackingStates
///  events.
/// </summary>
/// <remarks>
///  This could for example be used to activate / disable certain GameObjects
///  depending on the current tracking state.
/// </remarks>
[AddComponentMenu("VisionLib/VL Tracked Object Behaviour")]
public class VLTrackedObjectBehaviour : MonoBehaviour
{
    [Serializable]
    public class OnTrackedEvent : UnityEvent{}

    [Serializable]
    public class OnCriticalEvent : UnityEvent{}

    [Serializable]
    public class OnLostEvent : UnityEvent{}

    /// <summary>
    ///  Event fired whenever the tracking state is "tracked".
    /// </summary>
    [SerializeField]
    public OnTrackedEvent trackedEvent;

    /// <summary>
    ///  Event fired once after the tracking state changed to "tracked".
    /// </summary>
    [SerializeField]
    public OnTrackedEvent justTrackedEvent;

    /// <summary>
    ///  Event fired whenever the tracking state is "critical".
    /// </summary>
    [SerializeField]
    public OnCriticalEvent criticalEvent;

    /// <summary>
    ///  Event fired once after the tracking state changed to "critical".
    /// </summary>
    [SerializeField]
    public OnCriticalEvent justCriticalEvent;

    /// <summary>
    ///  Event fired whenever the tracking state is "lost".
    /// </summary>
    [SerializeField]
    public OnLostEvent lostEvent;

    /// <summary>
    ///  Event fired once after the tracking state changed to "lost".
    /// </summary>
    [SerializeField]
    public OnLostEvent justLostEvent;

    private string trackedObjectName = "TrackedObject";
    private string previousState;

    void HandleTrackerInitializing()
    {
        this.previousState = "";
    }

    void HandleTrackerStopped(bool success)
    {
    }

    void HandleTrackingStates(VLTrackingState state)
    {
        for (int i=0; i < state.objects.Length; ++i)
        {
            VLTrackingState.TrackingObject obj = state.objects[i];

            if (obj.name == this.trackedObjectName)
            {
                if (obj.state == "tracked")
                {
                    this.trackedEvent.Invoke();
                    if (this.previousState != obj.state)
                    {
                        this.justTrackedEvent.Invoke();
                    }
                }
                else if (obj.state == "critical")
                {
                    this.criticalEvent.Invoke();
                    if (this.previousState != obj.state)
                    {
                        this.justCriticalEvent.Invoke();
                    }
                }
                else if (obj.state == "lost")
                {
                    this.lostEvent.Invoke();
                    if (this.previousState != obj.state)
                    {
                        this.justLostEvent.Invoke();
                    }
                }

                this.previousState = obj.state;

                break;
            }
        }
    }

    void Awake()
    {
        if (this.trackedEvent == null)
        {
            this.trackedEvent = new OnTrackedEvent();
        }
        if (this.justTrackedEvent == null)
        {
            this.justTrackedEvent = new OnTrackedEvent();
        }
        if (this.criticalEvent == null)
        {
            this.criticalEvent = new OnCriticalEvent();
        }
        if (this.justCriticalEvent == null)
        {
            this.justCriticalEvent = new OnCriticalEvent();
        }
        if (this.lostEvent == null)
        {
            this.lostEvent = new OnLostEvent();
        }
        if (this.justLostEvent == null)
        {
            this.justLostEvent = new OnLostEvent();
        }
    }

    void OnEnable()
    {
        VLWorkerBehaviour.OnTrackerInitializing += HandleTrackerInitializing;
        VLWorkerBehaviour.OnTrackerStopped += HandleTrackerStopped;
        VLWorkerBehaviour.OnTrackingStates += HandleTrackingStates;
    }

    void OnDisable()
    {
        VLWorkerBehaviour.OnTrackingStates -= HandleTrackingStates;
        VLWorkerBehaviour.OnTrackerStopped -= HandleTrackerStopped;
        VLWorkerBehaviour.OnTrackerInitializing -= HandleTrackerInitializing;
    }

    void Update()
    {
    }
}

/**@}*/