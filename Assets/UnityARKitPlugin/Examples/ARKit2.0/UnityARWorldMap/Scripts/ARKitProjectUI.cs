using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ARKitProjectUI : MonoBehaviour
{

    #region PUBLIC_MEMBERS
    [Header("UI Buttons")]
    public Toggle m_INVToggle;
    public Toggle m_DELToggle;
    public Toggle m_OB1Toggle;
    public Toggle m_OB2Toggle;

    #endregion //PUBLIC_MEMBERS

    //#region PRIVATE_MEMBERS
    //#endregion //PRIVATE_MEMBERS
    PointerEventData m_PointerEventData;
    PointerEventData m_PointerEventDataNew;
    EventSystem m_EventSystem;
    GraphicRaycaster m_GraphicRaycaster;


    // Use this for initialization
    void Start()
    {
        m_OB1Toggle.interactable = true;
        m_OB1Toggle.isOn = true;

        m_INVToggle.interactable = true;
        m_INVToggle.isOn = false;

        m_DELToggle.interactable = true;
        m_DELToggle.isOn = false;

        m_OB2Toggle.interactable = true;
        m_OB2Toggle.isOn = false;

        m_EventSystem = FindObjectOfType<EventSystem>();
        m_GraphicRaycaster = FindObjectOfType<GraphicRaycaster>();



    }

    // Update is called once per frame
    void Update()
    {
        // When to enable the toggle?

        //Check if ObjectPlacement.isPlaced is true. If it is then enable the reset button.
    }

    #region PUBLIC_METHODS
    public void Reset()
    {
        Debug.Log("Reset() called");

        //When to reset? 
        //When reset button is clicked!!

    }

    //public bool InitializeUI()
    //{
    //    m_TGOToggle.interactable = true;

    //    //Should you turn the toggle on? Which one? 
    //    m_TGOToggle.isOn = true;
    //    return true;
    //}

    //maybe add "public bool IsCanvasButtonPressed" to make sure that the objects are not moved when pressing a canvas button?

    public bool IsCanvasButtonPressed()
    {
        m_PointerEventData = new PointerEventData(m_EventSystem)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        m_GraphicRaycaster.Raycast(m_PointerEventData, results);

        bool resultIsButton = false;
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponentInParent<Toggle>() ||
                result.gameObject.GetComponent<Button>() ||
                result.gameObject.GetComponent<InputField>())
            {
                resultIsButton = true;
               
                break;
            }
        }
        return resultIsButton;
    }

    public GameObject GetGameObjectPressed()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 30))
        {
            if (hit.collider.gameObject.GetComponent<ObjectPlacement>() || hit.collider.gameObject.GetComponentInChildren<ObjectPlacement>() || hit.collider.gameObject.GetComponentInParent < ObjectPlacement>())
            {
                Debug.Log("The gameobject has component ObjectPlacement");
                return hit.collider.gameObject;
            }
            
        }
        Debug.Log("No ObjectPlacement");
        return null;
        //Debug.Log("I AM INSIDE GET GAMEOBJECT PRESSED");
        //m_PointerEventDataNew = new PointerEventData(m_EventSystem)
        //{
        //    position = Input.mousePosition
        //};
        //List<RaycastResult> resultsNew = new List<RaycastResult>();
        //m_GraphicRaycaster.Raycast(m_PointerEventDataNew, resultsNew);

        //Debug.Log("bla" + resultsNew);

        //foreach (RaycastResult result in resultsNew)
        //{
        //    if (result.gameObject.GetComponent<ObjectPlacement>())
        //    {
        //        Debug.Log("The Game Object has component ObjectPlacement");
        //        return result.gameObject;
                
        //    }
        //    Debug.Log("The Game Object DOES NOT have component ObjectPlacement");
        //}
        ////Debug.Log("No RayCastResult list?");
        //return null;
    }

    
    #endregion //PUBLIC_MEMBERS

}
