using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using UnityEngine.Events;
using System.Linq;
using System;

public class InteractableObject : MonoBehaviour
{
    [Tooltip("if active the tap interactions are ignored")]
    public bool locked = false;
    public bool selected = false;
    public bool isDragging = false;
    protected float tapDownTime = 0;
    public Vector2 lastPos = new Vector2();

    int touchNumber = -1;

    float longtapTime = 1f;

    public UnityEvent OnTap;
    public UnityEvent OnLongTap;
    public UnityEvent OnDragStart;
    public UnityEvent OnDrag;
    public UnityEvent OnDragEnd;

    LayerMask layerMask;

    void Awake()
    {
        Debug.Log(name + " initialised");
        layerMask = LayerMask.GetMask("Default", "Actor", "Incedental", "Ragdoll");
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if (locked && GameControl.Instance.FlickBehaviour==GameControl.FlickMode.Tap) return;
        //#if PLATFORM_IOS
        if (Input.touchCount > 0)
        {
            Debug.Log("tapped");
            if (!selected)
            {
                touchNumber = Input.touchCount - 1;
            }
            var touch = Input.GetTouch(touchNumber);
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Ended)
            {
                Debug.Log("debug: touch began/end");
                RaycastHit hit;
                Ray screenPosition = Camera.main.ScreenPointToRay(touch.position);

                var screenPosition2 = Camera.main.ScreenToViewportPoint(touch.position);
                ARPoint point = new ARPoint
                {
                    x = screenPosition2.x,
                    y = screenPosition2.y
                };


                if (Physics.Raycast(screenPosition, out hit, 100, layerMask))
                {
                    bool isHit = false;
                    if (hit.transform == transform) isHit = true;
                    if (hit.transform.GetComponentInParent<DraggableObject>() != null)
                    {
                        if (hit.transform.GetComponentInParent<DraggableObject>().transform == transform)
                        {
                            isHit = true;
                        }
                    }
                    if (isHit)
                    {
                        //check for ended first
                        if (touch.phase == TouchPhase.Ended && selected)
                        {
                            if (tapDownTime > Time.time - longtapTime)
                            {
                                Debug.Log(name + " tapped!");

                                if (OnTap != null)
                                    OnTap.Invoke();
                            }
                            else
                            {
                                if (!isDragging)
                                {
                                    Debug.Log(name + " long tapped!");
                                    if (OnLongTap != null)
                                        OnLongTap.Invoke();
                                }
                            }
                            selected = false;
                        }
                        //set a flag that tapped, set time for handling long tap
                        if (touch.phase == TouchPhase.Began)
                        {
                            lastPos = touch.position;
                            selected = true;
                            tapDownTime = Time.time;
                        }
                    }
                }
                if (touch.phase == TouchPhase.Ended)
                {
                    if (isDragging)
                    {
                        Debug.Log(name + " ended drag");
                        if (OnDragEnd != null)
                        {
                            OnDragEnd.Invoke();
                        }
                    }
                    selected = false;
                    isDragging = false;
                    touchNumber = -1;
                }

            }
            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                if (selected)
                {
                    if (!isDragging && Vector2.Distance(lastPos, touch.position) > 5)
                    {
                        Debug.Log(name + " drag start");
                        isDragging = true;
                        if (OnDragStart != null)
                            OnDragStart.Invoke();
                    }
                    if (isDragging)
                    {
                        if (OnDrag != null)
                            OnDrag.Invoke();
                        Debug.Log(name + " is dragging...");
                        lastPos = touch.position;
                    }
                }
            }

        }
        //#endif
#if UNITY_EDITOR

        //check if object mousedown/up ON the object. If down - select!, if up - register a tap!
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            Ray screenPosition = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(screenPosition, out hit, 100, layerMask))
            {

                bool isHit = false;
                if (hit.transform == transform) isHit = true;
                if (hit.transform.GetComponentInParent<DraggableObject>() != null)
                {
                    if (hit.transform.GetComponentInParent<DraggableObject>().transform == transform)
                    {
                        isHit = true;
                    }
                }
                if (isHit)
                {
                    //check mouseup before down - just in case....
                    if (Input.GetMouseButtonUp(0) && selected)
                    {
                        if (tapDownTime > Time.time - longtapTime)
                        {
                            Debug.Log(name + " clicked!");

                            if (OnTap != null)
                                OnTap.Invoke();
                        }
                        else
                        {
                            if (!isDragging)
                            {
                                Debug.Log(name + " long clicked!");
                                if (OnLongTap != null)
                                    OnLongTap.Invoke();
                            }
                        }

                        selected = false;

                    }
                    //set a flag that tapped, set time for handling long tap
                    if (Input.GetMouseButtonDown(0))
                    {
                        lastPos = Input.mousePosition;
                        selected = true;
                        tapDownTime = Time.time;
                    }


                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (isDragging)
                {
                    Debug.Log(name + " ended drag");
                    if (OnDragEnd != null)
                    {
                        OnDragEnd.Invoke();
                    }
                }
                selected = false;
                isDragging = false;
            }

        }
        if (Input.GetMouseButton(0))
        {
            if (selected)
            {
                if (!isDragging && Vector2.Distance(lastPos, Input.mousePosition) > 5)
                {
                    Debug.Log(name + " drag start");
                    isDragging = true;
                    if (OnDragStart != null)
                        OnDragStart.Invoke();
                }

                if (isDragging)
                {
                    if (OnDrag != null)
                        OnDrag.Invoke();
                    //Debug.Log(name + " is dragging...");
                    lastPos = Input.mousePosition;
                }
            }


        }

#endif
    }



    public void Test()
    {
        Debug.Log(name);
    }


}
