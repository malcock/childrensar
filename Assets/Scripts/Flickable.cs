using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DraggableObject))]
public class Flickable : MonoBehaviour {

    private InteractableObject interactableObject;
    private DraggableObject draggableObject;

    List<Vector2> lastPositions = new List<Vector2>();
    List<float> lastTime = new List<float>();
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
	void LateUpdate () {
        if(isActive){
            //interactableObject.lastPos = new Vector2(Screen.width / 2, Screen.height / 2);
            lastPositions.Add(interactableObject.lastPos);
            lastTime.Add(Time.time);
            if (lastPositions.Count > flickTrackInterval)
            {
                lastPositions.RemoveAt(0);
                lastTime.RemoveAt(0);
            }

            if(Input.GetMouseButton(0)){
                startPosition = lastPositions[0];
                startTime = lastTime[0];
            }
            if(Input.GetMouseButtonUp(0)){
                endPosition = lastPositions[lastPositions.Count-1];
                endTime = lastTime[lastTime.Count-1];

                Debug.Log(name + " start :" + startTime + " " + startPosition.ToString());
                Debug.Log(name + "end :" + endTime + " " + endPosition.ToString());
                //need to project 2 rays different distances away
                Ray startRay = Camera.main.ScreenPointToRay(startPosition);
                Ray endRay = Camera.main.ScreenPointToRay(endPosition);



                float power = Vector2.Distance(startPosition,endPosition);

                Vector3 forceVector = (endRay.GetPoint(power) - startRay.GetPoint(0f));

                Debug.DrawLine(startPosition, startRay.GetPoint(0), Color.green, 5f);
                Debug.DrawLine(endPosition, endRay.GetPoint(power), Color.red, 5f);
                Debug.DrawLine(startPosition,endRay.GetPoint(power),Color.blue,5f);

                Debug.Log(name + "power:" + power + "force :" + forceVector.ToString());

                if(draggableObject.isRagdoll){
                    //draggableObject.DragController.GetComponent<Rigidbody>().AddForce(forceVector, ForceMode.Impulse);
                    draggableObject.Throw(forceVector,0.25f);
                    //StartCoroutine(Throw(forceVector));
                } else {
                    //draggableObject.DragController.transform
                }

                ////ok forget all that for now...
                //startPosition = lastPositions[0];
                //startTime = lastTime[0];
                //endPosition = lastPositions[lastPositions.Count - 1];
                //endTime = lastTime[lastTime.Count - 1];

                //float distance = Vector2.Distance(startPosition, endPosition);

                ////distance over time = speed
                //    float speed = distance/ (endTime - startTime);

                ////make a ray from the end position into the distance
                //Ray endingRay  = Camera.main.ScreenPointToRay(endPosition);

                ////make a ray from the start position to the end....
                //Ray direction = new Ray(startPosition, endingRay.GetPoint(distance));

                //Vector3 targetPosition = direction.GetPoint(speed);

                //Debug.Log(name + " targetPosition:" + targetPosition.ToString());

                //draggableObject.Throw(direction.GetPoint(speed),0.25f);
                isActive = false;



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

            //Debug.Log(name + "dragging to " + direction);
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
