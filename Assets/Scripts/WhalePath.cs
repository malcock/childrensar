using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhalePath : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RandomRotation(){
        transform.parent.rotation = Quaternion.Euler(0, Random.value * 360, 0);
    }
}
