using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PolarController : MonoBehaviour {

    public string AttractToLoad = "Polar Attract 1";

    public float penguinTime = 240, octopusTime = 240, changeover = 5, fadeTime=3;

    public float timeout;
    public CanvasGroup fadeGroup;

    float waterLevel;

    bool putDown = false;
    bool hasStarted = false;

    public enum State {Attract, Begin, Penguins, Escape, FloatDrop, Octopus, Return }
    [SerializeField]
    private State _state = State.Attract;
    public State state {
        get { return _state; }
        set {
            _state = value;
            switch(_state){
                case State.Attract:
                    incidentals.SetActive(false);
                    VideoPlayer videoPlayer = FindObjectOfType<VideoPlayer>();
                    string url = "file://" + Application.streamingAssetsPath + "/" + "attract-screen.mp4";

#if !UNITY_EDITOR
                        url = Application.streamingAssetsPath + "/" + "attract-screen.mp4";
#endif

                    //We want to play from url
                    videoPlayer.source = VideoSource.Url;
                    videoPlayer.url = url;
                    break;
                case State.Begin:
                    
                    Vector3 pos = water.position;
                    waterLevel = pos.y;
                    pos.y = -1.68f;
                    StartCoroutine(FadeIn());
                    StartCoroutine(Begin());
                    break;
                case State.Penguins:
                    Ring[] rings = FindObjectsOfType<Ring>();
                    foreach (Ring r in rings)
                    {
                        r.DisappearRing();
                    }
                    StartCoroutine(SwitchObjects(false));
                    for (int p = 0; p < penguins.Count; p++)
                    {
                        penguins[p].isReady = true;
                    }

                    whale.OctoOut = false;
                    timeout = penguinTime;
                    break;
                case State.Escape:
                    

                    whale.OctoOut = true;
                    timeout = changeover;
                    break;
                case State.FloatDrop:
                    timeout = changeover;
                    break;
                case State.Return:
                    
                    //if (octopus.state != OctopusController.State.End) octopus.StopGame();
                    whale.OctoOut = false;
                    timeout = changeover;
                    break;
                case State.Octopus:


                    StartCoroutine(SwitchObjects(true));

                    whale.OctoOut = true;
                    timeout = octopusTime;
                    octopus.BeginGame();
                    break;
            }
        }
    }
    public Transform water;
    public List<MainCharacter> penguins = new List<MainCharacter>();
    public List<FloatControl> floats = new List<FloatControl>();
    public GameObject bucket, hoops;
    public ParticleController flickSwapParticles;
    public OctopusController octopus;
    public WhalePath whale;
    public GameObject incidentals;

	// Use this for initialization
	void Start () {
        fadeGroup.interactable = true;
        fadeGroup.blocksRaycasts = true;
        AkSoundEngine.PostEvent("PolarAmbience", gameObject);
        //fadeGroup.alpha = 1;
        StartCoroutine(LevelStart());
        state = State.Attract;


        penguinTime = GameControl.Instance.gametimeA;
        octopusTime = GameControl.Instance.gametimeB;

        penguins = FindObjectsOfType<MainCharacter>().ToList();
        floats = FindObjectsOfType<FloatControl>().ToList();
        octopus.gameTime = octopusTime - 10;
        timeout = penguinTime;

        //AkSoundEngine.PostEvent("PolarAmbience",gameObject);
	}

    IEnumerator LevelStart()
    {
        fadeGroup.alpha = 1;
        float timing = 2;
        float t = timing;
        //Vector3 origScale = transform.localScale;
        while (t > 0)
        {
            AkSoundEngine.SetRTPCValue("MasterVolume", (1 - t / timing) * 100);
            //transform.localScale = origScale * (timeout / fadeOutTime);
            //fadeGroup.alpha = (timing / 2);
            t -= Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator FadeIn(){
        AkSoundEngine.PostEvent("TabletPickup", gameObject);
        fadeGroup.interactable = false;
        fadeGroup.blocksRaycasts = false;
        fadeGroup.alpha = 1;
        float timing = 2;
        //Vector3 origScale = transform.localScale;
        while (timing > 0)
        {
            //AkSoundEngine.SetRTPCValue("MasterVolume", (1 - fadeGroup.alpha) * 100);
            //transform.localScale = origScale * (timeout / fadeOutTime);
            fadeGroup.alpha = (timing / 2);
            timing -= Time.deltaTime;
            yield return null;
        }
    }
	
    IEnumerator Begin(){
        //incidentals.SetActive(false);


        float timer = 10;
        while(timer>0){
            Vector3 pos = water.position;
            pos.y = Mathf.Lerp(-1.63f, waterLevel, 1-(timer / 10));
            water.position = pos;
            timer -= Time.deltaTime;

            yield return null;
        }
        for (int f = 0; f < floats.Count; f++)
        {
            floats[f].lockInPlace = true;
        }
        state = State.Penguins;
        incidentals.SetActive(true);
        //whale.Reset();
    }

    IEnumerator SwitchObjects(bool showHoops){
        //flickSwapParticles.ActivateParticles();
        yield return new WaitForSeconds(0.15f);
        if(showHoops){
            hoops.SetActive(true);
            bucket.SetActive(false);
            //hoops.GetComponent<Animator>().SetTrigger("Enter");
        } else {
            hoops.SetActive(false);
            bucket.SetActive(true);
            foreach (Animator a in bucket.GetComponentsInChildren<Animator>())
            {
                a.SetTrigger("Enter");
            }
            //bucket.GetComponent<Animator>().SetTrigger("Enter");
        }
    }

    IEnumerator UnloadLevel(){
        float timing = 1;
        while(timing>0){
            AkSoundEngine.SetRTPCValue("MasterVolume", timing * 100);
            timing -= Time.deltaTime;
            yield return null;
        }

        AkSoundEngine.StopAll();
        Debug.Log("plugged in - load attract scene");
        GameControl.Instance.LoadScene(AttractToLoad);


    }

    bool isBlack = false;
    public float fadeOutTime = 2;
    IEnumerator blackFade;
    float timmy = 0;

    public void FadeScreen()
    {
        if (blackFade != null) StopCoroutine(blackFade);
        if (isBlack)
        {
            blackFade = fadeFromBlack();
        }
        else
        {
            blackFade = fadeToBlack();
        }
        StartCoroutine(blackFade);
    }

    IEnumerator fadeFromBlack()
    {
        Debug.Log("fade from black");
        isBlack = false;
        timmy = fadeOutTime;
        float audioFade = 0;

        VideoPlayer videoPlayer = FindObjectOfType<VideoPlayer>();
        RawImage img = videoPlayer.GetComponent<RawImage>();

        videoPlayer.Play();

        while (timmy > 0)
        {
            img.color = Color.Lerp(Color.black, Color.white, 1 - (timmy / fadeOutTime));
            //fadeGroup.alpha = (timeout / fadeOutTime);
            audioFade = 1 - (timmy / fadeOutTime);
            AkSoundEngine.SetRTPCValue("MasterVolume", audioFade * 100);
            timmy -= Time.deltaTime;
            yield return null;
        }
        timmy = 0;
    }

    IEnumerator fadeToBlack()
    {
        Debug.Log("fade to black");
        isBlack = true;
        //fadeImage.color = Color.black;

        VideoPlayer videoPlayer = FindObjectOfType<VideoPlayer>();
        RawImage img = videoPlayer.GetComponent<RawImage>();


        timmy = fadeOutTime;
        float audioFade = 1;
        while (timmy > 0)
        {
            img.color = Color.Lerp(Color.white, Color.black, 1 - (timmy / fadeOutTime));
            //fadeGroup.alpha = 1 - (timeout / fadeOutTime);
            audioFade = (timmy / fadeOutTime);
            AkSoundEngine.SetRTPCValue("MasterVolume", audioFade * 100);
            timmy -= Time.deltaTime;
            yield return null;
        }
        timmy = 0;
        videoPlayer.Stop();
    }


    // Update is called once per frame
    void Update () {
        //if (state == State.Attract)
        //{
        //    if (Input.GetMouseButtonUp(0))
        //    {

        //        FadeScreen();
        //    }
        //    if (Input.touchCount > 0)
        //    {
        //        Touch t = Input.GetTouch(0);
        //        if (t.phase == TouchPhase.Ended && timmy < 0.01f)
        //        {
        //            FadeScreen();
        //        }
        //    }
        //}

        if (Input.GetKeyUp(KeyCode.K)){
            Debug.Log("filling all penguins");
            for (int p = 0; p < penguins.Count; p++){

                penguins[p].fishEaten = 5;
            }
        }
#if UNITY_IOS
        if (SystemInfo.batteryStatus != BatteryStatus.Discharging && !putDown && hasStarted)
        {
            putDown = true;
            StartCoroutine(UnloadLevel());

        }
        if(SystemInfo.batteryStatus == BatteryStatus.Discharging && !hasStarted){
            hasStarted = true;
            state = State.Begin;
        }
#endif
        if(Input.GetKeyUp(KeyCode.Return)){
            Debug.Log("Manual Switch");
            StartCoroutine(UnloadLevel());
        }
        switch(state){
            case State.Penguins:

                //if in leave mode check if all are full and then force the next game mode
                if (GameControl.Instance.CharacterBehaviour == GameControl.CharacterMode.Leave)
                {
                    int allFed = 0;
                    for (int p = 0; p < penguins.Count; p++)
                    {
                        if (penguins[p].fishEaten >= penguins[p].fishMax) allFed++;
                    }
                    //Debug.Log("fed count:" + allFed);
                    if (allFed >= GameControl.Instance.feedNumber) state = State.Escape;
                }
                break;

            case State.Escape:
                for (int p = 0; p < penguins.Count; p++)
                {
                    switch (penguins[p].state)
                    {
                        case MainCharacter.State.Swimming:
                        case MainCharacter.State.Dragging:
                        case MainCharacter.State.Escape:
                            penguins[p].swimState = MainCharacter.SwimState.Escape;
                            break;
                        case MainCharacter.State.Idle:
                        case MainCharacter.State.Dropped:
                        case MainCharacter.State.Walking:
                            StartCoroutine(MakeEscape(penguins[p], 0.25f * p));
                            //penguins[p].state = MainCharacter.State.Escape;
                            break;
                    }

                }


                break;
            case State.FloatDrop:
                for (int f = 0; f < floats.Count;f++){
                    floats[f].lockInPlace = false;
                }
                break;
            case State.Octopus:
                
                break;

            case State.Return:
                for(int p = 0; p < penguins.Count; p++){
                    penguins[p].fishEaten = 0;
                    penguins[p].bellyAmount = 0;
                    penguins[p].isFull = false;
                    penguins[p].state = MainCharacter.State.Swimming;
                    penguins[p].swimState = MainCharacter.SwimState.Return;

                    floats[p].lockInPlace = true;
                }
                break;
        }

        if(state!=State.Attract)
            timeout -= Time.deltaTime;

        if(timeout<0){
            state = ((int)state + 1 < Enum.GetNames(typeof(PolarController.State)).Length) ? state + 1 : (State)2;
  

        }

        if(Input.GetKeyUp(KeyCode.T)) state = ((int)state + 1 < Enum.GetNames(typeof(PolarController.State)).Length) ? state + 1 : (State)2;

	}

    IEnumerator MakeEscape(MainCharacter p, float t){
        yield return new WaitForSeconds(t);
        p.state = MainCharacter.State.Escape;


    }
    public void LoadMenu()
    {
        GameControl.Instance.LoadScene("Menu");
    }
}
