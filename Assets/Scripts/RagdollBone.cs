using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[System.Serializable]
public class TriggerEvent : UnityEvent<Collider,GameObject>{}

public class RagdollBone : MonoBehaviour
{

    public UnityEvent onCollisionStay;
    public TriggerEvent onTriggerEnter;

    public Collider triggerCollider;

    private void Awake()
    {
        onCollisionStay = new UnityEvent();
        onTriggerEnter = new TriggerEvent();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer != gameObject.layer)
        {
            if (onCollisionStay != null) onCollisionStay.Invoke();
            //Debug.Log(name + " with " + collision.gameObject.name + " layer " + collision.gameObject.layer);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer!=gameObject.layer){
            triggerCollider = other;
            if (onTriggerEnter != null) onTriggerEnter.Invoke(other,gameObject);
        }
    }
}
