/** @addtogroup vlUnitySDK
 *  @{
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;
using AOT;

/// <summary>
///  The VLDebugImageBehaviour can be used to visualize debug images using the
///  Unity GUI system.
/// </summary>
/// <remarks>
///  Please use the VLDebugImage prefab. It will ensure, the correct object
///  hierarchy.
/// </remarks>
[AddComponentMenu("VisionLib/VL Debug Image Behaviour")]
public class VLDebugImageBehaviour : VLWorkerReferenceBehaviour
{
    /// <summary>
    ///  Target object with the RawImage component for displaying the debug
    ///  image.
    /// </summary>
    public GameObject imageObject;

    public enum Tracker
    {
        Undefined = 0, // The user can enter the key directly. Other trackers
                       // have a predefined set of image labels with
                       // corresponding keys
        ModelTracker_v1 = 1,
        HololensModelTracker_v1 = 2
    };

    /// <summary>
    ///  Array with available internal image keys for different trackers.
    /// </summary>
    public static readonly string[][] keys = new string[][]
    {
        // Undefined
        new string[]
        {
            ""
        },
        // modelTracker_v1
        new string[]
        {
            "LineTrackerDebugImage1",
            "LineTrackerDebugImage0",
            "CurrentImageRGBA"
        },
        // hololensModelTracker_v1
        new string[]
        {
            "LineTrackerDebugImage1",
            "LineTrackerDebugImage0",
            "CurrentImageRGBA"
        }
    };

    /// <summary>
    ///  Array with labels for the available internal image keys.
    /// </summary>
    public static readonly string[][] labels = new string[][]
    {
        // Undefined
        new string[]
        {
            ""
        },
        // modelTracker_v1
        new string[]
        {
            "LineTracker_Coarse",
            "LineTracker_Fine",
            "CameraImage"
        },
        // hololensModelTracker_v1
        new string[]
        {
            "LineTracker_Coarse",
            "LineTracker_Fine",
            "CameraImage"
        }
    };

    /// <summary>
    ///  Used tracking method.
    /// </summary>
    /// <remarks>
    ///  Using <see cref="Tracker.Undefined"/> allows you to manually
    ///  enter an internal image key. Please notice, that the image keys might
    ///  change in future. Therefore you might want to stick to defined
    ///  tracking methods and the corresponding image labels.
    /// </remarks>
    public Tracker tracker = Tracker.Undefined;

    /// <summary>
    ///  Used image label / key.
    /// </summary>
    public string id = "CurrentImageRGBA";

    private Tracker internalTracker;
    private string internalId;
    private bool subscribed;

    private GCHandle gcHandle;

    private Texture2D texture;
    private byte[] textureData;

    private RectTransform imageRectTransform;
    private VLAspectRatioFitter imageAspectRatioFitter;
    private RawImage rawImage;

    private ScreenOrientation screenOrientation = ScreenOrientation.Portrait;

    // Rotations for different screen orientations
    private static readonly Quaternion rot0 =
        Quaternion.AngleAxis(0.0f, Vector3.forward);
    private static readonly Quaternion rot90 =
        Quaternion.AngleAxis(90.0f, Vector3.forward);
    private static readonly Quaternion rot180 =
        Quaternion.AngleAxis(180.0f, Vector3.forward);
    private static readonly Quaternion rot270 =
        Quaternion.AngleAxis(270.0f, Vector3.forward);

    private bool KeyChanged()
    {
        // If the tracker was changed, then the key might also be different,
        // because the ID could map to a different key for each tracker
        if (this.tracker != this.internalTracker)
        {
            return true;
        }

        return this.id != this.internalId;
    }

    private string GetKey()
    {
        // For the 'undefined' tracker the ID contains the key
        if (this.tracker == Tracker.Undefined)
        {
            return this.id;
        }

        // For other trackers, we need to find the key which corresponds to
        // the ID
        int index = Array.IndexOf(
            VLDebugImageBehaviour.labels[(int)this.tracker], this.id);
        if (index >= 0)
        {
            return VLDebugImageBehaviour.keys[(int)this.tracker][index];
        }

        return "";
    }

    private string GetInternalKey()
    {
        // For the 'undefined' tracker the ID contains the key
        if (this.internalTracker == Tracker.Undefined)
        {
            return this.internalId;
        }

        // For other trackers, we need to find the key which corresponds to
        // the ID
        int index = Array.IndexOf(
            VLDebugImageBehaviour.labels[(int)this.internalTracker],
            this.internalId);
        if (index >= 0)
        {
            return VLDebugImageBehaviour.keys[(int)this.internalTracker][index];
        }

        return "";
    }

    // Dispatch event to object instance
    [MonoPInvokeCallback(typeof(VLWorker.ImageWrapperCallback))]
    private static void DispatchNamedImageEvent(IntPtr handle, IntPtr clientData)
    {
        try
        {
            VLImageWrapper image = new VLImageWrapper(handle, false);
            GCHandle gcHandle = GCHandle.FromIntPtr(clientData);
            VLDebugImageBehaviour debugImageBehaviour =
                (VLDebugImageBehaviour)gcHandle.Target;
            debugImageBehaviour.OnImage(image);
            image.Dispose();
        }
        catch (Exception e) // Catch all exceptions, because this is a callback
                            // invoked from native code
        {
            Debug.LogError("[vlUnitySDK] " + e.GetType().Name + ": " +
                e.Message);
        }
    }
    private static VLWorker.ImageWrapperCallback dispatchNamedImageEventDelegate =
        new VLWorker.ImageWrapperCallback(DispatchNamedImageEvent);

    private static TextureFormat ImageFormatToTextureFormat(
        VLUnitySdk.ImageFormat imageFormat)
    {
        switch (imageFormat)
        {
        case VLUnitySdk.ImageFormat.Grey:
            return TextureFormat.Alpha8;
        case VLUnitySdk.ImageFormat.RGB:
            return TextureFormat.RGB24;
        case VLUnitySdk.ImageFormat.RGBA:
            return TextureFormat.RGBA32;
        }

        throw new ArgumentException("Unsupported image format");
    }

    private void UpdateImageSize()
    {
        if (!this.imageRectTransform)
        {
            return;
        }
        if (this.screenOrientation == VLUnityCameraHelper.ScreenOrientation0)
        {
            this.imageRectTransform.rotation = rot0;
        }
        else if (this.screenOrientation == VLUnityCameraHelper.ScreenOrientation180)
        {
            this.imageRectTransform.rotation = rot180;
        }
        else if (this.screenOrientation == VLUnityCameraHelper.ScreenOrientation90CCW)
        {
            this.imageRectTransform.rotation = rot90;
        }
        else if (this.screenOrientation == VLUnityCameraHelper.ScreenOrientation90CW)
        {
            this.imageRectTransform.rotation = rot270;
        }

        if (!this.rawImage || !this.imageAspectRatioFitter)
        {
            return;
        }

        this.imageAspectRatioFitter.aspectRatio =
            (float)this.rawImage.texture.width / this.rawImage.texture.height;
    }

    private void OnOrientationChange(ScreenOrientation orientation)
    {
        this.screenOrientation = orientation;
        this.UpdateImageSize();
    }

    private void OnImage(VLImageWrapper image)
    {
        // Use camera image as texture
        int imageWidth = image.GetWidth();
        int imageHeight = image.GetHeight();
        if (imageWidth > 0 && imageHeight > 0)
        {
            int imageByteSize = imageWidth * imageHeight *
                image.GetBytesPerPixel();
            if (this.textureData == null ||
                this.textureData.Length != imageByteSize)
            {
                this.textureData = new byte[imageByteSize];
            }

            // Copy the image into a byte array
            if (image.CopyToBuffer(this.textureData))
            {
                // Generate a texture from the byte array
                VLUnitySdk.ImageFormat imageFormat = image.GetFormat();
                TextureFormat textureFormat =
                    ImageFormatToTextureFormat(imageFormat);
                if (!this.texture ||
                    this.texture.width != imageWidth ||
                    this.texture.height != imageHeight ||
                    this.texture.format != textureFormat)
                {
                    this.texture = new Texture2D(
                        imageWidth, imageHeight, textureFormat, false);
                    this.rawImage.texture = this.texture;

                    this.UpdateImageSize();
                }

                if (this.texture)
                {
                    this.texture.LoadRawTextureData(this.textureData);
                    this.texture.Apply();
                }
            }
        }
    }

    private void Awake()
    {
        this.texture = Texture2D.blackTexture;
        this.textureData = new byte[1];

        if (this.imageObject != null)
        {
            this.imageRectTransform =
                this.imageObject.GetComponentInChildren<RectTransform>();
            this.imageAspectRatioFitter =
                this.imageObject.GetComponentInChildren<VLAspectRatioFitter>();
            this.rawImage = this.imageObject.GetComponentInChildren<RawImage>();
            this.rawImage.uvRect = new Rect(0.0f, 0.0f, 1.0f, -1.0f);
            this.rawImage.texture = this.texture;
        }
        else
        {
            Debug.LogWarning("[vlUnitySDK] VLDebugImageBehaviour 'imageObject' must be set");
        }
    }

    private void Subscribe()
    {
        if (this.worker != null)
        {
            if (this.worker.AddNamedImageListener(this.GetKey(),
                dispatchNamedImageEventDelegate,
                GCHandle.ToIntPtr(this.gcHandle)))
            {
                this.internalTracker = this.tracker;
                this.internalId = this.id;
                this.subscribed = true;
            }
            else
            {
                Debug.LogWarning("[vlUnitySDK] Failed to add named image listener");
            }
        }
    }

    private void Unsubscribe()
    {
        if (this.worker != null &&
            !this.worker.GetDisposed() &&
            this.subscribed)
        {
            string key = this.GetInternalKey();
            if (!this.worker.RemoveNamedImageListener(key,
                dispatchNamedImageEventDelegate,
                GCHandle.ToIntPtr(this.gcHandle)))
            {
                Debug.LogWarning("[vlUnitySDK] Failed to remove named image listener");
            }
            this.subscribed = false;
        }
    }

    private void OnEnable()
    {
        // Get a handle to the current object and make sure, that the object
        // doesn't get deleted by the garbage collector. We then use this
        // handle as client data for the native callbacks. This allows us to
        // retrieve the current address of the actual object during the
        // callback execution. GCHandleType.Pinned is not necessary, because we
        // are accessing the address only through the handle object, which gets
        // stored in a global handle table.
        this.gcHandle = GCHandle.Alloc(this);

        this.OnOrientationChange(
            VLDetectScreenChangeBehaviour.GetOrientation(this.gameObject));
        VLDetectScreenChangeBehaviour.OnOrientationChange += OnOrientationChange;

        // Get the worker from the object with the WorkerBehaviour component
        this.InitWorkerReference();

        this.Subscribe();
    }

    private void OnDisable()
    {
        this.Unsubscribe();

        VLDetectScreenChangeBehaviour.OnOrientationChange -= OnOrientationChange;

        // Release the handle to the current object
        this.gcHandle.Free();
    }

    private void Update()
    {
        if (this.InitWorkerReference())
        {
            if (this.KeyChanged())
            {
                this.Unsubscribe();
                this.Subscribe();
            }
        }
    }
}

/**@}*/