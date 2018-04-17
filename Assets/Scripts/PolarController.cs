using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PolarController : MonoBehaviour {

    public string AttractToLoad = "Polar Attract 1";

    public float penguinTime = 240, octopusTime = 240, changeover = 5, fadeTime=3;

    public float timeout;

    float waterLevel;

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
        state = State.Begin;


        penguinTime = GameControl.Instance.gametimeA;
        octopusTime = GameControl.Instance.gametimeB;

        penguins = FindObjectsOfType<MainCharacter>().ToList();
        floats = FindObjectsOfType<FloatControl>().ToList();
        octopus.gameTime = octopusTime - 10;
        timeout = penguinTime;

        AkSoundEngine.PostEvent("PolarAmbience",gameObject);
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
            hoops.GetComponent<Animator>().SetTrigger("Enter");
        } else {
            hoops.SetActive(false);
            bucket.SetActive(true);
            bucket.GetComponent<Animator>().SetTrigger("Enter");
        }
    }

	// Update is called once per frame
	void Update () {

        if(Input.GetKeyUp(KeyCode.O)){
            for (int p = 0; p < penguins.Count; p++){
                penguins[p].fishEaten = 5;
            }
        }
//#if UNITY_IOS
        if (SystemInfo.batteryStatus != BatteryStatus.Discharging)
        {
            Debug.Log("plugged in - load attract scene");
            //GameControl.Instance.LoadScene(AttractToLoad);
        }
//#endif
        if(Input.GetKeyUp(KeyCode.Return)){
            Debug.Log("Manual Switch");
            GameControl.Instance.LoadScene(AttractToLoad);
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
                    if (allFed >= penguins.Count) state = State.Escape;
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
                            penguins[p].state = MainCharacter.State.Escape;
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


}
