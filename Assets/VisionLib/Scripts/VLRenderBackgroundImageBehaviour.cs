/** @addtogroup vlUnitySDK
 *  @{
 */

using UnityEngine;
using System.Collections;

/// <summary>
///  Behaviour used for rendering the camera image in the background.
/// </summary>
/// <remarks>
///  <para>
///   The necessary GameObjects for rending the camera image will be added to
///   the scene at runtime.
///  </para>
/// </remarks>
[AddComponentMenu("VisionLib/VL Render Background Image Behaviour")]
public class VLRenderBackgroundImageBehaviour : MonoBehaviour
{
    /// <summary>
    ///  Layer for rending the background image.
    /// </summary>
    /// <remarks>
    ///  This can't be changed at runtime.
    /// </remarks>
    public int backgroundLayer = 8;

    /// <summary>
    ///  Color used to clear the screen.
    /// </summary>
    /// <remarks>
    ///  This can't be changed at runtime.
    /// </remarks>
    public Color clearColor = Color.black;

    /// <summary>
    ///  Material used for rendering the background image.
    /// </summary>
    /// <remarks>
    ///  This can't be changed at runtime.
    /// </remarks>
    public Material imageMaterial = null;

    private GameObject backgroundGO;

    class ClearCameraData
    {
        public GameObject go = null;
        public Camera camera = null;
    }

    private class BackgroundCameraData
    {
        public GameObject go = null;
        public Camera camera = null;
    }

    private class BackgroundMeshData
    {
        public GameObject go = null;
        public Mesh mesh = null;
        public MeshFilter meshFilter = null;
        public MeshRenderer meshRenderer = null;
        public Material material = null;
        public Texture2D texture = null;
        public byte[] textureData = new byte[1];
    }

    //private ClearCameraData clearCameraData = null;
    private BackgroundCameraData backgroundCameraData0 = null;
    private BackgroundMeshData backgroundMeshData0 = null;

    private ScreenOrientation screenOrientation = ScreenOrientation.Portrait;
    private int screenWidth = -1;
    private int screenHeight = -1;

    // UV coordinates for different screen orientations
    // (the v-axis is always flipped, because the image is copied into the
    // texture upside down)
    private static readonly Vector2[] UV0 =
    {
        new Vector2(0.0f, 1.0f), // Bottom-left
        new Vector2(1.0f, 1.0f), // Bottom-right
        new Vector2(1.0f, 0.0f), // Top-right
        new Vector2(0.0f, 0.0f)  // Top-left
    };
    private static readonly Vector2[] UV90 =
    {
        new Vector2(0.0f, 0.0f), // Bottom-left
        new Vector2(0.0f, 1.0f), // Bottom-right
        new Vector2(1.0f, 1.0f), // Top-right
        new Vector2(1.0f, 0.0f)  // Top-left
    };
    private static readonly Vector2[] UV180 =
    {
        new Vector2(1.0f, 0.0f), // Bottom-left
        new Vector2(0.0f, 0.0f), // Bottom-right
        new Vector2(0.0f, 1.0f), // Top-right
        new Vector2(1.0f, 1.0f)  // Top-left
    };
    private static readonly Vector2[] UV270 =
    {
        new Vector2(1.0f, 1.0f), // Bottom-left
        new Vector2(1.0f, 0.0f), // Bottom-right
        new Vector2(0.0f, 0.0f), // Top-right
        new Vector2(0.0f, 1.0f)  // Top-left
    };

    private static GameObject CreateRoot()
    {
        return new GameObject("VLBackground");
    }

    private static ClearCameraData CreateClearCamera(
        Transform parentTransform, Color clearColor)
    {
        ClearCameraData data = new ClearCameraData();

        data.go = new GameObject("VLBackgroundClearCamera");
        data.go.transform.parent = parentTransform;

        data.camera = data.go.AddComponent<Camera>();
        data.camera.depth = 0; // First render path
        data.camera.cullingMask = 0; // Render nothing
        // Clear the screen with black
        data.camera.clearFlags = CameraClearFlags.SolidColor;
        data.camera.backgroundColor = clearColor;

        return data;
    }

    private static BackgroundCameraData CreateBackgroundCamera(
        string name, Transform parentTransform, int backgroundLayer)
    {
        BackgroundCameraData data = new BackgroundCameraData();

        data.go = new GameObject(name);
        data.go.transform.parent = parentTransform;

        data.camera = data.go.AddComponent<Camera>();
        data.camera.depth = 1; // Render after clearCamera
        data.camera.cullingMask = 1 << backgroundLayer; // Render only the background image

        // Clear nothing, because the clearCamera already cleared the screen
        data.camera.clearFlags = CameraClearFlags.Nothing;

        // Use an orthographic projection
        data.camera.orthographic = true;
        data.camera.orthographicSize = 0.375f;
        data.camera.nearClipPlane = 0.01f;
        data.camera.farClipPlane = 1.0f;

        return data;
    }

