using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractableObject))]
public class BucketDrop : MonoBehaviour {
    public enum ObjType { Ring, Salmon };
    public ObjType objType = ObjType.Salmon;
    public enum InteractionType { Flickable, Draggable, Catapult };
    public GameObject prefab;
    public InteractionType interaction = InteractionType.Flickable;
    public Vector3 dropOrigin = new Vector3(0, 0.5f, 0);
    public GameObject lastObj;
    bool waiting = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(lastObj == null){
            if(!waiting){
                StartCoroutine(Drop());
                waiting = true;
            }

        }else {
            if(lastObj.GetComponent<Flickable>().isActive){
                lastObj = null;
            }
        }
	}

    IEnumerator Drop(){
        yield return new WaitForSeconds(2);
        SpawnObject();
    }

    public void SpawnObject(){
        waiting = false;
        //Flickable[] flicks = FindObjectsOfType<Flickable>();

        //foreach(Flickable f in flicks){
        //    Debug.Log(f.name + " is active " + f.isActive);

        //    f.isActive = false;
        //    InteractableObject i = f.GetComponent<InteractableObject>();
        //    i.locked = false;
        //    i.selected = false;
        //    i.isDragging = false;
        //}
        Vector3 pos = transform.position + dropOrigin;
        pos.y += 0.075f;
        Quaternion rot = Quaternion.identity;
        if (objType == ObjType.Ring) rot = Quaternion.Euler(-90, 0, 0);
        GameObject obj = Instantiate(prefab,pos,rot) as GameObject;
        lastObj = obj;
        switch(interaction){
            case InteractionType.Flickable:
                Flickable flickable = obj.GetComponent<Flickable>();
                //flickable.SetActive();
                break;
            default:
                break;

        }
    }
}
