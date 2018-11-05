/** @addtogroup vlUnitySDK
 *  @{
 */

using UnityEngine;
using UnityEngine.Events;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using AOT;
using VLWorkerCommands;

/// <summary>
///  The VLRuntimeParameterBehaviour can be used to set tracking parameters at
///  runtime.
/// </summary>
/// <remarks>
///  <para>
///   You can use the VLRuntimeParameters_MoelTracker_v1 prefab to create
///   GameObjects with the available parameters attached to them.
///  </para>
///  <para>
///   The <see cref="SetValue"/> functions and the
///   events (<see cref="stringValueChangedEvent"/>,
///   <see cref="intValueChangedEvent"/>, <see cref="floatValueChangedEvent"/>,
///   <see cref="boolValueChangedEvent"/>) can be used to modify parameters
///   using Unity GUI elements.
///  </para>
/// </remarks>
[AddComponentMenu("VisionLib/VL Runtime Parameter Behaviour")]
public class VLRuntimeParameterBehaviour : VLWorkerReferenceBehaviour
{
    /// <summary>
    ///  Event with the value of a string parameter.
    /// </summary>
    [Serializable]
    public class OnStringValueChangedEvent : UnityEvent<string>{}

    /// <summary>
    ///  Event with the value of an integer parameter.
    /// </summary>
    [Serializable]
    public class OnIntValueChangedEvent : UnityEvent<int>{}

    /// <summary>
    ///  Event with the value of a floating point parameter.
    /// </summary>
    [Serializable]
    public class OnFloatValueChangedEvent : UnityEvent<float>{}

    /// <summary>
    ///  Event with the value of a boolean parameter.
    /// </summary>
    [Serializable]
    public class OnBoolValueChangedEvent : UnityEvent<bool>{}

    public enum ParameterType
    {
        String = 0,
        Int,
        Float,
        Bool
    };

    /// <summary>
    ///  Name of the tracking parameter.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Right now this value can't be changed at runtime.
    ///  </para>
    ///  <para>
    ///   The available parameter names depend on the tracking method.
    ///  </para>
    /// </remarks>
    public string parameterName;

    /// <summary>
    ///  Type of the parameter.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Right now this value can't be changed at runtime.
    ///  </para>
    ///  <para>
    ///   Only the corresponding events will get called.
    ///  </para>
    /// </remarks>
    public ParameterType parameterType;

    /// <summary>
    ///  Whether the parameter might be changed by the internal tracking
    ///  pipeline at runtime.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   If a parameter is changing, we will always check for changed values
    ///   and emit events for it. This check will be skipped for parameters
    ///   which are constant and events will only be emitted after the
    ///   initialization.
    ///  </para>
    /// </remarks>
    public bool changing;

    /// <summary>
    ///  Event, which will be invoked after receiving the value of a string
    ///  parameter.
    /// </summary>
    /// <remarks>
    ///  Used if <see cref="parameterType"/> is set to
    ///  <see cref="ParameterType.String"/>.
    /// </remarks>
    [SerializeField]
    public OnStringValueChangedEvent stringValueChangedEvent;

    /// <summary>
    ///  Event, which will be invoked after receiving the value of an integer
    ///  parameter.
    /// </summary>
    /// <remarks>
    ///  Used if <see cref="parameterType"/> is set to
    ///  <see cref="ParameterType.Int"/>.
    /// </remarks>
    [SerializeField]
    public OnIntValueChangedEvent intValueChangedEvent;

    /// <summary>
    ///  Event, which will be invoked after receiving the value of a floating
    //   point parameter.
    /// </summary>
    /// <remarks>
    ///  Used if <see cref="parameterType"/> is set to
    ///  <see cref="ParameterType.Float"/>.
    /// </remarks>
    [SerializeField]
    public OnFloatValueChangedEvent floatValueChangedEvent;

    /// <summary>
    ///  Event, which will be invoked after receiving the value of a boolean
    ///  parameter.
    /// </summary>
    /// <remarks>
    ///  Used if <see cref="parameterType"/> is set to
    ///  <see cref="ParameterType.Bool"/>.
    /// </remarks>
    [SerializeField]
    public OnBoolValueChangedEvent boolValueChangedEvent;

    private string internalParameterName;
    private ParameterType internalParameterType;
    private string parameterValue;

    private GCHandle gcHandle;
    private bool getting;

    /// <summary>
    ///  Set tracking parameter to given value.
    /// </summary>
    /// <remarks>
    ///  The name of the attribute can be specified with the
    ///  <see cref="parameterName"/> member variable.
    /// </remarks>
    public void SetValue(string value)
    {
        this.SetAttribute(value);
    }

