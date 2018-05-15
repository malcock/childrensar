using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalmonController : MonoBehaviour {
    Flickable flick;
    Animator animator;
	// Use this for initialization
	void Start () {
        flick = GetComponentInParent<Flickable>();
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        animator.SetBool("isHeld", flick.isActive);
	}
}
