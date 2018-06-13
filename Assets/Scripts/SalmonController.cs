using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractableObject))]
public class SalmonController : MonoBehaviour {
    Flickable flick;
    Animator animator;
    public float fadeOutTime = 1;
    private bool hasThrown = false;
    public bool isActive = true;
    InteractableObject interactableObj;
	// Use this for initialization
	void Start () {
        flick = GetComponentInParent<Flickable>();
        animator = GetComponentInChildren<Animator>();

        AkSoundEngine.PostEvent("AnimationSalmonFall", gameObject);

        foreach (RagBone bone in GetComponentsInChildren<RagBone>())
        {
            
            bone.onTriggerEnter.AddListener(TriggerEnter);
        }
        interactableObj = GetComponent<InteractableObject>();
        interactableObj.OnDragStart.AddListener(DragStart);
        interactableObj.OnDrag.AddListener(Drag);
        interactableObj.OnDragEnd.AddListener(DragEnd);
	}
	
	// Update is called once per frame
	void Update () {
        if(animator!=null)
            animator.SetBool("isHeld", flick.isActive);
	}

    void DragStart()
    {
        
        AkSoundEngine.PostEvent("InteractSalmonPickup", gameObject);
    }
    void Drag()
    {

    }
    void DragEnd()
    {
        if (!hasThrown)
        {
            AkSoundEngine.PostEvent("InteractSamonThrow", gameObject);
            hasThrown = true;
        }

    }

    public void DestroyRing()
    {
        StartCoroutine(FadeAway());
    }
    private void TriggerEnter(Collider other, GameObject obj)
    {
        if (!isActive) return;

        DestroyRing();
        isActive = false;
    }

    IEnumerator FadeAway()
    {
        //Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
        //foreach (Rigidbody r in rbs)
        //{
        //    r.isKinematic = true;
        //    r.useGravity = false;
        //}
        //Collider[] cls = GetComponentsInChildren<Collider>();
        //foreach(Collider c in cls){
        //    c.enabled = false;
        //}
        float timeout = fadeOutTime;
        //Vector3 origScale = transform.localScale;
        while (timeout > 0)
        {
            //transform.localScale = origScale * (timeout / fadeOutTime);

            timeout -= Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    public void DisappearRing()
    {
        StartCoroutine(DisRing());
    }


    IEnumerator DisRing()
    {
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody r in rbs)
        {
            r.isKinematic = true;
            r.useGravity = false;
        }
        Collider[] cls = GetComponentsInChildren<Collider>();
        foreach (Collider c in cls)
        {
            c.enabled = false;
        }
        float timeout = 1;
        Vector3 origScale = transform.localScale;
        while (timeout > 0)
        {
            transform.localScale = origScale * (timeout / fadeOutTime);

            timeout -= Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
