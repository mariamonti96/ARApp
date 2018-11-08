/*==============================================================================
Copyright (c) 2017-2018 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
==============================================================================*/

using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class PlaneManagerTest : MonoBehaviour
{
    
    #region PUBLIC_MEMBERS
    public PlaneFinderBehaviour m_PlaneFinder;
    
    [Header("Plane, Mid-Air, & Placement Augmentations")]
    //public GameObject m_PlaneAugmentation;
    public GameObject m_PlacementAugmentation;
    public static bool GroundPlaneHitReceived, AstronautIsPlaced;
    
    public static bool AnchorExists
    {
        get { return anchorExists; }
        private set { anchorExists = value; }
    }
    #endregion // PUBLIC_MEMBERS


    #region PRIVATE_MEMBERS
    const string unsupportedDeviceTitle = "Unsupported Device";
    const string unsupportedDeviceBody =
        "This device has failed to start the Positional Device Tracker. " +
        "Please check the list of supported Ground Plane devices on our site: " +
        "\n\nhttps://library.vuforia.com/articles/Solution/ground-plane-supported-devices.html";

    StateManager m_StateManager;
    SmartTerrain m_SmartTerrain;
    PositionalDeviceTracker m_PositionalDeviceTracker;
    ContentPositioningBehaviour m_ContentPositioningBehaviour;
    TouchHandlerTest m_TouchHandler;
    ProductPlacement m_ProductPlacement;
    GroundPlaneTestUI m_GroundPlaneUI;
    AnchorBehaviour m_PlaneAnchor;
    AnchorBehaviour m_PlacementAnchor;
    int AutomaticHitTestFrameCount;
    int m_AnchorCounter;
    bool uiHasBeenInitialized;
    static bool anchorExists; // backs public AnchorExists property
    #endregion // PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS

    void Start()
    {
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaARController.Instance.RegisterOnPauseCallback(OnVuforiaPaused);
        DeviceTrackerARController.Instance.RegisterTrackerStartedCallback(OnTrackerStarted);
        DeviceTrackerARController.Instance.RegisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);

        m_PlaneFinder.HitTestMode = HitTestMode.AUTOMATIC;

        m_ProductPlacement = FindObjectOfType<ProductPlacement>();
        m_TouchHandler = FindObjectOfType<TouchHandlerTest>();
        m_GroundPlaneUI = FindObjectOfType<GroundPlaneTestUI>();

        //m_PlaneAnchor = m_PlaneAugmentation.GetComponentInParent<AnchorBehaviour>();
        //m_MidAirAnchor = m_MidAirAugmentation.GetComponentInParent<AnchorBehaviour>();
        m_PlacementAnchor = m_PlacementAugmentation.GetComponentInParent<AnchorBehaviour>();
        //m_MidAirAnchor2 = m_MidAirAugmentation2.GetComponentInParent<AnchorBehaviour>();

        //UtilityHelperTest.EnableRendererColliderCanvas(m_PlaneAugmentation, false);
        //UtilityHelper.EnableRendererColliderCanvas(m_MidAirAugmentation, false);
        UtilityHelperTest.EnableRendererColliderCanvas(m_PlacementAugmentation, false);
        //UtilityHelper.EnableRendererColliderCanvas(m_MidAirAugmentation2, false);
    }

    void Update()
    {
        if (!VuforiaRuntimeUtilities.IsPlayMode() && !AnchorExists)
        {
            AnchorExists = DoAnchorsExist();
        }

        GroundPlaneHitReceived = (AutomaticHitTestFrameCount == Time.frameCount);

        SetSurfaceIndicatorVisible(GroundPlaneHitReceived && (Input.touchCount == 0));
        //SetSurfaceIndicatorVisible(GroundPlaneHitReceived);
    }

    void OnDestroy()
    {
        //Debug.Log("OnDestroy() called.");

        VuforiaARController.Instance.UnregisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaARController.Instance.UnregisterOnPauseCallback(OnVuforiaPaused);
        DeviceTrackerARController.Instance.UnregisterTrackerStartedCallback(OnTrackerStarted);
        DeviceTrackerARController.Instance.UnregisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);
    }

    #endregion // MONOBEHAVIOUR_METHODS

    #region GROUNDPLANE_CALLBACKS

    public void HandleAutomaticHitTest(HitTestResult result)
    {
        //Debug.Log("WE ARE INSIDE HandleAutomaticHitTest");
        AutomaticHitTestFrameCount = Time.frameCount;

        if (!uiHasBeenInitialized)
        {
            uiHasBeenInitialized = m_GroundPlaneUI.InitializeUI();
        }

        //if (planeMode == PlaneMode.PLACEMENT && !m_ProductPlacement.IsPlaced)
        //{
        //    SetSurfaceIndicatorVisible(false);
        //    m_ProductPlacement.SetProductAnchor(null);
        //    m_PlacementAugmentation.PositionAt(result.Position);
        //}

        if (!m_ProductPlacement.IsPlaced)
        {
            SetSurfaceIndicatorVisible(false);
            m_ProductPlacement.SetProductAnchor(null);
            m_PlacementAugmentation.PositionAt(result.Position);
        }
    }

    public void HandleInteractiveHitTest(HitTestResult result)
    {
        //Debug.Log("WE ARE INSIDE HandleInteractiveHitTest");
        if (result == null)
        {
            Debug.LogError("Invalid hit test result!");
            return;
        }

        if (!m_GroundPlaneUI.IsCanvasButtonPressed())
        {
            Debug.Log("HandleInteractiveHitTest() called.");

            // If the PlaneFinderBehaviour's Mode is Automatic, then the Interactive HitTestResult will be centered.

            // PlaneMode.Ground and PlaneMode.Placement both use PlaneFinder's ContentPositioningBehaviour
            m_ContentPositioningBehaviour = m_PlaneFinder.GetComponent<ContentPositioningBehaviour>();
            m_ContentPositioningBehaviour.DuplicateStage = false;

            // Place object based on Ground Plane mode
            //switch (planeMode)
            //{
            //    case PlaneMode.GROUND:

            //        m_ContentPositioningBehaviour.AnchorStage = m_PlaneAnchor;
            //        m_ContentPositioningBehaviour.PositionContentAtPlaneAnchor(result);
            //        UtilityHelperTest.EnableRendererColliderCanvas(m_PlaneAugmentation, true);

            //         // Astronaut should rotate toward camera with each placement


            //        m_PlaneAugmentation.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            //        //m_PlaneAugmentation.transform.parent.localEulerAngles = new Vector3(0, -90, 0);
            //        UtilityHelperTest.RotateTowardCamera(m_PlaneAugmentation);

            //        Debug.Log("Transform " + m_PlaneAugmentation.transform.rotation.y);
            

            //        AstronautIsPlaced = true;

            //        break;

                  //case PlaneMode.PLACEMENT:

            if (!m_ProductPlacement.IsPlaced || TouchHandlerTest.DoubleTap)
            {
                m_ContentPositioningBehaviour.AnchorStage = m_PlacementAnchor;
                m_ContentPositioningBehaviour.PositionContentAtPlaneAnchor(result);
                UtilityHelperTest.EnableRendererColliderCanvas(m_PlacementAugmentation, true);
            }

            if (!m_ProductPlacement.IsPlaced)
            {
                m_ProductPlacement.SetProductAnchor(m_PlacementAnchor.transform);
                m_TouchHandler.enableRotation = true;
            }

            //break;
        //}
        }
    }

    //public void PlaceObjectInMidAir(Transform midAirTransform)
    //{
    //    if (planeMode == PlaneMode.MIDAIR)
    //    {
    //        Debug.Log("PlaceObjectInMidAir() called.");

    //        m_ContentPositioningBehaviour.AnchorStage = m_MidAirAnchor;
    //        m_ContentPositioningBehaviour.PositionContentAtMidAirAnchor(midAirTransform);
    //        UtilityHelper.EnableRendererColliderCanvas(m_MidAirAugmentation, true);

    //        m_MidAirAugmentation.transform.localPosition = Vector3.zero;
    //        UtilityHelper.RotateTowardCamera(m_MidAirAugmentation);
    //    }
    //}

    #endregion // GROUNDPLANE_CALLBACKS


    #region PUBLIC_BUTTON_METHODS

    public void SetGroundMode(bool active)
    {
        if (active)
        {
            //planeMode = PlaneMode.GROUND;
            //m_GroundPlaneUI.UpdateTitle();
            m_PlaneFinder.enabled = true;
            //m_MidAirPositioner.enabled = false;
            //m_TouchHandler.enableRotation = false;
        }
    }

    //public void SetMidAirMode(bool active)
    //{
    //    if (active)
    //    {
    //        planeMode = PlaneMode.MIDAIR;
    //        m_GroundPlaneUI.UpdateTitle();
    //        m_PlaneFinder.enabled = false;
    //        m_MidAirPositioner.enabled = true;
    //        m_TouchHandler.enableRotation = false;
    //    }
    //}

    public void SetPlacementMode(bool active)
    {
        if (active)
        {
            //planeMode = PlaneMode.PLACEMENT;
            //m_GroundPlaneUI.UpdateTitle();
            m_PlaneFinder.enabled = true;
            //m_MidAirPositioner.enabled = false;
            m_TouchHandler.enableRotation = m_PlacementAugmentation.activeInHierarchy;
            if (m_PlacementAugmentation.activeInHierarchy)
                Debug.Log("the placement is active");
        }
    }

    public void ResetScene()
    {
        Debug.Log("ResetScene() called.");

        // reset augmentations
        //m_PlaneAugmentation.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        //m_PlaneAugmentation.transform.localEulerAngles = new Vector3(0, 0, 0);
        //UtilityHelperTest.EnableRendererColliderCanvas(m_PlaneAugmentation, false);

        //m_MidAirAugmentation.transform.position = Vector3.zero;
        //m_MidAirAugmentation.transform.localEulerAngles = Vector3.zero;
        //UtilityHelper.EnableRendererColliderCanvas(m_MidAirAugmentation, false);

        m_ProductPlacement.Reset();
        UtilityHelperTest.EnableRendererColliderCanvas(m_PlacementAugmentation, false);

        DeleteAnchors();
        m_ProductPlacement.SetProductAnchor(null);
        //AstronautIsPlaced = false;
        m_GroundPlaneUI.Reset();
        //SetGroundMode(true);
        
        m_TouchHandler.enableRotation = false;
        //SetPlacementMode(true);
    }

    public void ResetTrackers()
    {
        Debug.Log("ResetTrackers() called.");

        m_SmartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();
        m_PositionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();

        // Stop and restart trackers
        m_SmartTerrain.Stop(); // stop SmartTerrain tracker before PositionalDeviceTracker
        m_PositionalDeviceTracker.Reset();
        m_SmartTerrain.Start(); // start SmartTerrain tracker after PositionalDeviceTracker
    }

    #endregion // PUBLIC_BUTTON_METHODS



    #region PRIVATE_METHODS

    void DeleteAnchors()
    {
        //m_PlaneAnchor.UnConfigureAnchor();
        //m_MidAirAnchor.UnConfigureAnchor();
        m_PlacementAnchor.UnConfigureAnchor();
        //m_MidAirAnchor2.UnConfigureAnchor();
        AnchorExists = DoAnchorsExist();
    }

    void SetSurfaceIndicatorVisible(bool isVisible)
    {
        //Debug.Log("Is SurfaceIndicator Visible? " + isVisible);
        Renderer[] renderers = m_PlaneFinder.PlaneIndicator.GetComponentsInChildren<Renderer>(true);
        Canvas[] canvas = m_PlaneFinder.PlaneIndicator.GetComponentsInChildren<Canvas>(true);

        foreach (Canvas c in canvas)
            c.enabled = isVisible;

        foreach (Renderer r in renderers)
            r.enabled = isVisible;
    }

    bool DoAnchorsExist()
    {
        if (m_StateManager != null)
        {
            IEnumerable<TrackableBehaviour> trackableBehaviours = m_StateManager.GetActiveTrackableBehaviours();

            foreach (TrackableBehaviour behaviour in trackableBehaviours)
            {
                if (behaviour is AnchorBehaviour)
                {
                    return true;
                }
            }
        }
        return false;
    }

    #endregion // PRIVATE_METHODS


    #region VUFORIA_CALLBACKS

    void OnVuforiaStarted()
    {
        //Debug.Log("OnVuforiaStarted() called.");

        m_StateManager = TrackerManager.Instance.GetStateManager();

        // Check trackers to see if started and start if necessary
        m_PositionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();
        m_SmartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();

        if (m_PositionalDeviceTracker != null && m_SmartTerrain != null)
        {
            if (!m_PositionalDeviceTracker.IsActive)
                m_PositionalDeviceTracker.Start();
            if (m_PositionalDeviceTracker.IsActive && !m_SmartTerrain.IsActive)
                m_SmartTerrain.Start();
        }
        else
        {
            if (m_PositionalDeviceTracker == null)
                //Debug.Log("PositionalDeviceTracker returned null. GroundPlane not supported on this device.");
            if (m_SmartTerrain == null)
                //Debug.Log("SmartTerrain returned null. GroundPlane not supported on this device.");

            MessageBox.DisplayMessageBox(unsupportedDeviceTitle, unsupportedDeviceBody, false, null);
        }
    }

    void OnVuforiaPaused(bool paused)
    {
        Debug.Log("OnVuforiaPaused(" + paused.ToString() + ") called.");

        if (paused)
            ResetScene();
    }

    #endregion // VUFORIA_CALLBACKS


    #region DEVICE_TRACKER_CALLBACKS

    void OnTrackerStarted()
    {
        //Debug.Log("OnTrackerStarted() called.");

        m_PositionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();
        m_SmartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();

        if (m_PositionalDeviceTracker != null)
        {
            if (!m_PositionalDeviceTracker.IsActive)
                m_PositionalDeviceTracker.Start();

            //Debug.Log("PositionalDeviceTracker is Active?: " + m_PositionalDeviceTracker.IsActive +
            //          "\nSmartTerrain Tracker is Active?: " + m_SmartTerrain.IsActive);
        }
    }

    void OnDevicePoseStatusChanged(TrackableBehaviour.Status status, TrackableBehaviour.StatusInfo statusInfo)
    {
        //Debug.Log("OnDevicePoseStatusChanged(" + status + ", " + statusInfo + ")");
    }

    #endregion // DEVICE_TRACKER_CALLBACK_METHODS
}
