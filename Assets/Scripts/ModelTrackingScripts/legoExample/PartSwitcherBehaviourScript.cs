using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** 
    This piece of code helps understanding how parts can be switched on and off.
    Here we switch through parts, which we asssume to have the same name as in the defined .vl file.
    Please checkout the legoExample folder in the StreamingAssets/VisionLib.

    SetModelProperty is the most important function.
*/

public class PartSwitcherBehaviourScript : MonoBehaviour {


    public int curStep=0;
    public string[] objects;
    public VLWorkerBehaviour worker;
    public GameObject father;
    public Material guidedMat;
    public Material placedMat;

    // Use this for initialization
    void Start () 
    {

    }


    private void prepareStep(int step)
    {
        if (step >= objects.Length){
            return;
        }
        for (int i = 0; i < objects.Length; i++) 
        {
            bool newEnabledState = false;
            bool showObject = false; // show object in GUI
            bool showGuide = false;  // show object in GUI with guide material or placed material
            if (i < step) 
            {
                // set
                newEnabledState = true;
                showObject = true;
            } else if (i == step) 
            {
                // show instructions
                showObject = true;
                showGuide = true;
            } 

            // We enable each object in the visionlib corresponding to the building state by setting the enabled attribute
            worker.SetModelProperty("name:"+objects[i],"enabled",newEnabledState);

            // We set the material and the visiblity for the user
            GameObject Go = this.father.transform.GetChild(i).gameObject;
            Go.transform.GetChild(0).gameObject.GetComponent<Renderer>().material = showGuide?guidedMat:placedMat;
            Go.SetActive(showObject);

        }

        // just for info purposes, retreive the current model properties (not needed)
        worker.GetModelProperties(); 


    }
    
    // Update is called once per frame
    void Update () 
    {
        
    }

    public void NextStep()
    {
        curStep++;
        if (curStep >= objects.Length) curStep = 0;
        prepareStep(curStep);

    }

    void LogTrackerInitialized(bool success)
    {
        // We start everything, when the tracker has been initialized.
        if (success) 
        {
            curStep = 0;
            prepareStep(curStep);
        }

    }

    void LogModelProperties(VLModelProperties[] properties)
    {
         Debug.Log("Got model properties: " + properties.Length);
         for (var i =0;i<properties.Length;i++)
         {
             Debug.Log("Model:"+i+" with name:"+properties[i].name+" is enabled:" + properties[i].enabled);

         }
    }
    
    void OnEnable()
    {
  
        VLWorkerBehaviour.OnGetModelProperties += LogModelProperties;
        VLWorkerBehaviour.OnTrackerInitialized += LogTrackerInitialized;
    }

    void OnDisable()
    {
        VLWorkerBehaviour.OnTrackerInitialized -= LogTrackerInitialized;
        VLWorkerBehaviour.OnGetModelProperties += LogModelProperties;
    }

    void OnGUI(){
          GUI.Label(new Rect(200,150,300,30), "Step:"+(curStep+1)+"/"+objects.Length);
    }
}
