using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Edible. 
/// Applied to an object that will be eaten by a creature - could it be a fish? or can I make it work for a ring toss too?
/// </summary>
[RequireComponent(typeof(InteractableObject))]
public class Edible : MonoBehaviour {
    bool hasThrown = false;
    bool hasOwnCollider = false;
    InteractableObject interactableObj;
	// Use this for initialization
	void Start () {
        if(GetComponent<SimpleRagdoll>()!=null){
            foreach(RagBone bone in GetComponentsInChildren<RagBone>()){
                bone.onCollisionEnter.AddListener(EatMe);
                bone.onTriggerEnter.AddListener(InWater);
            }

        }
        interactableObj = GetComponent<InteractableObject>();

        interactableObj.OnDragStart.AddListener(DragStart);
        interactableObj.OnDrag.AddListener(Drag);
        interactableObj.OnDragEnd.AddListener(DragEnd);
	}
	
    void DragStart(){
        AkSoundEngine.PostEvent("InteractFishPickup",gameObject);
        AkSoundEngine.PostEvent("InteractFishPickupLoop",gameObject);
    }

    void Drag(){
        
    }

    void DragEnd(){
        if(!hasThrown){
            AkSoundEngine.PostEvent("InteractFishPickupLoopStop", gameObject);
            AkSoundEngine.PostEvent("InteractFishThrow", gameObject);
            hasThrown = true;
        }

        StartCoroutine(timeout());
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
            Destroy(gameObject,0.075f);
        }
    }

    IEnumerator timeout(){
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }
}
