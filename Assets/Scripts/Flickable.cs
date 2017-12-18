using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DraggableObject))]
public class Flickable : MonoBehaviour {

    private InteractableObject interactableObject;
    private DraggableObject draggableObject;

    public bool isActive = false;

    private Vector3 startPosition, endPosition;
    private float startTime, endTime;

	// Use this for initialization
	void Start () {
        interactableObject = GetComponent<InteractableObject>();
        draggableObject = GetComponent<DraggableObject>();
	}
	
	// Update is called once per frame
	void Update () {
        if(isActive){
            interactableObject.lastPos = new Vector2(Screen.width / 2, Screen.height / 2);


            if(Input.GetMouseButtonDown(0)){
                startPosition = Input.mousePosition;
                startTime = Time.time;
            }
            if(Input.GetMouseButtonUp(0)){
                endPosition = Input.mousePosition;
                endTime = Time.time;

                Debug.Log("start :" + startTime + " " + startPosition.ToString());
                Debug.Log("end :" + endTime + " " + endPosition.ToString());
                //need to project 2 rays different distances away
                Ray startRay = Camera.main.ScreenPointToRay(startPosition);
                Ray endRay = Camera.main.ScreenPointToRay(endPosition);



                float power = (endTime - startTime);

                Vector3 forceVector = (endRay.GetPoint(power) - startRay.GetPoint(0f))*10;

                Debug.Log("force :" + forceVector.ToString());

                if(draggableObject.isRagdoll){
                    //draggableObject.DragController.GetComponent<Rigidbody>().AddForce(forceVector, ForceMode.Impulse);
                    StartCoroutine(Throw(forceVector));
                } else {
                    //draggableObject.DragController.transform
                }




            }

        }
	}

    IEnumerator Throw(Vector3 direction){
        for (int t = 0; t < 10;t++){
            if(draggableObject.isRagdoll){
                draggableObject.DragTo(direction);

            }
            yield return null;
        }   

        isActive = false;
        interactableObject.locked = false;
        interactableObject.selected = false;
        interactableObject.isDragging = false;
    }

    public void SetActive(){
        if(!isActive){
            isActive = true;
            StartCoroutine(Activate());    
        }

    }

    IEnumerator Activate(){
        yield return new WaitForSeconds(0.1f);
        interactableObject.OnDragStart.Invoke();
        interactableObject.locked = true;
        interactableObject.selected = true;
        interactableObject.isDragging = true;

    }
}
