using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PolarController : MonoBehaviour {

    public float penguinTime = 240, octopusTime = 240, changeover = 5, fadeTime=3;

    public float timeout;

    public enum State {Penguins, Escape, Octopus, Return }
    [SerializeField]
    private State _state = State.Penguins;
    public State state {
        get { return _state; }
        set {
            _state = value;
            switch(_state){
                case State.Penguins:
                    StartCoroutine(SwitchObjects(false));

                    whale.OctoOut = false;
                    timeout = penguinTime;
                    break;
                case State.Escape:
                    

                    whale.OctoOut = true;
                    timeout = changeover;
                    break;
                case State.Return:

                    if (octopus.state != OctopusController.State.End) octopus.StopGame();
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

    public List<MainCharacter> penguins = new List<MainCharacter>();
    public List<FloatControl> floats = new List<FloatControl>();
    public GameObject bucket, hoops;
    public ParticleController flickSwapParticles;
    public OctopusController octopus;
    public WhalePath whale;


	// Use this for initialization
	void Start () {
        state = State.Penguins;

        penguinTime = GameControl.Instance.gametimeA;
        octopusTime = GameControl.Instance.gametimeB;

        penguins = FindObjectsOfType<MainCharacter>().ToList();
        floats = FindObjectsOfType<FloatControl>().ToList();
        octopus.gameTime = octopusTime - 10;
        timeout = penguinTime;

        AkSoundEngine.PostEvent("PolarAmbience",gameObject);
	}
	
    IEnumerator SwitchObjects(bool showHoops){
        flickSwapParticles.ActivateParticles();
        yield return new WaitForSeconds(0.15f);
        if(showHoops){
            hoops.SetActive(true);
            bucket.SetActive(false);
        } else {
            hoops.SetActive(false);
            bucket.SetActive(true);
        }
    }

	// Update is called once per frame
	void Update () {



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

                    floats[p].lockInPlace = false;
                }


                break;
            case State.Octopus:
                
                break;

            case State.Return:
                for(int p = 0; p < penguins.Count; p++){
                    penguins[p].fishEaten = 0;
                    penguins[p].state = MainCharacter.State.Swimming;
                    penguins[p].swimState = MainCharacter.SwimState.Return;

                    floats[p].lockInPlace = true;
                }
                break;
        }

        timeout -= Time.deltaTime;

        if(timeout<0){
            state = ((int)state + 1 < Enum.GetNames(typeof(PolarController.State)).Length) ? state + 1 : 0;


        }

        if(Input.GetKeyUp(KeyCode.T)) state = ((int)state + 1 < Enum.GetNames(typeof(PolarController.State)).Length) ? state + 1 : 0;

	}


}
