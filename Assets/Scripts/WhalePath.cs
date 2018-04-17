using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhalePath : MonoBehaviour {

    public float timeout = 10;
    public float firstDelay = 120;

    Animator animator;
    public bool OctoOut = false;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
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
        yield return new WaitForSeconds(firstDelay);

        Reset();
    }

    IEnumerator DoTimeout(){
        yield return new WaitForSeconds(timeout);
        if(!OctoOut){
            animator.SetTrigger("NextAnim");
            Debug.Log("Whale trig");
            AkSoundEngine.PostEvent("WhaleSwim",gameObject);
        } else {
            Reset();
        }


    }


}
