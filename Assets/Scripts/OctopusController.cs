using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusController : MonoBehaviour {

    Animator animator;



    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        for (int l = 1; l < animator.layerCount;l++){
            animator.SetLayerWeight(l, Mathf.Abs(Mathf.Sin(Time.time + (l*20))));
        }

	}
}
