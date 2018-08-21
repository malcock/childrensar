using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PolarController : MonoBehaviour {

    public string AttractToLoad = "Polar Attract 1";

    public float penguinTime = 240, octopusTime = 240, changeover = 5, fadeTime=3;

    public float timeout;
    public CanvasGroup fadeGroup;

    float waterLevel;

    bool putDown = false;

    public enum State {Begin, Penguins, Escape, FloatDrop, Octopus, Return }
    [SerializeField]
    private State _state = State.Begin;
    public State state {
        get { return _state; }
        set {
            _state = value;
            switch(_state){
                case State.Begin:
                    
                    Vector3 pos = water.position;
                    waterLevel = pos.y;
                    pos.y = -1.68f;
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
        AkSoundEngine.PostEvent("PolarAmbience", gameObject);
        StartCoroutine(FadeIn());
        state = State.Begin;


        penguinTime = GameControl.Instance.gametimeA;
        octopusTime = GameControl.Instance.gametimeB;

        penguins = FindObjectsOfType<MainCharacter>().ToList();
        floats = FindObjectsOfType<FloatControl>().ToList();
        octopus.gameTime = octopusTime - 10;
        timeout = penguinTime;

        //AkSoundEngine.PostEvent("PolarAmbience",gameObject);
	}

    IEnumerator FadeIn(){
        fadeGroup.alpha = 1;
        float timing = 2;
        //Vector3 origScale = transform.localScale;
        while (timing > 0)
        {
            AkSoundEngine.SetRTPCValue("MasterVolume", (1 - fadeGroup.alpha) * 100);
            //transform.localScale = origScale * (timeout / fadeOutTime);
            fadeGroup.alpha = (timing / 2);
            timing -= Time.deltaTime;
            yield return null;
        }
    }
	
    IEnumerator Begin(){
        incidentals.SetActive(false);


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

	// Update is called once per frame
	void Update () {

        if(Input.GetKeyUp(KeyCode.K)){
            Debug.Log("filling all penguins");
            for (int p = 0; p < penguins.Count; p++){

                penguins[p].fishEaten = 5;
            }
        }
#if UNITY_IOS
        if (SystemInfo.batteryStatus != BatteryStatus.Discharging && !putDown)
        {
            putDown = true;
            StartCoroutine(UnloadLevel());

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

        timeout -= Time.deltaTime;

        if(timeout<0){
            state = ((int)state + 1 < Enum.GetNames(typeof(PolarController.State)).Length) ? state + 1 : (State)1;
  

        }

        if(Input.GetKeyUp(KeyCode.T)) state = ((int)state + 1 < Enum.GetNames(typeof(PolarController.State)).Length) ? state + 1 : 0;

	}

    IEnumerator MakeEscape(MainCharacter p, float t){
        yield return new WaitForSeconds(t);
        p.state = MainCharacter.State.Escape;


    }
}
