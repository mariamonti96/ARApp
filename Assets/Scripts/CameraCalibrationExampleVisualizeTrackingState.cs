using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCalibrationExampleVisualizeTrackingState : MonoBehaviour {


	public GameObject pattern;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void StoreTrackingStates(VLTrackingState state)
	{
		if (state.objects.Length==0) return;
		if (pattern == null) return;

		VLTrackingState.TrackingObject obj = state.objects[0];
		if (obj.state == "collectingActive"){
			pattern.SetActive(true);
		} else {
			pattern.SetActive(false);
		}
	}

	void OnEnable()
	{
		VLWorkerBehaviour.OnTrackingStates += StoreTrackingStates;
	}

	void OnDisable()
	{
		VLWorkerBehaviour.OnTrackingStates -= StoreTrackingStates;
	}

}
