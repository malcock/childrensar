﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DraggableObject), typeof(RagdollControl))]
public class MainCharacter : MonoBehaviour
{

    public enum CharacterType { Penguin, Otter }

    public enum State { Idle, Walking, Dragging, Dropped, Swimming, Escape } //swimming is further controlled, escape is used to jump off floats
    public enum SwimState { Free, Escape, Return, Jumping, Interval } //swim state - escape will go out the room, return is to find it's float, free allows to swim about

    public CharacterType characterType = CharacterType.Penguin;

    public bool useLongJump = false;

    public State state = State.Idle;

    public SwimState swimState = SwimState.Free;
    public bool isReady = false;
    public Transform myFloat;
    public Transform EscapeLocation;
    public float swimTimeout = 4;
    public int fishMax = 5;
    public int fishEaten = 0;
    public AnimationCurve massAnim;
    public float walkDistanceThreshold = 0.0f;
    float walkTimeout = 3;

    public Vector3 targetPosition = new Vector3();

    float swimTime;

    Transform currentLand;

    Animator anim;
    DraggableObject draggableObject;
    InteractableObject interactableObject;
    RagdollControl doll;

    SkinnedMeshRenderer skinnedMeshRenderer;

    Collider mainCollider;
    Bounds waterBounds;

    float dropTimeout = 1;
    public float bellyAmount = 0;

