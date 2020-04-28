using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Edible. 
/// Applied to an object that will be eaten by a creature - could it be a fish? or can I make it work for a ring toss too?
/// </summary>
[RequireComponent(typeof(InteractableObject))]
public class Edible : MonoBehaviour
{
    Material mat;
    bool hasThrown = false;
    bool hasOwnCollider = false;
    public bool isSalmon = false;
    InteractableObject interactableObj;
    bool hasPickedUp = false;
    // Use this for initialization
    void Start()
    {
        if (GetComponent<SimpleRagdoll>() != null)
        {
            foreach (RagBone bone in GetComponentsInChildren<RagBone>())
            {
                bone.onCollisionEnter.AddListener(EatMe);
                bone.onTriggerEnter.AddListener(InWater);
            }

        }
        mat = GetComponentInChildren<MeshRenderer>().material;
        interactableObj = GetComponent<InteractableObject>();

        interactableObj.OnDragStart.AddListener(DragStart);
        interactableObj.OnDrag.AddListener(Drag);
        interactableObj.OnDragEnd.AddListener(DragEnd);

        if (isSalmon) StartCoroutine(FishSlap());
    }

    IEnumerator FishSlap()
    {

        while (!hasPickedUp)
        {
            AkSoundEngine.PostEvent("AnimationSalmonFlap", gameObject);
            yield return new WaitForSeconds(1.5f);
        }
    }

    void DragStart()
    {
        if (!isSalmon && !hasPickedUp)
        {
            Debug.Log("HEY HEY HEY");
            AkSoundEngine.PostEvent("InteractFishPickup", gameObject);
            AkSoundEngine.PostEvent("InteractFishPickupLoop", gameObject);
            hasPickedUp = true;
        }
        if (isSalmon && !hasThrown)
        {
            hasPickedUp = true;
        }
        InteractableObject[] iObjs = FindObjectsOfType<InteractableObject>();
        foreach (InteractableObject i in iObjs)
        {
            i.enabled = false;
        }
        this.enabled = true;
    }

    void Drag()
    {

    }

    void DragEnd()
    {
        if (!hasThrown)
        {
            if (!isSalmon)
            {
                //AkSoundEngine.PostEvent("InteractFishThrow", gameObject);
                //AkSoundEngine.PostEvent("InteractFishPickupLoopStop", gameObject);

            }

            hasThrown = true;
        }
        InteractableObject[] iObjs = FindObjectsOfType<InteractableObject>();
        foreach (InteractableObject i in iObjs)
        {
            i.enabled = true;
        }
        StartCoroutine(timeout());
    }

    // Update is called once per frame
    void Update()
    {

    }

    void InWater(Collider other, GameObject obj)
    {
        InteractableObject[] iObjs = FindObjectsOfType<InteractableObject>();
        foreach (InteractableObject i in iObjs)
        {
            i.enabled = true;
        }
        if (other.tag == "Water")
        {
            StartCoroutine(timeout());

        }
    }

    void EatMe(Collision collision, GameObject other)
    {
        Debug.Log(name + " eat?");
        Eater eater = collision.gameObject.GetComponent<Eater>();
        if (eater != null)
        {
            if (eater.OnFoodReceived != null) eater.OnFoodReceived.Invoke();
            Destroy(gameObject, 0.075f);
        }
    }

    IEnumerator timeout()
    {
        InteractableObject[] iObjs = FindObjectsOfType<InteractableObject>();
        foreach (InteractableObject i in iObjs)
        {
            i.enabled = true;
        }
        float fadeOutTime = 2;
        float timed = fadeOutTime;
        yield return new WaitForSeconds(2);
        while (timed > 0)
        {
            mat.SetFloat("_Cutoff", 1 - (timed / fadeOutTime));

            timed -= Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}

