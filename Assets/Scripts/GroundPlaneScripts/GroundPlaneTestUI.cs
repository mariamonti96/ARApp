/*==============================================================================
Copyright (c) 2018 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
==============================================================================*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GroundPlaneTestUI : MonoBehaviour
{
    #region PUBLIC_MEMBERS
    [Header("UI Elements")]
    //public Text m_Title;
    public Text m_TrackerStatus;
    //public Text m_Instructions;
    public CanvasGroup m_ScreenReticle;

    [Header("UI Buttons")]
    public Button m_ResetButton;
    //public Toggle m_PlacementToggle, m_GroundToggle, m_MidAirToggle, m_MidAirToggle2;
    #endregion // PUBLIC_MEMBERS


    #region PRIVATE_MEMBERS
    //const string TITLE_PLACEMENT = "Product Placement";
    //const string TITLE_GROUNDPLANE = "Ground Plane";
    //const string TITLE_MIDAIR = "Mid-Air";
    //const string TITLE_MIDAIR2 = "Mid-Air Spacecraft";

    GraphicRaycaster m_GraphicRayCaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    ProductPlacement m_ProductPlacement;
    TouchHandlerTest m_TouchHandler;

    Image m_TrackerStatusImage;
    #endregion // PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS
    void Start()
    {
        //m_ResetButton.interactable = m_MidAirToggle.interactable =
        //m_GroundToggle.interactable = m_PlacementToggle.interactable = 
        //        m_MidAirToggle2.interactable = false;
        m_ResetButton.interactable = false;

        //m_Title.text = TITLE_PLACEMENT;
        m_TrackerStatus.text = "";
        m_TrackerStatusImage = m_TrackerStatus.GetComponentInParent<Image>();

        m_ProductPlacement = FindObjectOfType<ProductPlacement>();
        m_TouchHandler = FindObjectOfType<TouchHandlerTest>();

        m_GraphicRayCaster = FindObjectOfType<GraphicRaycaster>();
        m_EventSystem = FindObjectOfType<EventSystem>();

        Vuforia.DeviceTrackerARController.Instance.RegisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);
    }

    void Update()
    {
        //if (PlaneManagerTest.AstronautIsPlaced)
        //{
        //    m_ResetButton.interactable = true;

        //}

        if (m_ProductPlacement.IsPlaced)
        {
            m_ResetButton.interactable = true;
        }

        m_TrackerStatusImage.enabled = !string.IsNullOrEmpty(m_TrackerStatus.text);
    }

    void LateUpdate()
    {
        if (PlaneManagerTest.GroundPlaneHitReceived)
        {
            // We got an automatic hit test this frame

            // Hide the onscreen reticle when we get a hit test
            m_ScreenReticle.alpha = 0;

            //m_Instructions.transform.parent.gameObject.SetActive(true);
            //m_Instructions.enabled = true;

            //if (PlaneManager.planeMode == PlaneManager.PlaneMode.GROUND)
            //{
            //    m_Instructions.text = "Tap to place Astronaut";
            //}
            //else if (PlaneManager.planeMode == PlaneManager.PlaneMode.PLACEMENT)
            //{
            //    m_Instructions.text = (m_ProductPlacement.IsPlaced) ?
            //        "• Touch and drag to move Chair" +
            //        "\n• Two fingers to rotate" +
            //        ((m_TouchHandler.enablePinchScaling) ? " or pinch to scale" : "") +
            //        "\n• Double-tap to reset Anchor location"
            //        :
            //        "Tap to place Chair";
            //}
        }
        else
        {
            // No automatic hit test, so set alpha based on which plane mode is active
            //m_ScreenReticle.alpha =
            //(PlaneManager.planeMode == PlaneManager.PlaneMode.GROUND ||
            //PlaneManager.planeMode == PlaneManager.PlaneMode.PLACEMENT) ? 1 : 0;

            //If GROUND
            //m_ScreenReticle.alpha = 1;
            //If PLACEMENT
            m_ScreenReticle.alpha = 0;

            //m_Instructions.transform.parent.gameObject.SetActive(true);
            //m_Instructions.enabled = true;

            //if (PlaneManager.planeMode == PlaneManager.PlaneMode.GROUND ||
            //    PlaneManager.planeMode == PlaneManager.PlaneMode.PLACEMENT)
            //{
            //    m_Instructions.text = "Point device towards ground";
            //}
            //else if (PlaneManager.planeMode == PlaneManager.PlaneMode.MIDAIR)
            //{
            //    m_Instructions.text = "Tap to place Drone";
            //}
            //else if (PlaneManager.planeMode == PlaneManager.PlaneMode.MIDAIR2){
            //    m_Instructions.text = "Tap to place Spacecraft";
            //}
        }

        
    }

    void OnDestroy()
    {
        Debug.Log("OnDestroy() called.");

        Vuforia.DeviceTrackerARController.Instance.UnregisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);
    }
    #endregion // MONOBEHAVIOUR_METHODS


    #region PUBLIC_METHODS
    public void Reset()
    {
        Debug.Log("Reset() called");
        //m_ResetButton.interactable = m_MidAirToggle.interactable = false;
        m_ResetButton.interactable = false;
        //PlaneManagerTest.SetGroundMode();
        


        //m_PlacementToggle.isOn = true;
    }

    //public void UpdateTitle()
    //{
    //    switch (PlaneManager.planeMode)
    //    {
    //        case PlaneManager.PlaneMode.GROUND:
    //            m_Title.text = TITLE_GROUNDPLANE;
    //            break;
    //        case PlaneManager.PlaneMode.MIDAIR:
    //            m_Title.text = TITLE_MIDAIR;
    //            break;
    //        case PlaneManager.PlaneMode.PLACEMENT:
    //            m_Title.text = TITLE_PLACEMENT;
    //            break;
    //        case PlaneManager.PlaneMode.MIDAIR2:
    //            m_Title.text = TITLE_MIDAIR2;
    //            break;
    //    }
    //}

    public bool InitializeUI()
    {
        // Runs only once after first successful Automatic hit test
        //m_PlacementToggle.interactable = true;
        //m_GroundToggle.interactable = true;

        if (Vuforia.VuforiaRuntimeUtilities.IsPlayMode())
        {
        //    m_MidAirToggle.interactable = true;
        //    m_MidAirToggle2.interactable = true;
            m_ResetButton.interactable = true;
        }
        //m_MidAirToggle.interactable = true;
        //m_MidAirToggle2.interactable = true;
        m_ResetButton.interactable = true;

        // Make the PlacementToggle active
        //m_PlacementToggle.isOn = true;

        return true;
    }

    public bool IsCanvasButtonPressed()
    {
        m_PointerEventData = new PointerEventData(m_EventSystem)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        m_GraphicRayCaster.Raycast(m_PointerEventData, results);

        bool resultIsButton = false;
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponentInParent<Toggle>() ||
                result.gameObject.GetComponent<Button>())
            {
                resultIsButton = true;
                break;
            }
        }
        return resultIsButton;
    }
    #endregion // PUBLIC_METHODS


    #region VUFORIA_CALLBACKS
    void OnDevicePoseStatusChanged(Vuforia.TrackableBehaviour.Status status, Vuforia.TrackableBehaviour.StatusInfo statusInfo)
    {
        //Debug.Log("OnDevicePoseStatusChanged(" + status + ", " + statusInfo + ")");

        switch (statusInfo)
        {
            case Vuforia.TrackableBehaviour.StatusInfo.INITIALIZING:

                if (Vuforia.VuforiaRuntimeUtilities.GetActiveFusionProvider() == 
                    Vuforia.FusionProviderType.PLATFORM_SENSOR_FUSION)
                {
                    m_TrackerStatus.text = "Initializing Tracker";
                }
                else
                {
                    m_TrackerStatus.text = "Waiting for anchor to be placed to initialize";
                }
                
                break;
            case Vuforia.TrackableBehaviour.StatusInfo.EXCESSIVE_MOTION:
                m_TrackerStatus.text = "Move slower";
                break;
            case Vuforia.TrackableBehaviour.StatusInfo.INSUFFICIENT_FEATURES:
                m_TrackerStatus.text = "Not enough visual features in the scene";
                break;
            default:
                m_TrackerStatus.text = "";
                break;
        }

    }
    #endregion // VUFORIA_CALLBACKS


}