    /// <summary>
    ///  Set tracking parameter to given value.
    /// </summary>
    /// <remarks>
    ///  The name of the attribute can be specified with the
    ///  <see cref="parameterName"/> member variable.
    /// </remarks>
    public void SetValue(int value)
    {
        this.SetAttribute(value.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    ///  Set tracking attribute to given value.
    /// </summary>
    /// <remarks>
    ///  The name of the attribute can be specified with the
    ///  <see cref="parameterName"/> member variable.
    /// </remarks>
    public void SetValue(float value)
    {
        this.SetAttribute(value.ToString("R", CultureInfo.InvariantCulture));
    }

    /// <summary>
    ///  Set tracking attribute to given value.
    /// </summary>
    /// <remarks>
    ///  The name of the attribute can be specified with the
    ///  <see cref="parameterName"/> member variable.
    /// </remarks>
    public void SetValue(bool value)
    {
        if (value)
        {
            this.SetAttribute("1");
        }
        else
        {
            this.SetAttribute("0");
        }
    }

    [MonoPInvokeCallback(typeof(VLWorker.JsonStringCallback))]
    private static void DispatchGetAttributeCallback(string errorJson,
        string resultJson, IntPtr clientData)
    {
        try
        {
            GCHandle gcHandle = GCHandle.FromIntPtr(clientData);
            VLRuntimeParameterBehaviour behaviour =
                (VLRuntimeParameterBehaviour)gcHandle.Target;
            behaviour.OnGetAttributeCallback(errorJson, resultJson);
        }
        catch (Exception e) // Catch all exceptions, because this is a callback
                            // invoked from native code
        {
            Debug.LogError("[vlUnitySDK] " + e.GetType().Name + ": " +
                e.Message);
        }
    }
    private static VLWorker.JsonStringCallback dispatchGetAttributeCallbackDelegate =
        new VLWorker.JsonStringCallback(DispatchGetAttributeCallback);

    private static bool ToBoolean(string str)
    {
        if (str == "0")
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void OnGetAttributeCallback(string errorJson, string resultJson)
    {
        this.getting = false;

        // The callback might occur after the behaviour was disabled
        if (!this.enabled)
        {
            return;
        }

        if (errorJson != null)
        {
            CommandError error =
                VLJsonUtility.FromJson<CommandError>(errorJson);
            Debug.LogWarning("[vlUnitySDK] OnGetAttributeCallback: " +
                error.message);
            return;
        }

        GetAttributeResult result =
            VLJsonUtility.FromJson<GetAttributeResult>(resultJson);
        if (this.parameterValue != result.value)
        {
            this.parameterValue = result.value;
            switch (this.internalParameterType)
            {
            case ParameterType.String:
                this.stringValueChangedEvent.Invoke(result.value);
                break;
            case ParameterType.Int:
                this.intValueChangedEvent.Invoke(Convert.ToInt32(result.value));
                this.floatValueChangedEvent.Invoke(Convert.ToSingle(result.value));
                break;
            case ParameterType.Float:
                this.floatValueChangedEvent.Invoke(Convert.ToSingle(result.value));
                break;
            case ParameterType.Bool:
                this.boolValueChangedEvent.Invoke(
                    VLRuntimeParameterBehaviour.ToBoolean(result.value));
                break;
            default:
                Debug.LogWarning("[vlUnitySDK] OnGetAttributeCallback: Unknown parameter type");
                break;
            }
        }
    }

    private void GetAttribute()
    {
        if (this.InitWorkerReference())
        {
            this.worker.PushCommand(
                new GetAttributeCmd(this.internalParameterName),
                VLRuntimeParameterBehaviour.dispatchGetAttributeCallbackDelegate,
                GCHandle.ToIntPtr(this.gcHandle));
        }
    }

    private void SetAttribute(string value)
    {
        if (this.InitWorkerReference())
        {
            this.parameterValue = value;
            SetAttributeCmd.Param param = new SetAttributeCmd.Param(
                this.internalParameterName, value);
            this.worker.PushCommand(
                new SetAttributeCmd(param),
                null, IntPtr.Zero);
        }
    }

    private void OnTrackerInitialized(bool success)
    {
        // Reset the 'getting' flag, because a previously ongoing callback
        // might not get called if the tracker was just re-initialized
        this.getting = false;
        this.parameterValue = null;
        this.GetAttribute();
    }

    private void Awake()
    {
        if (this.stringValueChangedEvent == null)
        {
            this.stringValueChangedEvent = new OnStringValueChangedEvent();
        }
        if (this.intValueChangedEvent == null)
        {
            this.intValueChangedEvent = new OnIntValueChangedEvent();
        }
        if (this.floatValueChangedEvent == null)
        {
            this.floatValueChangedEvent = new OnFloatValueChangedEvent();
        }
        if (this.boolValueChangedEvent == null)
        {
            this.boolValueChangedEvent = new OnBoolValueChangedEvent();
        }

        this.internalParameterName = this.parameterName;
        this.internalParameterType = this.parameterType;

        // Get a handle to the current object and make sure, that the object
        // doesn't get deleted by the garbage collector. We then use this
        // handle as client data for the native callbacks. This allows us to
        // retrieve the current address of the actual object during the
        // callback execution. GCHandleType.Pinned is not necessary, because we
        // are accessing the address only through the handle object, which gets
        // stored in a global handle table.
        this.gcHandle = GCHandle.Alloc(this);
    }

    private void OnEnable()
    {
        VLWorkerBehaviour.OnTrackerInitialized += OnTrackerInitialized;
        this.GetAttribute();
    }

    private void OnDisable()
    {
        VLWorkerBehaviour.OnTrackerInitialized -= OnTrackerInitialized;
    }

    private void Update()
    {
        // It's possible, that the worker is only available later. Therefore
        // we try to get the value as long as the worker is not ready or if
        // the user always wants the latest value.
        if (this.worker == null || this.changing)
        {
            // Don't get the value, if we haven't received the previous value
            if (!this.getting)
            {
                // TODO(mbuchner): Only get the attribute, if there are listeners
                // registered
                this.getting = true;
                this.GetAttribute();
            }
        }
    }

    private void OnDestroy()
    {
        // Release the handle to the current object
        this.gcHandle.Free();
    }
}

/**@}*/