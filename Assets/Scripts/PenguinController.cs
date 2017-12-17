using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(DraggableObject), typeof(AudioSource), typeof(RagdollHelper))]
public class PenguinController : MonoBehaviour
{


    Animator anim;

    InteractableObject interactableObj;
    DraggableObject dragObj;
    AudioSource audioSource;

    public GameObject myFloe;

    RagdollHelper doll;


    bool firstFrame = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        dragObj = GetComponent<DraggableObject>();
        audioSource = GetComponent<AudioSource>();
        interactableObj = GetComponent<InteractableObject>();

    }

    // Use this for initialization
    void Start()
    {
        anim.Play("IDLE_02", -1, Random.Range(0f, 1f));

        doll = GetComponent<RagdollHelper>();
        doll.onCollision.AddListener(OnCollision);
        doll.onTrigger.AddListener(OnTrigger);

        interactableObj.OnDragStart.AddListener(StartedDrag);
    }

    // Update is called once per frame
    void Update()
    {
        if (!firstFrame) doll.ragdolled = firstFrame = true;
        if(!doll.ragdolled && !anim.GetBool("isSwimming")){
            //find the camera and point the penguin at it
            Vector3 relativePosition = Camera.main.transform.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePosition);
            rotation.x = 0;
            rotation.z = 0;
            transform.rotation = rotation;
        }
    }

    public void Quack()
    {
        Debug.Log(name + " quack");
        anim.SetTrigger("Quack");
        //audioSource.clip = quack;
        //audioSource.Play();
        AkSoundEngine.SetSwitch("PenguinQuack","QuackIdle",gameObject);
        AkSoundEngine.PostEvent("PenguinQuack",gameObject);

        //PlaySound(quack);
        //anim.SetBool("isCatching", false);


    }

    public void Eat(){
        anim.SetTrigger("Catch");
        Debug.Log(name + " nom!");
        GetComponent<Rigidbody>().mass+=5;
    }

    private void StartedDrag()
    {
        doll.ragdolled = true;
        AkSoundEngine.PostEvent("PenguinSqueak",gameObject);
    }



    private void OnCollision()
    {
        if (!interactableObj.isDragging)
        {
            doll.ragdolled = false;
            //PlaySound(squeak);

            AkSoundEngine.PostEvent("PenguinSqueak",gameObject);
        }
        else
        {
            //can we make it match the height and position of the object?
        }
    }



    private void OnCollisionEnter(Collision collision)
    {

    }




    private void OnTrigger(Collider other, GameObject obj)
    {
        if(other.tag=="Water"){
            Debug.Log("ragdoll hit water");
            //make a splash?
            GameObject splash = Instantiate(Resources.Load("Splash",typeof(GameObject)),obj.transform.position,Quaternion.identity) as GameObject;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!interactableObj.isDragging)
        {
            Debug.Log(name + " landed?");

            doll.ragdolled = false;


        }
        //make a splash?
        if(other.tag=="Water"){
            Debug.Log(GetComponent<Rigidbody>().velocity);
        }


    }

    private void OnTriggerStay(Collider other)
    {
        //Water layer is built in layer 4
        if (other.tag == "Water")
        {
            if (BoundsContainedPercentage(GetComponent<Collider>().bounds, other.bounds) > 0.5f)
            {
                if (!anim.GetBool("isSwimming"))
                {
                    anim.SetBool("isSwimming", true);
                    doll.ResetRagdoll();
                    StartCoroutine(Swim());
                }


                //need to use the drag controller 
                //dragObj.DragTo(Vector3.zero);
            }




        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Water")
        {
            anim.SetBool("isSwimming", false);
            GetComponent<Rigidbody>().useGravity=true;
            GetComponent<Rigidbody>().isKinematic=false;
            StopCoroutine(FindLand());
            StopAllCoroutines();
        }
    }

    IEnumerator Swim(){
        //choose a random location to swim to and go to it before doing a come home
        GameObject water = GameObject.FindWithTag("Water");
        Bounds waterBounds = water.GetComponent<Collider>().bounds;
        Vector3 pos = new Vector3(Random.Range(waterBounds.min.x, waterBounds.max.x),
                                  Random.Range(waterBounds.min.y, waterBounds.max.y),
                                  Random.Range(waterBounds.min.z, waterBounds.max.z));
        pos *= 0.9f;
        Rigidbody rb = GetComponent<Rigidbody>();
        float timeout = 4;
        while(timeout>0){
            rb.useGravity = false;
            Vector3 direction = pos - transform.position;

            Vector3 rot = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
            rot.x = 90;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rot), Time.deltaTime*2);
            direction.y = 0;
            direction = direction.normalized;
            direction *= 8;
            rb.AddForce(direction);
            timeout -= Time.deltaTime;
            yield return null;
        }

        StartCoroutine(FindLand());
    }

    IEnumerator FindLand()
    {
        Debug.Log("finding land...");
        //find the closest landmass - need to tag suitable targets with "Land"
        List<GameObject> gos = GameObject.FindGameObjectsWithTag("Land").ToList();
        GameObject closest = gos[0];

        List<KeyValuePair<GameObject, float>> distances = new List<KeyValuePair<GameObject, float>>();
        foreach (GameObject g in gos)
        {
            distances.Add(new KeyValuePair<GameObject, float>(g,Vector3.Distance(transform.position,g.transform.position)));
        }
        //actually select the 2nd closest one
        distances.Sort((a, b) => a.Value.CompareTo(b.Value));
        if(distances[0].Value<0.25f){
            closest = distances[1].Key;
        } else {
            closest = distances[0].Key;
        }

        if(myFloe!=null){
            closest = myFloe;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        Debug.Log(closest.name);
        float distance = Vector3.Distance(new Vector3(transform.position.x,0,transform.position.z), new Vector3(closest.transform.position.x,0,closest.transform.position.z));
        while(distance>1f){
            
            rb.useGravity = false;
            Vector3 direction = (closest.transform.position - transform.position);

            Vector3 rot = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
            rot.x = 90;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rot), Time.deltaTime * 3);

            direction.y = 0;
            direction *= 10;
            rb.AddForce(direction);
            distance = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(closest.transform.position.x, 0, closest.transform.position.z));
            yield return null;
        }
        Debug.Log("land reached");
        Vector3 targ = closest.transform.position;
        targ.y += 1f;
        rb.useGravity = true;
        StartCoroutine(JumpToLand(targ));


        Debug.Log("No longer swimming");
    }

    IEnumerator JumpToLand(Vector3 position){
        interactableObj.OnDragStart.Invoke();
        interactableObj.locked = true;
        interactableObj.selected = true;
        interactableObj.isDragging = true;

        Debug.Log(name + " attempting jump");
        interactableObj.selected = true;
        interactableObj.isDragging = true;
        float timeout = 0.65f;
        while(timeout>0){
            dragObj.DragTo(position);
            timeout -= Time.deltaTime;
            yield return null;
        }

        Debug.Log(name + " jumped");
        interactableObj.locked = false;
        interactableObj.selected = false;
        interactableObj.isDragging = false;
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
