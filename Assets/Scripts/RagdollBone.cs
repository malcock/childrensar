using UnityEngine;
using System.Collections;
using UnityEngine.Events;



public class RagdollBone : MonoBehaviour
{

    public UnityEvent onCollisionStay;
    public CollisionEvent onCollisionEnter;
    public TriggerEvent onTriggerEnter;

    public Collider triggerCollider;

    private void Awake()
    {
        onCollisionEnter = new CollisionEvent();
        onCollisionStay = new UnityEvent();
        onTriggerEnter = new TriggerEvent();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != gameObject.layer)
        {
            if (onCollisionEnter != null) onCollisionEnter.Invoke(collision, gameObject);
        }
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
        if (other.gameObject.layer != gameObject.layer)
        {
            triggerCollider = other;
            if (onTriggerEnter != null) onTriggerEnter.Invoke(other, gameObject);
        }
    }
}
