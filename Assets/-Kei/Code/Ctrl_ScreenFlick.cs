using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ctrl_ScreenFlick : MonoBehaviour {

    
    public GameObject throwableItemPrefab;
    public Transform SpwanPoint;
    public float MinSwipDist = 0;
    public float upwardThrowAngle = 100.0f;
    public float throwSpeedMultiplier = 2.0f;
    public float maxStrength = 0.02f;
    public float touchFollowSmoothing = 0.3F;
    public float xAngleMultiplier = 4.0f;
    public float distancnFromScreen = 0.5f;
    public float movementRangeMultiplier = 1.0f;


    private GameObject throwableInstance;
    private float FlickTimer = 1.0f;
    private Vector2 startPress;
    private Vector2 endTouch;
    private Vector2 currentPos;
    private Vector2 lastFlickPos;
    private Vector3 angle;
    private Vector3 velocity = Vector3.zero;
    

    float swipdistance;
    float flickDistance;
    float flickDistanceX;
    float flickDistanceY;





    void Update()
        {
            if (Input.touchCount > 0)
                {
                    InputControls(Input.touches[0].position);
                }
            else
                {
                    MouseInputControls(Input.mousePosition);
                }
                
        }

    void InputControls (Vector2 inputPos)
        {
            Touch touch = Input.touches[0];
            Camera cam = Camera.main;

            if (touch.phase == TouchPhase.Moved)
                {
                    currentPos = Input.touches[0].position;
                }
            if (touch.phase == TouchPhase.Began )
                {
                    if(throwableInstance == null)
                        throwableInstance = Instantiate(throwableItemPrefab, SpwanPoint.position, Quaternion.identity);
                    throwableInstance.transform.GetChild(0).GetComponent<Rigidbody>().isKinematic = true;
                    startPress = touch.position;
                }

            if (touch.phase == TouchPhase.Ended)
                {
                    if(throwableInstance != null)
                    throwableInstance.transform.GetChild(0).GetComponent<Rigidbody>().isKinematic = false;
                    swipdistance = (touch.position - startPress).magnitude;
                    flickDistance = (Vector2.Distance(currentPos, lastFlickPos) * throwSpeedMultiplier) * -1;
                    flickDistanceX = (Mathf.Abs(currentPos.x - lastFlickPos.x) * throwSpeedMultiplier) * -1;
                    flickDistanceY = ((currentPos.y - lastFlickPos.y) * throwSpeedMultiplier) * -1;
                    flickDistance = flickDistance > maxStrength ? maxStrength : flickDistance;

                    if (swipdistance > MinSwipDist)
                        {
                            endTouch = touch.position;
                            MoveAngle();
                            throwableInstance.transform.GetChild(0).GetComponent<Rigidbody>().AddForce(new Vector3((angle.x * flickDistanceX * xAngleMultiplier), (angle.y * flickDistanceY), (angle.z * flickDistance)));
                            throwableInstance = null;
                        }
                }
            if (touch.phase == TouchPhase.Stationary)
                {
                    lastFlickPos = Input.touches[0].position;
                }

            if (throwableInstance != null)
                {
                    Vector3 tempPos = new Vector3();
                    tempPos = cam.ScreenToWorldPoint(new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, cam.nearClipPlane + distancnFromScreen));
                    throwableInstance.transform.GetChild(0).position = Vector3.SmoothDamp(throwableInstance.transform.GetChild(0).position, tempPos * movementRangeMultiplier, ref velocity, touchFollowSmoothing);
                }
                
        }

    public Vector3 mouseLastPos;
    public Vector3 delta;

    void MouseInputControls (Vector2 inputPos)
        {
            Camera cam = Camera.main;
        
            delta = Input.mousePosition - mouseLastPos;
            if (delta.magnitude > 0)
                {
                    currentPos = inputPos;
                }
            if (Input.GetMouseButtonDown(0))
                {
                    if(throwableInstance == null)
                        throwableInstance = Instantiate(throwableItemPrefab, SpwanPoint.position, Quaternion.identity);
                    throwableInstance.transform.GetChild(0).GetComponent<Rigidbody>().isKinematic = true;
                    startPress = inputPos;
                }

            if (Input.GetMouseButtonUp(0))
                {
                    if(throwableInstance != null)
                        throwableInstance.transform.GetChild(0).GetComponent<Rigidbody>().isKinematic = false;
                    swipdistance = (inputPos - startPress).magnitude;
                    flickDistance = (Vector2.Distance(currentPos, lastFlickPos) * throwSpeedMultiplier) * -1;
                    flickDistanceX = (Mathf.Abs(currentPos.x - lastFlickPos.x) * throwSpeedMultiplier) * -1;
                    flickDistanceY = ((currentPos.y - lastFlickPos.y) * throwSpeedMultiplier) * -1;
                    flickDistance = flickDistance > maxStrength ? maxStrength : flickDistance;

                    if (swipdistance > MinSwipDist)
                        {
                            endTouch = inputPos;
                            MoveAngle();
                            throwableInstance.transform.GetChild(0).GetComponent<Rigidbody>().AddForce(new Vector3((angle.x * flickDistanceX * xAngleMultiplier), (angle.y * flickDistanceY), (angle.z * flickDistance)));
                            throwableInstance = null;
                        }
                }

            if (delta.magnitude == 0)
                {
                    lastFlickPos = inputPos;
                }

            if (throwableInstance != null)
                {
                    Vector3 tempPos = new Vector3();
                    tempPos = cam.ScreenToWorldPoint(new Vector3(inputPos.x, inputPos.y, cam.nearClipPlane + distancnFromScreen));
                    throwableInstance.transform.GetChild(0).position = Vector3.SmoothDamp(throwableInstance.transform.GetChild(0).position, tempPos * movementRangeMultiplier, ref velocity, touchFollowSmoothing);
                }
            mouseLastPos = Input.mousePosition;
        }

    void MoveAngle()
        {
            angle = this.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(endTouch.x, endTouch.y + upwardThrowAngle, (this.GetComponent<Camera>().nearClipPlane - 50.0f)));
        }
}
