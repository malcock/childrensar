using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ctrl_ScreenFlickThrow : MonoBehaviour
{


    public Vector2 touchStart;
    public Vector2 touchEnd;
    public int flickTime = 5;
    public int flickLength = 0;
    public float ballVelocity = 0.0f;
    public float ballSpeed = 0;
    public Vector3 worldAngle;
    public GameObject ballPrefab;
    float swipeDist;

    private bool GetVelocity = false;
    //public GameObject[] woosh; //no
    //    public AudioClip ballAudio;  //yes
    public float comfortZone = 0.0f;
    public bool couldBeSwipe;
    public float startCountdownLength = 0.0f;
    public bool startTheTimer = false;
    static bool globalGameStart = false;
    static bool shootEnable = false;
    private float startGameTimer = 0.0f;

    private AudioSource asParamsControl;

    void Start()
    {
        //asParamsControl = this.gameObject.GetComponentInChildren<AudioSource>();
        //    this.asParamsControl.playOnAwake = false;
        //    this.asParamsControl.loop = false;
        startTheTimer = true;
        Time.timeScale = 1;
        if (Application.isEditor)
        {
            Time.fixedDeltaTime = 0.01f;
        }
    }

    void Update()
    {
        if (startTheTimer)
        {
            startGameTimer += Time.deltaTime;
        }
        if (startGameTimer > startCountdownLength)
        {
            globalGameStart = true;
            shootEnable = true;
            startTheTimer = false;
            startGameTimer = 0;
        }

        if (shootEnable)
        {
            Debug.Log("enabled!");
            if (Input.touchCount > 0)
            {
                var touch = Input.touches[0];
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        flickTime = 5;
                        timeIncrease();
                        couldBeSwipe = true;
                        GetVelocity = true;
                        touchStart = touch.position;
                        break;
                    case TouchPhase.Moved:
                        if (Mathf.Abs(touch.position.y - touchStart.y) < comfortZone)
                        {
                            couldBeSwipe = false;
                        }
                        else
                        {
                            couldBeSwipe = true;
                        }
                        break;
                    case TouchPhase.Stationary:
                        if (Mathf.Abs(touch.position.y - touchStart.y) < comfortZone)
                        {
                            couldBeSwipe = false;
                        }
                        break;
                    case TouchPhase.Ended:
                        float swipeDist = (touch.position - touchStart).magnitude;
                        //couldBeSwipe
                        if (couldBeSwipe || swipeDist > comfortZone)
                        {
                            GetVelocity = false;
                            touchEnd = touch.position;
                            GameObject ball = Instantiate(ballPrefab, new Vector3(0.0f, 0, -3.0f), Quaternion.identity) as GameObject;
                            GetSpeed();
                            GetAngle();
                            ball.GetComponent<Rigidbody>().AddForce(new Vector3((worldAngle.x * ballSpeed), (worldAngle.y * ballSpeed), (worldAngle.z * ballSpeed)));
                            //PlayWhoosh();
                            //audio.PlayOneShot(ballAudio);   //yes play an audio clip once pre instantiation

                        }
                        break;
                    default:
                        break;
                        break;

                }//end switch case
                if (GetVelocity)
                {
                    flickTime++;
                }
            }
        }
        if (!shootEnable)
        {
            Debug.Log("shot disabled!");
        }
    }

    void timeIncrease()
    {
        if (GetVelocity)
        {
            flickTime++;
        }
    }

    void GetSpeed()
    {
        flickLength = 90;
        if (flickTime > 0)
        {
            ballVelocity = flickLength / (flickLength - flickTime);
        }
        ballSpeed = ballVelocity * 30;
        ballSpeed = ballSpeed - (ballSpeed * 1.65f);
        if (ballSpeed <= -33)
        {
            ballSpeed = -33;
        }
        Debug.Log("flick was" + flickTime);
        flickTime = 5;
    }

    void GetAngle()
    {
        worldAngle = this.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(touchEnd.x, touchEnd.y + 50f, ((this.GetComponent<Camera>().nearClipPlane - 50.0f))));
    }
    //no
    //  void PlayWhoosh()
    //  {
    //      GameObject sound = Instantiate(whoosh[Random.Range(0,whoosh.length)],transform.position,transform.rotation) as GameObject;
    //      Debug.Log("Whoosh!");
    //  }
}
