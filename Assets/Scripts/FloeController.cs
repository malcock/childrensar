using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ConstantForce))]
public class FloeController : MonoBehaviour {

    Vector3 startPosition;
    ConstantForce constantForce;
    Transform myTransform;
	// Use this for initialization
	void Start () {
        startPosition = transform.position;
        constantForce = GetComponent<ConstantForce>();
        myTransform = transform;
	}
	
	// Update is called once per frame
	void Update () {
        //try to move towards the start position
        if(Vector3.Distance(startPosition,myTransform.position)>0.25f){
            Vector3 direction = startPosition - myTransform.position;
            direction.y = 0;
            constantForce.force = direction*10;
        } else {
            constantForce.force = new Vector3(-3.1f, 0, 1.74f);
        }

    }
}
