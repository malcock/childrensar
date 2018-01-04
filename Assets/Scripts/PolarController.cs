using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PolarController : MonoBehaviour {

    public float penguinTime = 240, octopusTime = 240, changeover = 5;

    private float timeout;

    public enum State {Penguins, Escape, Octopus, Return }
    [SerializeField]
    private State _state = State.Penguins;
    public State state {
        get { return _state; }
        set {
            _state = value;
            switch(_state){
                case State.Penguins:
                    timeout = penguinTime;
                    break;
                case State.Escape: case State.Return:
                    timeout = changeover;
                    break;
                case State.Octopus:
                    timeout = octopusTime;
                    break;
            }
        }
    }

    public List<MainCharacter> penguins = new List<MainCharacter>();
    public List<FloatControl> floats = new List<FloatControl>();
    public OctopusController octopus;
    public WhalePath whale;

	// Use this for initialization
	void Start () {
        penguins = FindObjectsOfType<MainCharacter>().ToList();
        floats = FindObjectsOfType<FloatControl>().ToList();

	}
	
	// Update is called once per frame
	void Update () {

        switch(state){
            case State.Penguins:

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

	}


}
