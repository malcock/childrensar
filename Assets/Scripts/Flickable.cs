using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DraggableObject))]
public class Flickable : MonoBehaviour {

    private InteractableObject interactableObject;
    private DraggableObject draggableObject;

    List<Vector2> lastPositions = new List<Vector2>();
    public int flickTrackInterval = 10;

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
            //interactableObject.lastPos = new Vector2(Screen.width / 2, Screen.height / 2);
            lastPositions.Add(interactableObject.lastPos);
            if (lastPositions.Count > flickTrackInterval) lastPositions.RemoveAt(0);

            if(Input.GetMouseButtonDown(0)){
                startPosition = lastPositions[0];
                startTime = Time.time;
            }
            if(Input.GetMouseButtonUp(0)){
                endPosition = lastPositions[lastPositions.Count-1];
                endTime = Time.time;

                Debug.Log("start :" + startTime + " " + startPosition.ToString());
                Debug.Log("end :" + endTime + " " + endPosition.ToString());
                //need to project 2 rays different distances away
                Ray startRay = Camera.main.ScreenPointToRay(startPosition);
                Ray endRay = Camera.main.ScreenPointToRay(endPosition);



                float power = Vector2.Distance(startPosition,endPosition);

                Vector3 forceVector = (endRay.GetPoint(power) - startRay.GetPoint(0f));

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
        interactableObject.OnDragStart.Invoke();
        interactableObject.locked = true;
        interactableObject.selected = true;
        interactableObject.isDragging = true;

        float timeout = 0.25f;
        while(timeout>0){
            if (draggableObject.isRagdoll)
            {
                draggableObject.DragTo(direction);

            }
            Debug.Log("dragging to " + direction);
            timeout -= Time.deltaTime;
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
