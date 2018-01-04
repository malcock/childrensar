using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BuoyancyToolkit.BuoyancyForce),typeof(ConstantForce),typeof(Rigidbody))]
public class FloatControl : MonoBehaviour
{
    public bool lockInPlace = true;
    Vector3 startPosition;
    ConstantForce constantForce;
    Transform myTransform;

    
	// Use this for initialization
	void Start()
	{
        startPosition = transform.position;
        constantForce = GetComponent<ConstantForce>();
        myTransform = transform;
	}

	// Update is called once per frame
	void Update()
	{
        if(lockInPlace){
            //try to move towards the start position
            if (Vector3.Distance(startPosition, myTransform.position) > 0.25f)
            {
                Vector3 direction = startPosition - myTransform.position;
                direction.y = 0;
                constantForce.force = direction * 10;
            }
            else
            {
                constantForce.force = new Vector3(-3.1f, 0, 1.74f);
            }
        } else {
            //move away from the play area if the octopus/bears are in play
            Vector3 direction = new Ray(Vector3.zero, transform.position).GetPoint(10);
            direction.y = 0;
            constantForce.force = direction;
        }

	}
}
