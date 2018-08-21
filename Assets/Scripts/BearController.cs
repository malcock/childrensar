using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BearController : MonoBehaviour {
    public Animator animator;
    public enum State { Start, Begin, Active, End }
    public State state = State.Start;
    public List<BearController> childBears;

    public float gameTime = 240;

    bool gameSuccess = false;
    public bool isFed = false;
    public bool isReady = false;

    Vector3 orig;

    IEnumerator playGame;


    public float bobOffset = 0;
    public float bobAmount = 0.5f;
    public float unfedOffset = 1.05f;
    public float fedOffset = 0.5f;

    public float currentOffset = 1.05f;
    float offset = 0;
    SkinnedMeshRenderer skin;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        childBears = GetComponentsInChildren<BearController>().ToList();
        childBears.Remove(this);
    }

	// Use this for initialization
	void Start () {
        orig = transform.position;
        currentOffset = unfedOffset;
        skin = GetComponentInChildren<SkinnedMeshRenderer>();
        StartCoroutine(IdleSound(Random.Range(20, 80)));
	}


    IEnumerator IdleSound(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        //can we cancel this depending on when the last event was called? In the last 5 seconds maybe?
        if (state == State.Active)
        {

            AkSoundEngine.PostEvent("BearIdle", gameObject);
            StartCoroutine(IdleSound(Random.Range(20, 80)));

        }

    }
	
	// Update is called once per frame
	void Update () {
        //rotate towards camera
        Vector3 relativePosition = Camera.main.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePosition);
        rotation.x = 0;
        rotation.z = 0;
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 3);
        float t = 0;
        float tD = 0;

       
        if (state != State.Start)
        {
            t = Time.time;
            tD = Time.deltaTime;
        }
        if (childBears.Count>0){
            List<BearController> fedChildren = childBears.Where(x => x.isFed).ToList();
            if(fedChildren.Count==childBears.Count){
                animator.SetTrigger("Ready");
                animator.SetBool("IsFed", true);
                isReady = true;
            }

        } else {
            animator.SetBool("IsFed", isFed);
        }
            
        if (isFed)
        {
            currentOffset = Mathf.Lerp(currentOffset, fedOffset, tD);
            offset = Mathf.Lerp(offset, -0.05f, tD);
        }
        else
        {
            currentOffset = Mathf.Lerp(currentOffset, unfedOffset, tD);
            offset = Mathf.Lerp(offset, 0.025f, tD);
        }
        float newY = (Mathf.Sin(t + bobOffset) * (bobAmount * currentOffset))+offset;
        Vector3 pos = transform.localPosition;
        pos.y = newY;

        transform.localPosition = pos;
        //}


	}

    public void Catch(){
        List<BearController> fedChildren = childBears.Where(x => x.isFed).ToList();
        if(fedChildren.Count==childBears.Count || childBears.Count==0){
            animator.SetTrigger("Catch");
            if(childBears.Count==0){
                AkSoundEngine.PostEvent("BearKidEat", gameObject);
            } else {
                AkSoundEngine.PostEvent("BearEat", gameObject);
            }

            isFed = true;
            if(childBears.Count>0){
                animator.SetTrigger("Celebrate");
                foreach (BearController b in childBears)
                {
                    b.animator.SetTrigger("Celebrate");
                    b.isFed = false;
                    b.isReady = false;
                    b.currentOffset = b.unfedOffset;
                    b.state = State.End;
                    if (b.childBears.Count == 0)
                    {
                        Splashable[] splashables = b.GetComponentsInChildren<Splashable>();
                        foreach (Splashable sp in splashables)
                        {
                            sp.enabled = true;
                        }
                    }
                }
                isFed = false;
                isReady = false;
                state = State.End;
                currentOffset = unfedOffset;
                StartCoroutine(FinaliseState());


            }
        }
    }
    IEnumerator FinaliseState(){
        yield return new WaitForSeconds(6f);
        state = State.Start;
        foreach(BearController b in childBears){
            b.state = State.Start;
            b.skin.enabled = false;
        }
        skin.enabled = false;

        FindObjectOfType<AlpineController>().state = AlpineController.State.Return;
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

        foreach(BearController b in childBears)
        {
            b.BeginGame();
            b.isReady = true;
            b.skin.enabled = true;
        }
        state = State.Begin;
        skin.enabled = true;

        animator.SetTrigger("Ready");
        Debug.Log(name + " play bears!");
        AkSoundEngine.SetSwitch("BearAppearDisappear", "Appear", gameObject);
        AkSoundEngine.PostEvent("BearAppearDisappear", gameObject);

        yield return new WaitForSeconds(2);
        //AkSoundEngine.PostEvent("OctopusVocal", gameObject);
        state = State.Active;
        if (childBears.Count==0)
        {
            Splashable[] splashables = GetComponentsInChildren<Splashable>();
            foreach(Splashable sp in splashables){
                sp.enabled = false;
            }
        }
        yield return new WaitForSeconds(gameTime);
        animator.SetTrigger("Fail");
        if (childBears.Count == 0)
        {
            foreach(BearController b in childBears){
                b.GetComponent<Animator>().SetTrigger("Fail");
            }
            Splashable[] splashables = GetComponentsInChildren<Splashable>();
            foreach (Splashable sp in splashables)
            {
                sp.enabled = true;
            }
        }
        state = State.End;
        AkSoundEngine.SetSwitch("BearAppearDisappear", "Disappear", gameObject);
        AkSoundEngine.PostEvent("BearAppearDisappear", gameObject);
        AkSoundEngine.PostEvent("BearVocal", gameObject);
    }

    public void StopGame()
    {
        StopAllCoroutines();
        state = State.End;
    }

    public void Speak()
    {
        SalmonController[] salmons = FindObjectsOfType<SalmonController>();
        foreach(SalmonController s in salmons){
            InteractableObject ob = s.GetComponent<InteractableObject>();
            if (ob.locked) return;
        }

        Debug.Log(name + "Speak!");
        animator.SetTrigger("Speak");
        if (childBears.Count > 0)
        {
            AkSoundEngine.PostEvent("BearIdle", gameObject);
        } else {
            AkSoundEngine.PostEvent("Bear_Small_Tap", gameObject);
        }

    }

}
