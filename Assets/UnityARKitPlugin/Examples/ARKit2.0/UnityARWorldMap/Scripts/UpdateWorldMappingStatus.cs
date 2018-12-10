using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.iOS;

public class UpdateWorldMappingStatus : MonoBehaviour 
{

	public Text text;
	public Text tracking;
    public Transform TextPanel;
    public Transform TrackingPanel;

	// Use this for initialization
	void Start () 
	{
		UnityARSessionNativeInterface.ARFrameUpdatedEvent += CheckWorldMapStatus;
        Debug.Log("happening once");
	}

	void CheckWorldMapStatus(UnityARCamera cam)
	{
		text.text = cam.worldMappingStatus.ToString ();
		tracking.text = cam.trackingState.ToString () + " " + cam.trackingReason.ToString ();
        //Debug.Log("Happening once");
	}

    public IEnumerator ShowPopup()
    {
        
        TextPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(10);
        TextPanel.gameObject.SetActive(false);

    }

	void OnDestroy()
	{
		UnityARSessionNativeInterface.ARFrameUpdatedEvent -= CheckWorldMapStatus;
	}

}
