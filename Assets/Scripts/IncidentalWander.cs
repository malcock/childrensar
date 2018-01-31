using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncidentalWander : MonoBehaviour {

    Vector3 targetPosition;
    Vector3 oldPosition;
    Transform myTransform;

    public Collider containerCollider;

    public float timeout = 5;
    public bool lockY = true;
    public float speed = 0.5f;
    public float turningSpeed = 0.5f;

    float startY;

	// Use this for initialization
	void Start () {
        myTransform = transform;
        startY = myTransform.position.y;
        StartCoroutine(RandomTarget());
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 direction = targetPosition - myTransform.position;
        Vector3 rot = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
        myTransform.rotation = Quaternion.Lerp(myTransform.rotation, Quaternion.Euler(rot), Time.deltaTime * turningSpeed);
        myTransform.Translate(Vector3.forward * Time.deltaTime * speed);

	}


    IEnumerator RandomTarget(){
        while(isActiveAndEnabled){
            oldPosition = myTransform.position; 
            Bounds bounds = containerCollider.bounds;
            targetPosition = new Vector3(Random.Range(bounds.min.x, bounds.max.x),
                                      Random.Range(bounds.min.y, bounds.max.y),
                                      Random.Range(bounds.min.z, bounds.max.z));
            if (lockY) targetPosition.y = startY;

            yield return new WaitForSeconds(timeout);
        }
    }
}
