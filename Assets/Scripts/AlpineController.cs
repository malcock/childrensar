using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AlpineController : MonoBehaviour
{

    public string AttractToLoad = "Alpine Attract 1";

    public float otterTime = 240, bearTime = 240, changeover = 5, fadeTime = 3;

    public float timeout;
    public CanvasGroup fadeGroup;

    float waterLevel;
    bool putDown = false;

    public enum State { Begin, Otters, Escape, FloatDrop, Bears, Return }
    [SerializeField]
    private State _state = State.Begin;
    public State state
    {
        get { return _state; }
        set
        {
            _state = value;
            switch (_state)
            {
                case State.Begin:

                    Vector3 pos = water.position;
                    waterLevel = pos.y;
                    pos.y = -1.68f;
                    StartCoroutine(Begin());
                    break;
                case State.Otters:
                    SalmonController[] rings = FindObjectsOfType<SalmonController>();
                    foreach (SalmonController r in rings)
                    {
                        r.DisappearRing();
                    }
                    StartCoroutine(SwitchObjects(false));
                    for (int p = 0; p < otters.Count; p++)
                    {
                        otters[p].isReady = true;
                    }

                    whale.OctoOut = false;
                    timeout = otterTime;
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
                case State.Bears:


                    StartCoroutine(SwitchObjects(true));

                    whale.OctoOut = true;
                    timeout = bearTime;
                    bears.BeginGame();
                    break;
            }
        }
    }
    public Transform water;
    public List<MainCharacter> otters = new List<MainCharacter>();
    public List<FloatControl> floats = new List<FloatControl>();
    public GameObject bucket, salmon;
    public ParticleController flickSwapParticles;
    public BearController bears;
    public WhalePath whale;
    public GameObject incidentals;

    // Use this for initialization
    void Start()
    {
        
        AkSoundEngine.PostEvent("AlpineAmbience", gameObject);
        StartCoroutine(FadeIn());
        state = State.Begin;


        otterTime = GameControl.Instance.gametimeA;
        bearTime = GameControl.Instance.gametimeB;

        otters = FindObjectsOfType<MainCharacter>().ToList();
        floats = FindObjectsOfType<FloatControl>().ToList();
        bears.gameTime = bearTime - 10;
        timeout = otterTime;

        //AkSoundEngine.PostEvent("AlpineAmbience", gameObject);
    }

    IEnumerator FadeIn()
    {
        fadeGroup.alpha = 1;
        float timing = 2;
        //Vector3 origScale = transform.localScale;
        while (timing > 0)
        {
            //transform.localScale = origScale * (timeout / fadeOutTime);
            fadeGroup.alpha = (timing / 2);
            timing -= Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Begin()
    {
        incidentals.SetActive(false);

        float timer = 10;
        while (timer > 0)
        {
            Vector3 pos = water.position;
            pos.y = Mathf.Lerp(-1.63f, waterLevel, 1 - (timer / 10));
            water.position = pos;
            timer -= Time.deltaTime;

            yield return null;
        }
        for (int f = 0; f < floats.Count; f++)
        {
            floats[f].lockInPlace = true;
        }
        state = State.Otters;
        incidentals.SetActive(true);
        //whale.Reset();
    }

    IEnumerator SwitchObjects(bool showHoops)
    {
        //flickSwapParticles.ActivateParticles();
        yield return new WaitForSeconds(0.15f);
        if (showHoops)
        {
            salmon.SetActive(true);
            bucket.SetActive(false);
            //salmon.GetComponent<Animator>().SetTrigger("Enter");
        }
        else
        {
            salmon.SetActive(false);
            bucket.SetActive(true);
            foreach (Animator a in bucket.GetComponentsInChildren<Animator>()){
                a.SetTrigger("Enter");
            }
            //bucket.GetComponent<Animator>().SetTrigger("Enter");
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyUp(KeyCode.K))
        {
            Debug.Log("filling all penguins");
            for (int p = 0; p < otters.Count; p++)
            {

                otters[p].fishEaten = 5;
            }
        }
#if UNITY_IOS
        if (SystemInfo.batteryStatus != BatteryStatus.Discharging && !putDown)
        {
            AkSoundEngine.StopAll();
            Debug.Log("plugged in - load attract scene");
            GameControl.Instance.LoadScene(AttractToLoad);
            putDown = true;
        }
#endif
        if (Input.GetKeyUp(KeyCode.Return))
        {
            Debug.Log("Manual Switch");
            GameControl.Instance.LoadScene(AttractToLoad);
        }
        switch (state)
        {
            case State.Otters:

                //if in leave mode check if all are full and then force the next game mode
                if (GameControl.Instance.CharacterBehaviour == GameControl.CharacterMode.Leave)
                {
                    int allFed = 0;
                    for (int p = 0; p < otters.Count; p++)
                    {
                        if (otters[p].fishEaten >= otters[p].fishMax) allFed++;
                    }
                    //Debug.Log("fed count:" + allFed);
                    if (allFed >= GameControl.Instance.feedNumber) state = State.Escape;
                }
                break;

            case State.Escape:
                for (int p = 0; p < otters.Count; p++)
                {
                    switch (otters[p].state)
                    {
                        case MainCharacter.State.Swimming:
                        case MainCharacter.State.Dragging:
                        case MainCharacter.State.Escape:
                            otters[p].swimState = MainCharacter.SwimState.Escape;
                            break;
                        case MainCharacter.State.Idle:
                        case MainCharacter.State.Dropped:
                        case MainCharacter.State.Walking:
                            StartCoroutine(MakeEscape(otters[p], 0.25f * p));
                            //penguins[p].state = MainCharacter.State.Escape;
                            break;
                    }

                }


                break;
            case State.FloatDrop:
                for (int f = 0; f < floats.Count; f++)
                {
                    floats[f].lockInPlace = false;
                }
                break;
            case State.Bears:

                break;

            case State.Return:
                for (int p = 0; p < otters.Count; p++)
                {
                    otters[p].fishEaten = 0;
                    otters[p].bellyAmount = 0;
                    otters[p].state = MainCharacter.State.Swimming;
                    otters[p].swimState = MainCharacter.SwimState.Return;

                    floats[p].lockInPlace = true;
                }
                break;
        }

        timeout -= Time.deltaTime;

        if (timeout < 0)
        {
            state = ((int)state + 1 < Enum.GetNames(typeof(PolarController.State)).Length) ? state + 1 : (State)1;


        }

        if (Input.GetKeyUp(KeyCode.T)) state = ((int)state + 1 < Enum.GetNames(typeof(PolarController.State)).Length) ? state + 1 : 0;

    }

    IEnumerator MakeEscape(MainCharacter p, float t)
    {
        yield return new WaitForSeconds(t);
        p.state = MainCharacter.State.Escape;


    }
}
