using UnityEngine;
using System.Collections;
using UnityEngine.Events;



public class RagBone : MonoBehaviour
{

    public CollisionEvent onCollisionEnter, onCollisionStay, onCollisionExit;
    public TriggerEvent onTriggerEnter, onTriggerStay, onTriggerExit;

    private void Awake()
    {
        onCollisionEnter = new CollisionEvent();
        onCollisionStay = new CollisionEvent();
        onCollisionExit = new CollisionEvent();
        onTriggerEnter = new TriggerEvent();
        onTriggerStay = new TriggerEvent();
        onTriggerExit = new TriggerEvent();
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
            if (onCollisionStay != null) onCollisionStay.Invoke(collision, gameObject);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer != gameObject.layer)
        {
            if (onCollisionExit != null) onCollisionExit.Invoke(collision, gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != gameObject.layer)
        {

            if (onTriggerEnter != null) onTriggerEnter.Invoke(other, gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer != gameObject.layer)
        {
            if (onTriggerStay != null) onTriggerStay.Invoke(other, gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != gameObject.layer)
        {
            if (onTriggerExit != null) onTriggerExit.Invoke(other, gameObject);
        }
    }
}
