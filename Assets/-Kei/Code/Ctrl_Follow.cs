using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ctrl_Follow : MonoBehaviour {

    // Use this for initializ
    public GameObject fish;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        fish.transform.position = transform.position;
	}
}
