using UnityEngine;
using System.Collections;

/// <summary>
///  This behaviour shows how to implement a loading screen.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(RectTransform))]
public class LoadingScreenBehaviour : MonoBehaviour
{
    private RectTransform rectTransform;
    private Animator animator;
    private int loadingHash = Animator.StringToHash("loading");

    public void Show()
    {
        this.rectTransform.SetAsLastSibling();
        this.animator.SetBool(loadingHash, true);
    }

    public void Hide()
    {
        this.animator.SetBool(loadingHash, false);
    }

    private void Awake()
    {
        this.rectTransform = this.GetComponent<RectTransform>();
        this.animator = this.GetComponent<Animator>();
        this.animator.SetBool(loadingHash, false);
    }

    private void Start()
    {
        VLWorkerBehaviour.OnTrackerInitializing += HandleTrackerInitializing;
        VLWorkerBehaviour.OnTrackerInitialized += HandleTrackerInitialized;

        // It's possible to stop the tracking, before the 'initialized' event
        // was emitted. Therefore we also listen to the 'stop' event.
        VLWorkerBehaviour.OnTrackerStopped += HandleTrackerInitialized;
    }

    private void OnDestroy()
    {
        VLWorkerBehaviour.OnTrackerInitializing -= HandleTrackerInitializing;
        VLWorkerBehaviour.OnTrackerInitialized -= HandleTrackerInitialized;
        VLWorkerBehaviour.OnTrackerStopped -= HandleTrackerInitialized;
    }

    private void HandleTrackerInitializing()
    {
        this.Show();
    }

    private void HandleTrackerInitialized(bool success)
    {
        this.Hide();
    }
}
