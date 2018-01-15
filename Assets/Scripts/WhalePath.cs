using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhalePath : MonoBehaviour {

    public float timeout = 10;

    Animator animator;
    public bool OctoOut = false;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        Reset();
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void Reset(){
        Debug.Log(name + " reset");
        AkSoundEngine.StopAll(gameObject);
        StartCoroutine(DoTimeout());
    }

    IEnumerator DoTimeout(){
        yield return new WaitForSeconds(timeout);
        if(!OctoOut){
            animator.SetTrigger("NextAnim");
            AkSoundEngine.PostEvent("WhaleSwim",gameObject);
        } else {
            Reset();
        }


    }


}
