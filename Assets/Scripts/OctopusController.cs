using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OctopusController : MonoBehaviour
{

    Animator animator;

    public enum State { Start, Begin, Active, End }
    public State state = State.Start;
    public float wobbleAmount = 20;

    public enum BoringLegMode { Pre, Start, Middle, End }
    public BoringLegMode boringLegState = BoringLegMode.Pre;

    public int boringLeg = 0;

    public Dictionary<int, bool> octoArms = new Dictionary<int, bool>();

    float time = 0;
    float finalLegTime = 3.33f;

    public float gameTime = 240;

    bool gameSuccess = false;

    IEnumerator playGame;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        foreach (BoxCollider box in GetComponentsInChildren<BoxCollider>())
        {
            //box.gameObject.AddComponent<OctoBone>();
            //box.isTrigger = true;
        }
        for (int l = 1; l < animator.layerCount; l++)
        {
            octoArms.Add(l-1, false);
        }
    }

    // Use this for initialization
    void Start()
    {
        ResetArms();
    }

    public void ResetArms()
    {
        var keys = new List<int>(octoArms.Keys);
        gameSuccess = false;
        foreach (int k in keys)
        {
            octoArms[k] = false;
        }
    }

    public void BeginGame()
    {
        if (playGame != null) StopCoroutine(playGame);
        playGame = PlayGame();
        StartCoroutine(playGame);
    }

    IEnumerator PlayGame()
    {
        yield return new WaitForSeconds(1);
        state = State.Begin;
        animator.SetTrigger("Begin");
        AkSoundEngine.SetSwitch("OctopusAppearDisappear", "Appear", gameObject);
        AkSoundEngine.PostEvent("OctopusAppearDisappear", gameObject);

        yield return new WaitForSeconds(2);
        AkSoundEngine.PostEvent("OctopusVocal", gameObject);
        state = State.Active;
        yield return new WaitForSeconds(gameTime);
        state = State.End;
        AkSoundEngine.SetSwitch("OctopusAppearDisappear", "Disappear", gameObject);
        AkSoundEngine.PostEvent("OctopusAppearDisappear", gameObject);
        AkSoundEngine.PostEvent("OctopusVocal", gameObject);
    }

    public void StopGame()
    {
        StopAllCoroutines();
        state = State.End;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameControl.Instance.OctoBehaviour == GameControl.OctoMode.Crazy)
        {
            CrazyLegs();
        }
        else if(GameControl.Instance.OctoBehaviour == GameControl.OctoMode.Final)
        {
            FinalLegs();
        } else {
            BoringLegs();
        }


    }

    void BoringLegs()
    {
        switch (state)
        {
            case State.Start:

                break;
            case State.Begin:
                foreach (Ring r in GetComponentsInChildren<Ring>())
                {
                    Destroy(r.gameObject);
                }
                break;
            case State.Active:
                switch (boringLegState)
                {
                    case BoringLegMode.Pre:
                        //Find an available arm and choose it
                        KeyValuePair<int, bool>[] arms = octoArms.Where(x => x.Value == false).ToArray();
                        //choose random arm
                        if (arms.Length == 0)
                        {
                            state = State.End;
                            break;
                        }

                        boringLeg = arms[Random.Range(0, arms.Length - 1)].Key;


                        boringLegState = BoringLegMode.Start;
                        break;

                    case BoringLegMode.Start:
                        //kill all layer weights
                        for (int l = 1; l < animator.layerCount; l++)
                        {
                            animator.SetLayerWeight(l, 0);
                        }
                        //set the weighting to max
                        animator.SetLayerWeight(boringLeg, 1);
                        //now make it active
                        animator.SetBool("Active", true);

                        //need some way of counting 
                        //AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(boringLeg);
                        //AnimatorClipInfo animClip = animator.GetCurrentAnimatorClipInfo(boringLeg);
                        //if(animClip.clip.)

                        break;

                    case BoringLegMode.Middle:
                        // this would be where we would pause the animation and play the ring game

                        break;

                    case BoringLegMode.End:
                        //a ring has been tossed on this tentacle, we need to lower it and start the cycle again
                        if (animator.GetLayerWeight(boringLeg) > 0)
                        {
                            animator.SetLayerWeight(boringLeg, animator.GetLayerWeight(boringLeg) - Time.deltaTime);
                        }
                        else
                        {
                            octoArms[boringLeg] = true;
                            animator.SetBool("Active", false);
                            boringLegState = BoringLegMode.Pre;
                        }

                        break;

                }



                Vector3 rotation = transform.eulerAngles;
                float newY = 180 + Mathf.Sin(Time.time) * wobbleAmount;
                rotation.y = Mathf.Lerp(rotation.y, newY, Time.deltaTime);
                transform.eulerAngles = rotation;
                break;
            case State.End:
                ResetArms();

                int layersReady = animator.layerCount - 1;
                time = 0;
                for (int l = 1; l < animator.layerCount; l++)
                {
                    if (animator.GetLayerWeight(l) > 0.05f)
                    {
                        animator.SetLayerWeight(l, Mathf.Lerp(animator.GetLayerWeight(l), 0, Time.deltaTime));
                    }
                    else
                    {
                        animator.SetLayerWeight(l, 0);
                        layersReady--;
                    }

                }
                if (layersReady == 0)
                {
                    StopCoroutine(playGame);
                    animator.SetBool("Active", false);
                    animator.SetTrigger("Celebrate");
                    state = State.Start;
                    FindObjectOfType<PolarController>().state = PolarController.State.Return;
                }
                break;

        }
    }

    IEnumerator HandleLeg(int leg, float timeout){
        animator.SetTrigger("Leg" + leg);
        yield return new WaitForSeconds(5f);

        animator.SetTrigger("Leg" + leg);
    }

    void FinalLegs()
    {
        switch (state)
        {
            case State.Start:

                break;
            case State.Begin:
                
                foreach(Collider c in GetComponentsInChildren<Collider>()){
                    c.enabled = true;
                }
                foreach (Ring r in GetComponentsInChildren<Ring>())
                {
                    Destroy(r.gameObject);
                }
                break;
            case State.Active:
                //every 3.5 seconds, choose a different leg to raise from those that are still available
                animator.SetBool("Active", true);

                finalLegTime += Time.deltaTime;
                if (finalLegTime > 10f)
                {
                    KeyValuePair<int, bool>[] arms = octoArms.Where(x => x.Value == false).ToArray();
                    //choose random arm
                    if (arms.Length == 0)
                    {
                        state = State.End;
                        gameSuccess = true;
                        break;
                    }



                    boringLeg = arms[Random.Range(0, arms.Length)].Key;
                    Debug.Log("chosen leg: " + boringLeg);
                    StartCoroutine(HandleLeg(boringLeg, 0.5f));


                    //for (int l = 1; l < animator.layerCount; l++)
                    //{
                    //    animator.SetLayerWeight(l, 0);
                    //}
                    ////set the weighting to max
                    //animator.SetLayerWeight(boringLeg, 1);

                    finalLegTime = 0;
                }





                Vector3 rotation = transform.eulerAngles;
                float newY = 180 + Mathf.Sin(Time.time) * wobbleAmount;
                rotation.y = Mathf.Lerp(rotation.y, newY, Time.deltaTime);
                transform.eulerAngles = rotation;
                break;
            case State.End:
                
                StopCoroutine(playGame);
                AkSoundEngine.SetSwitch("OctopusAppearDisappear", "Disappear", gameObject);
                AkSoundEngine.PostEvent("OctopusAppearDisappear", gameObject);
                animator.SetBool("Active", false);
                if (gameSuccess){
                    animator.SetTrigger("Celebrate");
                    AkSoundEngine.PostEvent("InteractRingSuccessL3", gameObject);
                } else {
                    animator.SetTrigger("Done");
                }


                ResetArms();
                state = State.Start;
                foreach (Collider c in GetComponentsInChildren<Collider>())
                {
                    c.enabled = false;
                }
                FindObjectOfType<PolarController>().state = PolarController.State.Return;



                break;

        }
    }

    void CrazyLegs()
    {


        switch (state)
        {
            case State.Start:

                break;
            case State.Begin:
                foreach (Ring r in GetComponentsInChildren<Ring>())
                {
                    Destroy(r.gameObject);
                }
                break;
            case State.Active:
                animator.SetBool("Active", true);
                for (int l = 1; l < animator.layerCount; l++)
                {
                    animator.SetLayerWeight(l, Mathf.Abs(Mathf.Sin(time + (l * 20))));
                    time += Time.deltaTime * 0.25f;
                }
                //rotate the octopus slightly
                Vector3 rotation = transform.eulerAngles;
                float newY = 180 + Mathf.Sin(Time.time) * wobbleAmount;
                rotation.y = Mathf.Lerp(rotation.y, newY, Time.deltaTime);
                transform.eulerAngles = rotation;
                break;
            case State.End:
                int layersReady = animator.layerCount - 1;
                time = 0;
                for (int l = 1; l < animator.layerCount; l++)
                {
                    if (animator.GetLayerWeight(l) > 0.05f)
                    {
                        animator.SetLayerWeight(l, Mathf.Lerp(animator.GetLayerWeight(l), 0, Time.deltaTime));
                    }
                    else
                    {
                        animator.SetLayerWeight(l, 0);
                        layersReady--;
                    }

                }
                if (layersReady == 0)
                {
                    animator.SetBool("Active", false);
                    animator.SetTrigger("Done");
                    state = State.Start;
                }
                break;

        }
    }
}
