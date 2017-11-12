using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class RagdollBone : MonoBehaviour
{
    
    public UnityEvent onCollisionStay;

    private void Awake()
    {
        onCollisionStay = new UnityEvent();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer != gameObject.layer)
        {
            if (onCollisionStay != null) onCollisionStay.Invoke();
            Debug.Log(name + " with " + collision.gameObject.name + " layer " + collision.gameObject.layer);
        }
    }

}
