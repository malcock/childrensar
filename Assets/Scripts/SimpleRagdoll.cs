using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRagdoll : MonoBehaviour
{

    public bool ragdolled
    {
        get { return _ragdolled; }
        set
        {
            _ragdolled = value;

            if(_ragdolled){
                
            }
        }
    }

    private bool _ragdolled;

    // Use this for initialization
    void Awake()
    {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            if(rb.transform != transform){
                RagdollBone r = rb.gameObject.AddComponent<RagdollBone>();
                r.onCollisionStay.AddListener(onCollisionStay);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void onCollisionStay()
    {

    }
}
