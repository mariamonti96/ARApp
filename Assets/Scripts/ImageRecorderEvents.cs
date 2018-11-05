using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageRecorderEvents : MonoBehaviour {

	public Text textComponent;
	private string trackingStates = "";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (this.textComponent != null)
		{
			this.textComponent.text =
				trackingStates;
		}
	}

	void Awake()
	{
	}


	void StoreTrackingStates(VLTrackingState state)
	{
		if (state.objects.Length>0){
			VLTrackingState.TrackingObject obj = state.objects[0];
			trackingStates = obj.state+ " @Frame ("+obj._NumberOfTemplates+")";
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
