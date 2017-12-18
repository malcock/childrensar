using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Edible. 
/// Applied to an object that will be eaten by a creature - could it be a fish? or can I make it work for a ring toss too?
/// </summary>
public class Edible : MonoBehaviour {

    bool hasOwnCollider = false;

	// Use this for initialization
	void Start () {
        if(GetComponent<SimpleRagdoll>()!=null){
            foreach(RagdollBone bone in GetComponentsInChildren<RagdollBone>()){
                bone.onCollisionEnter.AddListener(EatMe);
                bone.onTriggerEnter.AddListener(InWater);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void InWater(Collider other, GameObject obj){
        if (other.tag == "Water")
        {
            Destroy(gameObject);
        }
    }

    void EatMe(Collision collision, GameObject other){
        Debug.Log(name + " eat?");
        Eater eater = collision.gameObject.GetComponent<Eater>();
        if(eater !=null){
            if (eater.OnFoodReceived != null) eater.OnFoodReceived.Invoke();
            Destroy(gameObject);
        }
    }


}
