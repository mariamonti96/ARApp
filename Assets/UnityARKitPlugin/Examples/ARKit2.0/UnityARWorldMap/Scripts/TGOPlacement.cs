using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TGOPlacement : MonoBehaviour
{

    #region PUBLIC_MEMBERS
    public bool IsPlaced { get; private set; }

    #endregion //PUBLIC_MEMBERS

    #region PRIVATE_MEMBERS
    //Material objectMaterial;
    MeshRenderer objectRenderer;

    ARKitProjectUI m_ARKitProjectUI;

    #endregion //PRIVATE_MEMBERS

    #region MONOBEHAVIOUR_METHODS
    // Use this for initialization
    void Start()
    {

        objectRenderer = GetComponent<MeshRenderer>();
        //objectMaterial = Resources.Load<objectMaterial>("defaultMat");

        m_ARKitProjectUI = FindObjectOfType<ARKitProjectUI>();

    }

    // Update is called once per frame
    void Update()
    {
        objectRenderer.enabled = IsPlaced;

    }

    #endregion //MONOBEHAVIOUR_METHODS

    #region PUBLIC_METHODS
    public void Reset()
    {
        transform.position = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        IsPlaced = false;

    }

    public void placeObject(Vector3 position, Quaternion rotation)
    {
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
        IsPlaced = true;

    }
    #endregion //PUBLIC_METHODS

}