    bool firstFrame = false;
    public bool isGettingOutOfWater = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        draggableObject = GetComponent<DraggableObject>();
        interactableObject = GetComponent<InteractableObject>();
        doll = GetComponent<RagdollControl>();
        mainCollider = GetComponent<Collider>();
        waterBounds = GameObject.FindWithTag("Water").GetComponent<Collider>().bounds;
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        fishMax = GameControl.Instance.fishMax;

    }

    // Use this for initialization
    void Start()
    {
        interactableObject.OnDragStart.AddListener(StartDrag);
        interactableObject.OnDragEnd.AddListener(EndDrag);
        doll.onTriggerEnter.AddListener(HitWater);
        doll.onCollisionStay.AddListener(HitLand);
        doll.onTriggerExit.AddListener(LeaveWater);


        //start random behaviours
        StartCoroutine(Blink(Random.Range(5f, 10f), Random.Range(1, 3), Random.Range(2f, 10f)));
        StartCoroutine(IdleSound(Random.Range(20, 80)));
    }

    IEnumerator IdleSound(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        //can we cancel this depending on when the last event was called? In the last 5 seconds maybe?
        if(state==State.Idle){
            string eventName = (characterType == CharacterType.Penguin) ? "PenguinQuack" : "OtterVocal";
            if (swimState != SwimState.Escape)
            {
                AkSoundEngine.SetSwitch(eventName, "Idle", gameObject);
                AkSoundEngine.PostEvent(eventName, gameObject);
            }
            StartCoroutine(IdleSound(Random.Range(20, 80)));
            if (characterType == CharacterType.Penguin && state != State.Escape)
            {
                int id = Random.Range(0, 2);
                if(id==1){
                    AkSoundEngine.PostEvent("PenguinFootsteps", gameObject);
                }
                anim.SetTrigger("Idle" + id);
            }
        }

    }

    IEnumerator Blink(float waitTime, int blinkCount, float blinkSpeed)
    {
        
        yield return new WaitForSeconds(waitTime);
        int blinkIndex = characterType == CharacterType.Penguin ? 2 : 0;
        for (int times = 0; times < blinkCount; times++)
        {
            float blinkAmount = 0;
            while (blinkAmount < 100)
            {
                
                skinnedMeshRenderer.SetBlendShapeWeight(blinkIndex, blinkAmount);
                blinkAmount += blinkSpeed;
                yield return null;
            }
            while (blinkAmount > 0)
            {
                skinnedMeshRenderer.SetBlendShapeWeight(blinkIndex, blinkAmount);
                blinkAmount -= blinkSpeed;
                yield return null;
            }

            yield return null;
        }

        StartCoroutine(Blink(Random.Range(5f, 10f), Random.Range(1, 3), Random.Range(2f, 10f)));
    }

    IEnumerator StopDropThrough(){

        doll.hips.GetComponent<Rigidbody>().isKinematic = true;
        yield return null;
        doll.hips.position = new Vector3(0, 5, 0);
        yield return null;
        doll.hips.GetComponent<Rigidbody>().isKinematic = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (!isReady)
            return;
        dropTimeout -= Time.deltaTime;
        //if the character has fallen through the world somehow, we need to reset it 
        if (transform.position.y < -50)
        {
            transform.position = new Vector3(0, 5, 0);
        }
        if (doll.hips.position.y < -50)
        {
            StartCoroutine(StopDropThrough());

        }

        //drop character on start
        if (!firstFrame) doll.ragdolled = firstFrame = true;
        anim.SetBool("isSwimming", false);
        mainCollider.attachedRigidbody.useGravity = true;

        //if character set to flee when full, force swim and flee mode
        if (GameControl.Instance.CharacterBehaviour == GameControl.CharacterMode.Leave)
        {
            if (fishEaten >= fishMax)
            {
                if (state != State.Swimming)
                    state = State.Escape;
                swimState = SwimState.Escape;
            }
        }

        //basic controls for the penguin states
        switch (state)
        {
            case State.Idle:
                bool findCenter = false;
                walkTimeout -= Time.deltaTime;
                if (walkTimeout > 0)
                {
                    if (currentLand != null)
                    {
                        Vector3 landPos = currentLand.position;
                        //landPos.y = transform.position.y;
                        float distanceToCenter = Vector3.Distance(transform.position, landPos);

                        if (distanceToCenter > walkDistanceThreshold)
                        {
                            findCenter = true;
                            doll.ResetRagdoll();
                        }
                    }
                }

                if (findCenter)
                {
                    anim.SetFloat("speed", 1);
                    //walk to the centre of the object currently stood on
                    Rigidbody landBody = currentLand.GetComponent<Rigidbody>();
                    //if (landBody != null) landBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                    Vector3 landPos = currentLand.position;
                    landPos.y = transform.position.y;

                    Vector3 direction = landPos - transform.position;

                    Vector3 rot = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;

                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rot), Time.deltaTime * 3);

                    transform.position = Vector3.Lerp(transform.position, landPos, Time.deltaTime);
                    //direction.y = 1;
                    //direction = direction.normalized;
                    //direction *= 50;
                    //if(dropTimeout<0) mainCollider.attachedRigidbody.AddForce(direction);



                }
                else
                {
                    //rotate towards camera
                    Vector3 relativePosition = Camera.main.transform.position - transform.position;
                    Quaternion rotation = Quaternion.LookRotation(relativePosition);
                    rotation.x = 0;
                    rotation.z = 0;
                    transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 3);
                    anim.SetFloat("speed", 0);
                    if(currentLand!=null){
						Rigidbody landBody = currentLand.GetComponent<Rigidbody>();
                        if (landBody != null) landBody.constraints = RigidbodyConstraints.None;
                        
                    }
                }




                //switch to swim mode?
                if (doll.state == RagdollControl.RagdollState.animated)
                {
                    if (BoundsContainedPercentage(mainCollider.bounds, waterBounds) > 0.5f)
                    {
                        state = State.Swimming;
                        //AkSoundEngine.PostEvent("PenguinSwim", gameObject);

                        //should stopping this state be controlled elsewhere though?
                    }
                }

                break;
            case State.Dropped:
                //switch to swim mode?
                AkSoundEngine.PostEvent("PenguinSwimStop", gameObject);
                dropTimeout = 1;
                if (doll.state == RagdollControl.RagdollState.animated)
                {
                    if (BoundsContainedPercentage(mainCollider.bounds, waterBounds) > 0.5f)
                    {
                        state = State.Swimming;
                        //should stopping this state be controlled elsewhere though?
                    }
                    else
                    {
                        state = State.Idle;
                    }
                }
                break;
            case State.Swimming:
                if (BoundsContainedPercentage(mainCollider.bounds, waterBounds) < 0.1f)
                {
                    state = State.Dropped;
                    //should stopping this state be controlled elsewhere though?
                }
                if (doll.state == RagdollControl.RagdollState.animated)
                {


                    anim.SetBool("isSwimming", true);
                    mainCollider.attachedRigidbody.useGravity = false;
                    //targetPosition = new Vector3();
                    switch (swimState)
                    {
                        case SwimState.Free:
                            //choose a random position in the water to swim to
                            targetPosition = new Vector3(Random.Range(waterBounds.min.x, waterBounds.max.x),
                                      Random.Range(waterBounds.min.y, waterBounds.max.y),
                                      Random.Range(waterBounds.min.z, waterBounds.max.z));
                            targetPosition *= 0.7f;
                            swimTime = Time.time;
                            swimState = SwimState.Interval;
                            break;
                        case SwimState.Escape:
                            //use EscapeLocation as the target if set
                            if (EscapeLocation != null)
                            {
                                targetPosition = EscapeLocation.position;
                            }
                            else
                            {
                                //find a vector away from the center
                                targetPosition = new Ray(Vector3.zero, transform.position).GetPoint(7);
                                targetPosition.y = -0.5f;
                            }

                            break;
                        case SwimState.Return:
                            //return to a defined float
                            if (myFloat != null)
                            {
                                targetPosition = myFloat.transform.position;
                            }
                            //if we've been swimming ages, maybe we got stuck - lets try being free again for a bit
                            if (swimTime + (swimTimeout * 3) < Time.time)
                                swimState = SwimState.Free;

                            break;
                        case SwimState.Jumping:

                            break;
                        case SwimState.Interval:

                            if (swimTime + swimTimeout < Time.time)
                            {
                                Debug.Log("interval reset fish");
                                if (GameControl.Instance.CharacterBehaviour == GameControl.CharacterMode.Stay)
                                {
                                    fishEaten = 0;
                                    swimState = SwimState.Return;
                                }
                                else
                                {
                                    if (fishEaten < fishMax) swimState = SwimState.Return;
                                }



                            }


                            break;
                    }

                    if (swimState == SwimState.Return)
                    {
                        if (myFloat != null)
                        {
                            targetPosition = myFloat.transform.position;
                        }
                    }
                    //find new direction and rotate towards it
                    Vector3 direction = targetPosition - transform.position;

                    Vector3 rot = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
                    rot.x = 90;

                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rot), Time.deltaTime * 2);
                    //direction.y = 0;
                    direction = direction.normalized;
                    direction *= 8;
                    mainCollider.attachedRigidbody.AddForce(direction);

                    //now check how far away we are from our target
                    float distance = Vector3.Distance(transform.position, targetPosition);
                    float distanceCompare = 0.1f;
                    if (swimState == SwimState.Return) distanceCompare = 1f;
                    if (distance < distanceCompare)
                    {
                        switch (swimState)
                        {
                            case SwimState.Interval:
                                swimState = SwimState.Return;
                                break;
                            case SwimState.Return:
                                isGettingOutOfWater = true;
                                if (useLongJump)
                                {
                                    //jump only a little, but long time
                                    targetPosition.y += 0.5f;
                                    state = State.Dragging;
                                    draggableObject.Throw(targetPosition, 1f);
                                }
                                else
                                {
                                    //jump quite high, but for short time
                                    targetPosition.y += 3.5f;
                                    state = State.Dragging;
                                    draggableObject.Throw(targetPosition, 0.4f);
                                }

                                break;
                        }
                    }
                }

                break;
            case State.Escape:
                //this is to handle walking into the water or something 
                // use EscapeLocation as the target if set
                if (EscapeLocation != null)
                {
                    targetPosition = EscapeLocation.position;
                }
                else
                {
                    //find a vector away from the camera
                    targetPosition = new Ray(Vector3.zero, transform.position).GetPoint(7);
                    targetPosition.y = -0.5f;
                }
                //rotate to target
                Vector3 relDir = targetPosition - transform.position;
                Quaternion rota = Quaternion.LookRotation(relDir);
                rota.x = 0;
                rota.z = 0;
                transform.rotation = Quaternion.Slerp(transform.rotation, rota, Time.deltaTime * 3);
                if (doll.state == RagdollControl.RagdollState.animated)
                {
                    if (Quaternion.Angle(transform.rotation, rota) < 15f)
                    {
                        relDir.y += 3;
                        state = State.Dragging;
                        draggableObject.Throw(relDir, 0.15f);

                    }

                    if (BoundsContainedPercentage(mainCollider.bounds, waterBounds) > 0.5f)
                    {
                        state = State.Swimming;
                        //AkSoundEngine.PostEvent("PenguinSwim", gameObject);
                        //should stopping this state be controlled elsewhere though?
                    }
                }

                break;
            case State.Dragging:
                break;


        }

        //handle the blendshape and mass using the fish eaten values
        float mass = doll.totalMass + (((float)fishEaten / (float)fishMax) * (fishMax * 3));
        mass = doll.totalMass + (fishEaten * 2);
        mainCollider.attachedRigidbody.mass = Mathf.Lerp(mainCollider.attachedRigidbody.mass, mass, Time.deltaTime);


        float newWeight = fishEaten > 0 ? ((float)(fishEaten + 1) / (float)fishMax) * 100 : 0;

        int bellyBlend = (characterType == CharacterType.Penguin) ? 0 : 2;
        skinnedMeshRenderer.SetBlendShapeWeight(bellyBlend, bellyAmount);


    }

    private void LateUpdate()
    {

    }

    void StartDrag()
    {
        doll.ragdolled = true;
        state = State.Dragging;
        string eventName = (characterType == CharacterType.Penguin) ? "PenguinQuack" : "OtterVocal";
        if (swimState != SwimState.Escape)
        {
            AkSoundEngine.SetSwitch(eventName, "Tapped", gameObject);
            AkSoundEngine.PostEvent(eventName, gameObject);
        }
    }
    void EndDrag()
    {
        state = State.Dropped;
        string eventName = (characterType == CharacterType.Penguin) ? "PenguinQuack" : "OtterVocal";
        string switchName = isGettingOutOfWater ? "Climb" : "Thrown";
        if(swimState != SwimState.Escape){
			AkSoundEngine.SetSwitch(eventName, switchName, gameObject);
			AkSoundEngine.PostEvent(eventName, gameObject);
            
        }
        isGettingOutOfWater = false;
    }

    void HitWater(Collider other, GameObject obj)
    {
        //don't try to do a splash if already swimming
        if (state == State.Dragging) return;
        //set correct swimstate depending on what's happening?
        if (swimState != SwimState.Escape)
            swimState = (state == State.Escape) ? SwimState.Escape : SwimState.Free;
        //state = State.Swimming;
        Debug.Log(obj.name + " splash");
        MakeSplash(other, obj);

        //drop all fish to return fatness to normal
        if (GameControl.Instance.CharacterBehaviour == GameControl.CharacterMode.Stay)
        {
            Debug.Log("hit water reset fish");
            fishEaten = 0;
        }
        else
        {

        }


    }

    void LeaveWater(Collider other, GameObject obj)
    {

        MakeSplash(other, obj);
    }

    void MakeSplash(Collider other, GameObject obj)
    {
        if (other.tag == "Water")
        {

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            string splashSize = "SmallSoft";
            if (rb != null)
            {
                float intensity = rb.velocity.normalized.magnitude;
                if (intensity > 0.75f)
                {
                    splashSize = "Large";
                }
                else if (intensity > 0.5f)
                {
                    splashSize = "Medium";
                }
                else if (intensity > 0.15f)
                {
                    splashSize = "Small";
                }
                else
                {
                    splashSize = "SmallSoft";
                }
            }
            //if(swimState!=SwimState.Escape){
                Splash splash = Instantiate(Resources.Load("Splash", typeof(Splash)), obj.transform.position, Quaternion.identity) as Splash;
                splash.splashSize = splashSize;
            //}

        }
    }



    void HitLand(Collision collision, GameObject obj)
    {
        if (state != State.Dragging)
        {
            walkTimeout = 5;
            currentLand = collision.transform;

            doll.ragdolled = false;
            state = State.Idle;
            if (currentLand.name == "Bed_01" || currentLand.name=="FishBucket_01")
            {
                state = State.Escape;
            }
            PlayOneShot("InteractPenguinDrop");
        }

    }


    public void PlayOneShot(string AKEventName)
    {
        AkSoundEngine.PostEvent(AKEventName, gameObject);
    }

    public void Speak()
    {
        string eventName = (characterType == CharacterType.Penguin) ? "PenguinQuack" : "OtterVocal";
        if (swimState != SwimState.Escape)
        {
            AkSoundEngine.SetSwitch(eventName, "Call", gameObject);
            AkSoundEngine.PostEvent(eventName, gameObject);

        }
        string triggerName = "Speak";
        if(characterType==CharacterType.Otter){
            if (Random.value >= 0.5f) triggerName = "Speak2";
        }
        anim.SetTrigger(triggerName);
    }

    public void Catch()
    {
        anim.SetTrigger("Catch");
        if(swimState != SwimState.Escape){
            string eventName = (characterType == CharacterType.Penguin) ? "PenguinQuack" : "OtterVocal";
            AkSoundEngine.SetSwitch(eventName, "Eat", gameObject);
            AkSoundEngine.PostEvent(eventName, gameObject);
        }


        fishEaten++;
        StartCoroutine(AnimateWeight(1f, ((float)fishEaten/(float)fishMax) * 100));

        //mainCollider.attachedRigidbody.mass = doll.totalMass + (fishEaten);
    }

    IEnumerator AnimateWeight(float timeout, float stop)
    {
        Debug.Log("Anim belly to " + stop);
        float t = timeout;
        float start = bellyAmount;

        while (t > 0)
        {
            float p = 1 - (t / timeout);
            float e = massAnim.Evaluate(p);
            //Debug.Log(e);
            float val = start + (e * stop);
            bellyAmount = val;
            t -= Time.deltaTime;
            yield return null;
        }

            
    }

    IEnumerator EscapeAttempt(){
        yield return new WaitForSeconds(0.5f);
        state = State.Escape;
    }

    private float BoundsContainedPercentage(Bounds obj, Bounds region)
    {
        var total = 1f;

        for (var i = 0; i < 3; i++)
        {
            var dist = obj.min[i] > region.center[i] ?
                obj.max[i] - region.max[i] :
                region.min[i] - obj.min[i];

            total *= Mathf.Clamp01(1f - dist / obj.size[i]);
        }

        return total;
    }
}
