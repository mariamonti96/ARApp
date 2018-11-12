/*============================================================================== 
Copyright (c) 2018 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.   
==============================================================================*/

using UnityEngine;
using Vuforia;

public class BEPIProductPlacement : MonoBehaviour
{

    #region PUBLIC_MEMBERS
    public bool IsPlaced { get; private set; }

    [Header("Placement Controls")]
    public GameObject m_TranslationIndicator;
    public GameObject m_RotationIndicator;
    public Transform Floor;

    [Header("Placement Augmentation Size Range")]
    [Range(0.1f, 2.0f)]
    public float ProductSize = 0.01f;
    #endregion // PUBLIC_MEMBERS


    #region PRIVATE_MEMBERS
    Material objectMaterial;
    //Material ChairShadow, ChairShadowTransparent;
    MeshRenderer objectRenderer;
    //[SerializeField]
    //MeshRenderer shadowRenderer;

    const string EmulatorGroundPlane = "Emulator Ground Plane";

    GroundPlaneTestUI m_GroundPlaneUI;
    Camera mainCamera;
    Ray cameraToPlaneRay;
    RaycastHit cameraToPlaneHit;

    float m_PlacementAugmentationScale;
    Vector3 ProductScaleVector;
    #endregion // PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS
    void Start()
    {

        
        objectRenderer = GetComponent<MeshRenderer>();

        objectMaterial = Resources.Load<Material>("defaultMat");
        
        //chairMaterialsTransparent = new Material[]
        //{
        //    Resources.Load<Material>("ChairBodyTransparent"),
        //    Resources.Load<Material>("ChairFrameTransparent")
        //};

        //ChairShadow = Resources.Load<Material>("ChairShadow");
        //ChairShadowTransparent = Resources.Load<Material>("ChairShadowTransparent");

        m_GroundPlaneUI = FindObjectOfType<GroundPlaneTestUI>();

        // Enable floor collider if running on device; Disable if running in PlayMode
        Floor.gameObject.SetActive(!VuforiaRuntimeUtilities.IsPlayMode());


        mainCamera = Camera.main;

        //m_PlacementAugmentationScale = VuforiaRuntimeUtilities.IsPlayMode() ? 0.1f : ProductSize;

        //ProductScaleVector =
        //    new Vector3(m_PlacementAugmentationScale,
        //                m_PlacementAugmentationScale,
        //                m_PlacementAugmentationScale);

        //gameObject.transform.localScale = ProductScaleVector;
    }


    void Update()
    {

       
            objectRenderer.enabled = (IsPlaced);
          
        //EnablePreviewModeTransparency(!IsPlaced);
        //if (!IsPlaced)
        //    UtilityHelperRes.RotateTowardCamera(gameObject);
        //}
        //else
        //{
        //    shadowRenderer.enabled = chairRenderer.enabled = IsPlaced;
        //}
        
        if (PlaneManagerTest.planeMode == PlaneManagerTest.PlaneMode.BEPI && IsPlaced)
        {
            
            m_RotationIndicator.SetActive(Input.touchCount == 2);
            Debug.Log("Input.touchCount is " + Input.touchCount);
            Debug.Log("Is RotationIndicator active?" + m_RotationIndicator.activeSelf);
            m_TranslationIndicator.SetActive(
                (BEPITouchHandlerTest.IsSingleFingerDragging || BEPITouchHandlerTest.IsSingleFingerStationary) && !m_GroundPlaneUI.IsCanvasButtonPressed());

            if (BEPITouchHandlerTest.IsSingleFingerDragging || (VuforiaRuntimeUtilities.IsPlayMode() && Input.GetMouseButton(0)))
            {
                if (!m_GroundPlaneUI.IsCanvasButtonPressed())
                {
                    cameraToPlaneRay = mainCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(cameraToPlaneRay, out cameraToPlaneHit))
                    {
                        if (cameraToPlaneHit.collider.gameObject.name ==
                            (VuforiaRuntimeUtilities.IsPlayMode() ? EmulatorGroundPlane : Floor.name))
                        {
                            gameObject.PositionAt(cameraToPlaneHit.point);
                        }
                    }
                }
            }
        }
        else
        {
            
            m_RotationIndicator.SetActive(false);
           
            m_TranslationIndicator.SetActive(false);
        }

    }
    #endregion // MONOBEHAVIOUR_METHODS


    #region PUBLIC_METHODS
    public void Reset()
    {
        transform.position = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        //transform.localScale = ProductScaleVector;
    }

    public void SetProductAnchor(Transform transform)
    {
        if (transform)
        {
            Debug.Log("BEPI is placed");
            IsPlaced = true;
            gameObject.transform.SetParent(transform);
            gameObject.transform.localPosition = Vector3.zero;
            UtilityHelperTest.RotateTowardCamera(gameObject);
        }
        else
        {
            IsPlaced = false;
            gameObject.transform.SetParent(null);
        }
    }
    #endregion // PUBLIC_METHODS


    #region PRIVATE_METHODS
    //void EnablePreviewModeTransparency(bool previewEnabled)
    //{
    //    if (!previewEnabled)
    //    {
    //        objectRenderer.material = objectMaterial;
    //    }
    //    //shadowRenderer.material = previewEnabled ? ChairShadowTransparent : ChairShadow;
    //}
    #endregion // PRIVATE_METHODS

}
