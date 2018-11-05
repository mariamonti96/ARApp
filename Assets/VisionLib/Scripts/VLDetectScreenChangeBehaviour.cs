/** @addtogroup vlUnitySDK
 *  @{
 */

using UnityEngine;
using System;
using System.Collections;

[AddComponentMenu("VisionLib/VL Detect Screen Change Behaviour")]
public class VLDetectScreenChangeBehaviour : MonoBehaviour
{
    [Serializable]
    public class Overwrite
    {
        public enum Orientation
        {
            Portrait,
            PortraitUpsideDown,
            LandscapeLeft,
            LandscapeRight
        }

        public bool active = false;
        public Orientation orientation = Orientation.Portrait;
    }

    public delegate void OrientationChangeAction(ScreenOrientation orientation);
    public static event OrientationChangeAction OnOrientationChange;

    public delegate void SizeChangeAction(int width, int height);
    public static event SizeChangeAction OnSizeChange;

    public static VLDetectScreenChangeBehaviour FindInstance(GameObject go)
    {
        // Look for the VLDetectScreenChangeBehaviour at the given GameObject
        // first
        if (go != null)
        {
            VLDetectScreenChangeBehaviour behaviour =
                go.GetComponent<VLDetectScreenChangeBehaviour>();
            if (behaviour != null)
            {
                return behaviour;
            }
        }

        // Try to find it anywhere in the scene
        return UnityEngine.Object.FindObjectOfType<VLDetectScreenChangeBehaviour>();
    }

    public static ScreenOrientation GetOrientation(GameObject go)
    {
        VLDetectScreenChangeBehaviour behaviour = FindInstance(go);
        if (behaviour != null)
        {
            return behaviour.GetOrientation();
        }
        return Screen.orientation;
    }

    /// <summary>
    ///  Settings for overwriting the screen orientation.
    /// </summary>
    /// <remarks>
    ///  On systems without a screen orientation sensor, Unity will always
    ///  report a portrait screen orientation. By activating the orientation
    ///  overwrite, it's possible to simulate a different screen orientation.
    ///  This allows the proper playback of iOS and Android image sequences
    ///  captured in landscape mode with an "imageRecorder" configuration.
    /// </remarks>
    public Overwrite overwrite;

    private ScreenOrientation orientation = ScreenOrientation.AutoRotation;
    private int width = -1;
    private int height = -1;

    /// <summary>
    ///  Returns the current screen orientation considering the overwrite
    ///  setting.
    /// </summary>
    /// <returns>
    ///  Screen.orientation or <see cref="overwrite.orientation"/> depending on
    ///  the <see cref="overwrite.active"/> value.
    /// </returns>
    public ScreenOrientation GetOrientation()
    {
        if (!this.overwrite.active)
        {
            return Screen.orientation;
        }

        switch (this.overwrite.orientation)
        {
        case Overwrite.Orientation.Portrait:
            return ScreenOrientation.Portrait;
        case Overwrite.Orientation.PortraitUpsideDown:
            return ScreenOrientation.PortraitUpsideDown;
        case Overwrite.Orientation.LandscapeLeft:
            return ScreenOrientation.LandscapeLeft;
        case Overwrite.Orientation.LandscapeRight:
            return ScreenOrientation.LandscapeRight;
        }

        return ScreenOrientation.AutoRotation; // This should never happen
    }

    private void Awake()
    {
    }

    private void Start()
    {
        UpdateScreenOrientation();
        UpdateScreenSize();
    }

    private void OnDestroy()
    {
    }

    private ScreenOrientation GetScreenOrientation()
    {
        if (!this.overwrite.active)
        {
            // Get the screen orientation from Unity
            return Screen.orientation;
        }

        // Use the user-defined screen orientation
        switch (this.overwrite.orientation)
        {
        case Overwrite.Orientation.Portrait:
            return ScreenOrientation.Portrait;
        case Overwrite.Orientation.PortraitUpsideDown:
            return ScreenOrientation.PortraitUpsideDown;
        case Overwrite.Orientation.LandscapeLeft:
            return ScreenOrientation.LandscapeLeft;
        case Overwrite.Orientation.LandscapeRight:
            return ScreenOrientation.LandscapeRight;
        }

        // This should never happen
        return ScreenOrientation.AutoRotation;
    }

    private void UpdateScreenOrientation()
    {
        ScreenOrientation currentOrientation = GetScreenOrientation();

        // Orientation not changed?
        if (currentOrientation == this.orientation)
        {
            return;
        }

        // The screen orientation should never be 'AutoRotation'
        if (currentOrientation == ScreenOrientation.AutoRotation)
        {
            Debug.LogWarning("[vlUnitySDK] Cannot derive correct screen orientation");
            return;
        }

        // Tell the native code about the new screen orientation
        if (currentOrientation == ScreenOrientation.Portrait)
        {
            VLUnitySdk.SetScreenOrientation(0);
        }
        else if (currentOrientation == ScreenOrientation.PortraitUpsideDown)
        {
            VLUnitySdk.SetScreenOrientation(1);
        }
        else if (currentOrientation == ScreenOrientation.LandscapeLeft)
        {
            VLUnitySdk.SetScreenOrientation(2);
        }
        else if (currentOrientation == ScreenOrientation.LandscapeRight)
        {
            VLUnitySdk.SetScreenOrientation(3);
        }
        else
        {
            // Unity returns an undocumented value for the screen
            // orientation on iOS for the first few update iterations.
            //Debug.LogWarning("[vlUnitySDK] Unsupported ScreenOrientation: " +
            //    currentOrientation);
            return;
        }

        this.orientation = currentOrientation;

        // Emit change event
        if (OnOrientationChange != null)
        {
            OnOrientationChange(this.orientation);
        }
    }

    private void UpdateScreenSize()
    {
        // Device orientation changed?
        if (Screen.width != this.width ||
            Screen.height != this.height)
        {
            this.width = Screen.width;
            this.height = Screen.height;

            if (OnSizeChange != null)
            {
                OnSizeChange(this.width, this.height);
            }
        }
    }

    private void Update()
    {
        UpdateScreenOrientation();
        UpdateScreenSize();
    }
}

/**@}*/