using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhalePath : MonoBehaviour {

    public float timeout = 10;
    public float firstDelay = 120;
    private int timesaround = 0;

    Animator animator;
    public bool OctoOut = false;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();

	}
	private void OnEnable()
	{
        Debug.Log("Whale start");

        StartCoroutine(Begin());
	}
	// Update is called once per frame
	void Update () {
		
	}


    public void Reset(){
        Debug.Log(name + " reset");
        StartCoroutine(DoTimeout());
        AkSoundEngine.StopAll(gameObject);

    }

    IEnumerator Begin(){
        Debug.Log("Whale started first wait " + firstDelay);
        yield return new WaitForSeconds(firstDelay);
        Debug.Log("Whale first delay complete");
        Reset();
    }

    IEnumerator DoTimeout(){
        yield return new WaitForSeconds(timeout);
        if(!OctoOut){
            animator.SetTrigger("NextAnim");
            Debug.Log("Whale trig");
            AkSoundEngine.PostEvent("WhaleSwim",gameObject);
            timesaround++;
            if(timesaround>1)
                AkSoundEngine.PostEvent("WhaleBreach", gameObject);

        } else {
            Reset();
        }



    }


}
