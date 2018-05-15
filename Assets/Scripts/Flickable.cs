using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DraggableObject))]
public class Flickable : MonoBehaviour
{

    private InteractableObject interactableObject;
    private DraggableObject draggableObject;

    List<Vector2> lastPositions = new List<Vector2>();
    List<float> lastTime = new List<float>();
    public int flickTrackInterval = 10;

    public bool isActive = false;

    private Vector3 startPosition, endPosition;
    private float startTime, endTime;
    bool allow = false;
    int heldFrameCount = 0;


    // Use this for initialization
    void Start()
    {
        interactableObject = GetComponent<InteractableObject>();
        draggableObject = GetComponent<DraggableObject>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (isActive)
        {

            Vector3 dragPos = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2)).GetPoint(draggableObject.lockDistance);

            //interactableObject.isDragging = true;
            if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);
                if (GameControl.Instance.FlickBehaviour == GameControl.FlickMode.Final)
                {
                    dragPos = Camera.main.ScreenPointToRay(t.position).GetPoint(draggableObject.lockDistance);

                }
                heldFrameCount++;
                if (heldFrameCount > 20) allow = true;
                if (t.phase == TouchPhase.Began)
                {
                    startPosition = t.position;
                    startTime = Time.time;
                    Debug.Log("TOUCH BEGAN");

                }
                if (t.phase == TouchPhase.Ended)
                {
                    endPosition = t.position;
                    endTime = Time.time;
                    heldFrameCount = 0;
                    if (allow){
                        MakeThrow();
                        Debug.Log("THROW");
                    }
                        
                    allow = true;
                    Debug.Log("TOUCH END");
                }

            }

#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                startPosition = Input.mousePosition;
                startTime = Time.time;
            }
            if (Input.GetMouseButton(0))
            {
                if (GameControl.Instance.FlickBehaviour == GameControl.FlickMode.Final)
                    dragPos = Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(draggableObject.lockDistance);

                heldFrameCount++;
                if (heldFrameCount > 20) allow = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                endPosition = Input.mousePosition;
                endTime = Time.time;
                heldFrameCount = 0;
                if (allow)
                    MakeThrow();
                allow = true;
            }
#endif

            draggableObject.DragTo(dragPos);
        }




    }

    void MakeThrow()
    {
        Debug.Log(name + " start :" + startTime + " " + startPosition.ToString());
        Debug.Log(name + "end :" + endTime + " " + endPosition.ToString());
        //need to project 2 rays different distances away
        Ray startRay = Camera.main.ScreenPointToRay(startPosition);
        Ray endRay = Camera.main.ScreenPointToRay(endPosition);



        float power = Vector2.Distance(startPosition, endPosition);

        Vector3 endPos = endRay.GetPoint(power);

        //endPos.y += 200;

        Debug.DrawLine(startPosition, startRay.GetPoint(0), Color.green, 5f);
        Debug.DrawLine(endPosition, endRay.GetPoint(power), Color.red, 5f);
        Debug.DrawLine(startPosition, endRay.GetPoint(power), Color.blue, 5f);


        float throwTime = 0.25f;

#if UNITY_IOS
        throwTime = 0.5f;
        endPos.y += 100;
#endif
        Vector3 forceVector = (endPos - startRay.GetPoint(0f));
        Debug.Log(name + "power:" + power + "force :" + forceVector.ToString());

        if (draggableObject.isRagdoll)
        {
            //draggableObject.DragController.GetComponent<Rigidbody>().AddForce(forceVector, ForceMode.Impulse);
            draggableObject.Throw(forceVector, throwTime);
            //StartCoroutine(Throw(forceVector));
        }
        else
        {
            //draggableObject.DragController.transform
        }
        Debug.Log(name + " thrown");
        isActive = false;
    }

    IEnumerator Throw(Vector3 direction)
    {
        interactableObject.OnDragStart.Invoke();
        interactableObject.locked = true;
        interactableObject.selected = true;
        interactableObject.isDragging = true;

        float timeout = 0.25f;
        while (timeout > 0)
        {
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

    public void SetActive()
    {
        if (!isActive)
        {
            Debug.Log("FISH ACTIVATED!");
            isActive = true;
            StartCoroutine(Activate());
        }

    }

    IEnumerator Activate()
    {
        yield return new WaitForSeconds(0.1f);
        interactableObject.OnDragStart.Invoke();
        interactableObject.locked = true;
        interactableObject.selected = true;
        interactableObject.isDragging = true;
        Debug.Log("SERIOUSLY FISH ACTIVATED!");

    }
}
