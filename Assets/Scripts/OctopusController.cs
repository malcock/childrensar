using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusController : MonoBehaviour {

    Animator animator;

    public bool isActive = false;

    float time = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (isActive)
        {
            animator.SetBool("Active",true);
            for (int l = 1; l < animator.layerCount; l++)
            {
                animator.SetLayerWeight(l, Mathf.Abs(Mathf.Sin(time + (l * 20))));
                time += Time.deltaTime*0.25f;
            }
        } else {
            
            int layersReady = animator.layerCount-1;
            time = 0;
            for (int l = 1; l < animator.layerCount; l++)
            {
                if (animator.GetLayerWeight(l) > 0.05f)
                {
                    animator.SetLayerWeight(l, Mathf.Lerp(animator.GetLayerWeight(l), 0, Time.deltaTime));
                }
                else {
                    animator.SetLayerWeight(l,0);
                    layersReady--;
                }

            }
            if (layersReady == 0) animator.SetBool("Active", false);
        }

	}
}