    private static Mesh CreateQuadMesh()
    {
        Mesh mesh = new Mesh();

        mesh.vertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, 0.5f), // Bottom-left
            new Vector3( 0.5f, -0.5f, 0.5f), // Bottom-right
            new Vector3( 0.5f,  0.5f, 0.5f), // Top-right
            new Vector3(-0.5f,  0.5f, 0.5f)  // Top-left

        };

        mesh.normals = new Vector3[]
        {
            Vector3.back,
            Vector3.back,
            Vector3.back,
            Vector3.back
        };

        mesh.uv = UV0;

        mesh.triangles = new int[]
        {
            // Clock-wise, because Unity is left-handed
            0, 2, 1,
            0, 3, 2
        };

#if !UNITY_5_5_OR_NEWER
        // The following function was removed from newer Unity versions.
        // Calling it isn't necessary anymore.
        mesh.Optimize();
#endif

        return mesh;
    }

    private static BackgroundMeshData CreateBackgroundMesh(string name,
        Transform parentTransform, int backgroundLayer, Material material)
    {
        BackgroundMeshData data = new BackgroundMeshData();

        // Create GameObject
        data.go = new GameObject(name);
        data.go.transform.parent = parentTransform;
        data.go.transform.localScale =
            new Vector3(1.0f, 0.75f, 1.0f);
        data.go.layer = backgroundLayer;

        // Create MeshFilter
        data.meshFilter = data.go.AddComponent<MeshFilter>();
        data.mesh = CreateQuadMesh();
        data.meshFilter.mesh = data.mesh;

        // Create MeshRenderer
        data.meshRenderer = data.go.AddComponent<MeshRenderer>();
        data.meshRenderer.shadowCastingMode =
            UnityEngine.Rendering.ShadowCastingMode.Off;
        data.meshRenderer.receiveShadows = false;
#if UNITY_5_4_OR_NEWER
        data.meshRenderer.lightProbeUsage =
            UnityEngine.Rendering.LightProbeUsage.Off;
#else
        data.meshRenderer.useLightProbes = false;
#endif
        data.meshRenderer.reflectionProbeUsage =
            UnityEngine.Rendering.ReflectionProbeUsage.Off;

        // Create texture (will be set properly later)
        data.texture = Texture2D.blackTexture;
        data.textureData = new byte[1];

        // Create Material
        data.material = material;

        // Use texture
        data.material.mainTexture = data.texture;

        // Use material
        data.meshRenderer.material = data.material;

        return data;
    }

    private static Texture2D CreateTexture(int width, int height,
        VLUnitySdk.ImageFormat imageFormat)
    {
        switch (imageFormat)
        {
        case VLUnitySdk.ImageFormat.Grey:
            return new Texture2D(width, height, TextureFormat.Alpha8, false);
        case VLUnitySdk.ImageFormat.RGB:
            return new Texture2D(width, height, TextureFormat.RGB24, false);
        case VLUnitySdk.ImageFormat.RGBA:
            return new Texture2D(width, height, TextureFormat.RGBA32, false);
        }

        return null;
    }

    /// <summary>
    ///  Update the size (scale) of the background mesh and the orthographic
    ///  size of the background camera
    /// </summary>
    private void UpdateBackgroundSize()
    {
        if (!this.backgroundMeshData0.texture)
        {
            return;
        }

        float iw;
        float ih;
        if (this.screenOrientation == VLUnityCameraHelper.ScreenOrientation0 ||
            this.screenOrientation == VLUnityCameraHelper.ScreenOrientation180)
        {
            iw = System.Convert.ToSingle(this.backgroundMeshData0.texture.width);
            ih = System.Convert.ToSingle(this.backgroundMeshData0.texture.height);
        }
        else
        {
            // Swap width and height, because the texture will be rotated,
            // but the VisionLib image stays unchanged
            iw = System.Convert.ToSingle(this.backgroundMeshData0.texture.height);
            ih = System.Convert.ToSingle(this.backgroundMeshData0.texture.width);
        }
        float imageAspectRatio = iw / ih;

        float sw = System.Convert.ToSingle(this.screenWidth);
        float sh = System.Convert.ToSingle(this.screenHeight);
        float screenAspectRatio = sw / sh;

        // Scale the image up until it covers the whole screen
        // TODO(mbuchner): Add mode parameter to scale the image differently
        float targetWidth;
        float targetHeight;
        // Is the relative height larger?
        if (screenAspectRatio < imageAspectRatio)
        {
            // Keep the height and adapt the width accordingly
            targetWidth = sh * imageAspectRatio;
            targetHeight = sh;
        }
        else
        {
            // Keep the width and adapt the height accordingly
            targetWidth = sw;
            targetHeight = sw / imageAspectRatio;
        }

        // Rotate the UV-coordinates, because the size of the background mesh
        // will be adapted to the screen orientation, but the background image
        // always has the same orientation
        if (this.screenOrientation == VLUnityCameraHelper.ScreenOrientation0)
        {
            this.backgroundMeshData0.mesh.uv = UV0;
        }
        else if (this.screenOrientation == VLUnityCameraHelper.ScreenOrientation180)
        {
            this.backgroundMeshData0.mesh.uv = UV180;
        }
        else if (this.screenOrientation == VLUnityCameraHelper.ScreenOrientation90CCW)
        {
            this.backgroundMeshData0.mesh.uv = UV90;
        }
        else if (this.screenOrientation == VLUnityCameraHelper.ScreenOrientation90CW)
        {
            this.backgroundMeshData0.mesh.uv = UV270;
        }

        this.backgroundMeshData0.go.transform.localScale =
            new Vector3(targetWidth, targetHeight, 1.0f);

        this.backgroundCameraData0.camera.orthographicSize = sh / 2.0f;
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
            if (this.backgroundMeshData0.textureData.Length != imageByteSize)
            {
                this.backgroundMeshData0.textureData = new byte[imageByteSize];
            }

            // Copy the image into a byte array
            if (image.CopyToBuffer(this.backgroundMeshData0.textureData))
            {
                // Generate a texture from the byte array
                if (!this.backgroundMeshData0.texture ||
                    this.backgroundMeshData0.texture.width != imageWidth ||
                    this.backgroundMeshData0.texture.height != imageHeight)
                {
                    this.backgroundMeshData0.texture = CreateTexture(
                        imageWidth, imageHeight, image.GetFormat());

                    this.backgroundMeshData0.material.mainTexture =
                        this.backgroundMeshData0.texture;

                    this.UpdateBackgroundSize();
                }

                if (this.backgroundMeshData0.texture)
                {
                    this.backgroundMeshData0.texture.LoadRawTextureData(
                        this.backgroundMeshData0.textureData);
                    this.backgroundMeshData0.texture.Apply();
                }
            }
        }
        else
        {
            Debug.LogWarning("[vlUnitySDK] Image size is 0");
        }
    }

    private void OnOrientationChange(ScreenOrientation orientation)
    {
        this.screenOrientation = orientation;
        this.UpdateBackgroundSize();
    }

    private void OnScreenSizeChange(int screenWidth, int screenHeight)
    {
        this.screenWidth = screenWidth;
        this.screenHeight = screenHeight;
        this.UpdateBackgroundSize();
    }

    private void Awake()
    {
        this.backgroundGO = CreateRoot();

        //this.clearCameraData = CreateClearCamera(
        //    this.backgroundGO.transform, this.clearColor);
        CreateClearCamera(this.backgroundGO.transform, this.clearColor);

        this.backgroundCameraData0 = CreateBackgroundCamera(
            "VLBackgroundCamera0", this.backgroundGO.transform,
            this.backgroundLayer);

        this.backgroundMeshData0 = CreateBackgroundMesh(
            "VLBackgroundMesh0", this.backgroundCameraData0.go.transform,
            this.backgroundLayer, this.imageMaterial);
    }

    private void OnEnable()
    {
        // Unity returns an unknown screen orientation on iOS for some reason.
        // Therefore we check if the value is valid here.
        ScreenOrientation orientation =
            VLDetectScreenChangeBehaviour.GetOrientation(this.gameObject);
        if (orientation == ScreenOrientation.Portrait ||
            orientation == ScreenOrientation.PortraitUpsideDown ||
            orientation == ScreenOrientation.LandscapeLeft ||
            orientation == ScreenOrientation.LandscapeRight)
        {
            this.OnOrientationChange(orientation);
        }
        this.OnScreenSizeChange(Screen.width, Screen.height);

        VLDetectScreenChangeBehaviour.OnOrientationChange += OnOrientationChange;
        VLDetectScreenChangeBehaviour.OnSizeChange += OnScreenSizeChange;

        VLWorkerBehaviour.OnImage += OnImage;
        this.backgroundGO.SetActive(true);
    }

    private void OnDisable()
    {
        // GameObject not destroyed already?
        if (this.backgroundGO != null)
        {
            this.backgroundGO.SetActive(false);
        }
        VLWorkerBehaviour.OnImage -= OnImage;

        VLDetectScreenChangeBehaviour.OnSizeChange -= OnScreenSizeChange;
        VLDetectScreenChangeBehaviour.OnOrientationChange -= OnOrientationChange;
    }

    private void Start()
    {
    }

    private void OnDestroy()
    {
    }

    private void Update()
    {
    }
}

/**@}*/