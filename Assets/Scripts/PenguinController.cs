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


    RagdollHelper doll;

    List<float> audioData = new List<float>();

    public AudioClip quack, squeak;

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


        anim.Play("IDLE_01", -1, Random.Range(0f, 1f));

        doll = GetComponent<RagdollHelper>();
        doll.onCollision.AddListener(OnCollision);
        interactableObj.OnDragStart.AddListener(StartedDrag);
    }

    // Update is called once per frame
    void Update()
    {
        if (!firstFrame) doll.ragdolled = firstFrame = true;
        if (Input.GetKeyUp(KeyCode.B)) doll.ragdolled = !doll.ragdolled;
    }

    public void Quack()
    {
        if (anim.GetBool("isCatching")) return;

        anim.SetBool("isCatching", true);
        //audioSource.clip = quack;
        //audioSource.Play();
        PlaySound(quack);
        StartCoroutine(EndQuack());
        //anim.SetBool("isCatching", false);

    }

    private void StartedDrag()
    {
        doll.ragdolled = true;
    }

    public void Squeak()
    {
        PlaySound(squeak);
    }

    public void PlaySound(AudioClip clip)
    {
        float[] samples = new float[clip.samples * clip.channels];

        clip.GetData(samples, 0);

        for (int i = 0; i < samples.Length; i++)
        {
            if (audioData.Count > i)
            {
                audioData[i] += samples[i];
            }
            else
            {
                audioData.Add(samples[i]);
            }
        }
        audioData = samples.ToList();
        Debug.Log(audioData.Count);

    }

    private void OnCollision()
    {
        if (!interactableObj.isDragging)
        {
            doll.ragdolled = false;
            //PlaySound(squeak);
        }
        else
        {
            //can we make it match the height and position of the object?
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

    }

    private void OnAudioFilterRead(float[] data, int channels)
    {

        for (int i = 0; i < data.Length - 1; i += 2)
        {
            if (audioData.Count <= 0)
                return;
            data[i] = audioData[0];
            data[i + 1] = audioData[0];
            audioData.RemoveAt(0);


        }
        Debug.Log(audioData.Count);
    }

    private IEnumerator EndQuack()
    {
        yield return new WaitForSeconds(0.25f);
        anim.SetBool("isCatching", false);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!interactableObj.isDragging)
        {
            Debug.Log(name + " landed?");

            doll.ragdolled = false;
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
                    StartCoroutine(FindLand());
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
            GetComponent<Rigidbody>().useGravity=true;;
            StopCoroutine(FindLand());
        }
    }

    IEnumerator FindLand()
    {

        while (anim.GetBool("isSwimming"))
        {
            Debug.Log("finding land...");
            //find the closest landmass - need to tag suitable targets with "Land"
            List<GameObject> gos = GameObject.FindGameObjectsWithTag("Land").ToList();
            GameObject closest = gos[0];

            for (int g = 1; g < gos.Count; g++)
            {
                if (Vector3.Distance(transform.position, closest.transform.position) > Vector3.Distance(transform.position, gos[g].transform.position))
                {
                    closest = gos[g];
                }
            }
            //we have the closest thing now, lets come up with a vector to it
            Debug.Log(closest.name);
            Quaternion lookRot = Quaternion.LookRotation(closest.transform.position - transform.position);
            transform.rotation = lookRot;
            GetComponent<Rigidbody>().useGravity = false;
            Vector3 force = new Vector3(0, 10, 20);
            if (Vector3.Distance(closest.transform.position, transform.position) < 0.75f){
                GetComponent<Rigidbody>().AddForce(new Vector3(0, 3, 1), ForceMode.Impulse);
            } else {
                GetComponent<Rigidbody>().AddForce(new Vector3(0, 10, 20), ForceMode.Force);
            }

            yield return new WaitForSeconds(1);
        }

        Debug.Log("No longer swimming");
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
