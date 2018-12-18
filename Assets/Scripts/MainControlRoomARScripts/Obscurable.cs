using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//When this component is added to a GameObject, the object can be occluded
//by any object with a material with the InvisibleMask shader
public class Obscurable : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Renderer[] renders = GetComponentsInChildren<Renderer>();
        foreach (Renderer rendr in renders)
        {
            rendr.material.renderQueue = 2002; // set their renderQueue
        }
    }

}
