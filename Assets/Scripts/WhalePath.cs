using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhalePath : MonoBehaviour {

    public float timeout = 10;

    Animator animator;

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
        StartCoroutine(DoTimeout());
    }

    IEnumerator DoTimeout(){
        yield return new WaitForSeconds(timeout);
        animator.SetTrigger("NextAnim");

    }


}
