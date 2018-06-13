using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(InteractableObject))]
public class Ring : MonoBehaviour
{
    public float spinTime = 1;
    private bool hasThrown = false;

    public bool isActive = true;
    private bool success = false;

    public float fadeOutTime = 1;
    InteractableObject interactableObj;

    // Use this for initialization
    void Start()
    {
        foreach(RagBone bone in GetComponentsInChildren<RagBone>()){
            bone.onCollisionEnter.AddListener(CollisionEnter);
            bone.onTriggerEnter.AddListener(TriggerEnter);
        }
        interactableObj = GetComponent<InteractableObject>();
        interactableObj.OnDragStart.AddListener(DragStart);
        interactableObj.OnDrag.AddListener(Drag);
        interactableObj.OnDragEnd.AddListener(DragEnd);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DragStart(){
        if(!hasThrown)
            AkSoundEngine.PostEvent("InteractRingPickup", gameObject);
    }
    void Drag(){
        
    }
    void DragEnd(){
        if(!hasThrown){
            AkSoundEngine.PostEvent("InteractRingThrow", gameObject);
            hasThrown = true;
        }

    }

    public void DestroyRing(){
        StartCoroutine(FadeAway());
    }
    private void TriggerEnter(Collider other,GameObject obj)
    {
        if (!isActive) return;

        DestroyRing();
        isActive = false;
    }

    private void CollisionEnter(Collision collision,GameObject obj)
    {
        if (!isActive) return;
        Debug.Log("ring collided: " + collision.gameObject.name);

        OctoBone hasBone = collision.gameObject.GetComponent<OctoBone>();
        if(hasBone!=null){
            OctopusController octoController = hasBone.GetComponentInParent<OctopusController>();
            if(hasBone.LimbNum==octoController.boringLeg){
                Debug.Log("ring hit octo");
                //the ring hit an octobone, attach it and do stuff
                Transform parent = hasBone.transform;
                Vector3 position = collision.contacts[0].point;
                if (hasBone.TargetBone != null)
                {
                    parent = hasBone.TargetBone.transform;
                    position = hasBone.TargetBone.transform.position;
                }
                success = true;
                PlaceRing(position, parent);

                //send a signal to the octopus parent to say it's been caught

                hasBone.GetComponentInParent<OctopusController>().boringLegState = OctopusController.BoringLegMode.End;
                octoController.octoArms[octoController.boringLeg] = true;

                //play a sound?
                AkSoundEngine.PostEvent("InteractRingSuccessL" + hasBone.soundLevel, gameObject);
                //particle effects?
            } else {
                AkSoundEngine.PostEvent("InteractRingImpact", gameObject);
                DestroyRing();
            }


        } else {
            AkSoundEngine.PostEvent("InteractRingImpact", gameObject);
            DestroyRing();
        }
        isActive = false;
    }

    void PlaceRing(Vector3 position, Transform parent){
        

        //turn off all the things and reset everything to 0s
        foreach(Rigidbody rb in GetComponentsInChildren<Rigidbody>()){
            rb.isKinematic = true;

        }
        foreach(Collider c in GetComponentsInChildren<Collider>()){
            c.enabled = false;
        }
        foreach(Transform t in GetComponentsInChildren<Transform>()){
            t.localPosition = Vector3.zero;
            t.rotation = Quaternion.identity;
        }

        transform.position = position;
        transform.eulerAngles = new Vector3(90, 0, 10);
        transform.parent = parent;


        StartCoroutine(SpinRing());
        AkSoundEngine.PostEvent("OctopusVocal", gameObject);
    }

    IEnumerator SpinRing(){
        float timeout = spinTime;
        Vector3 originalPos = transform.localPosition;
        while(timeout>0){
            float offset = timeout * 0.1f;
            float spinFactor = timeout * 20;
            transform.localPosition = originalPos + new Vector3(Mathf.Sin(spinFactor) * offset, 0, Mathf.Cos(spinFactor) * offset);
            timeout-=Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator FadeAway(){
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
        while(timeout>0){
            //transform.localScale = origScale * (timeout / fadeOutTime);

            timeout -= Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    public void DisappearRing(){
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
        foreach(Collider c in cls){
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
