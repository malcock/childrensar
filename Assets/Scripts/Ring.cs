using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ring : MonoBehaviour
{
    public float spinTime = 1;

    public bool isActive = true;
    private bool success = false;

    public float fadeOutTime = 5;

    // Use this for initialization
    void Start()
    {
        foreach(RagBone bone in GetComponentsInChildren<RagBone>()){
            bone.onCollisionEnter.AddListener(CollisionEnter);
            bone.onTriggerEnter.AddListener(TriggerEnter);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
            Debug.Log("ring hit octo");
            //the ring hit an octobone, attach it and do stuff
            Transform parent = hasBone.transform;
            Vector3 position = collision.contacts[0].point;
            if (hasBone.TargetBone != null) {
                parent = hasBone.TargetBone.transform;
                position = hasBone.TargetBone.transform.position;
            }
            success = true;
            PlaceRing(position, parent);
            //play a sound?

            //particle effects?

        } else {
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
        float timeout = fadeOutTime;
        List<Material> materials = new List<Material>();
        Renderer[] renders = GetComponentsInChildren<Renderer>();
        foreach(Renderer r in renders){
            materials.AddRange(r.materials);
        }
        while(timeout>0){
            foreach(Material m in materials){
                Color c = m.color;
                c.a = timeout / fadeOutTime;
                m.color = c;
            }
            timeout -= Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

}
