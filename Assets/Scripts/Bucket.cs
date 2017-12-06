using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractableObject))]
public class Bucket : MonoBehaviour {
    public enum InteractionType { Flickable, Draggable, Catapult };
    public GameObject prefab;
    public InteractionType interaction = InteractionType.Flickable;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SpawnObject(){
        Vector3 pos = transform.position;
        pos.y += 0.075f;
        GameObject obj = Instantiate(prefab,pos,Quaternion.identity) as GameObject;
        switch(interaction){
            case InteractionType.Flickable:
                Flickable flickable = obj.GetComponent<Flickable>();
                flickable.SetActive();
                break;
            default:
                break;

        }
    }
}
