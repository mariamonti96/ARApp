using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
///  Turns Input.GetKeyDown into a UnityEvent.
/// </summary>
/// <remarks>
///  This behaviour can be added to the same GameObject multiple times to
///  process different key codes.
/// </remarks>
public class KeyDownEventDispatcher : MonoBehaviour
{
    [Serializable]
    public class OnKeyDownEvent : UnityEvent{}

    public KeyCode keyCode;

    /// <summary>
    ///  Event fired whenever Input.GetKeyDown(keyCode) returns true.
    /// </summary>
    [SerializeField]
    public OnKeyDownEvent keyDownEvent;

    private void Awake()
    {
        if (this.keyDownEvent == null)
        {
            this.keyDownEvent = new OnKeyDownEvent();
        }
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(this.keyCode))
        {
            this.keyDownEvent.Invoke();
        }
    }
}