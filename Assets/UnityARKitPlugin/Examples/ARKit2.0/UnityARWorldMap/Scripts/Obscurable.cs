using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
