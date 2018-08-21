using UnityEngine;
using System.Collections;
using BuoyancyToolkit;

[RequireComponent(typeof(BuoyancyToolkit.BuoyancyForce),typeof(ConstantForce),typeof(Rigidbody))]
public class FloatControl : MonoBehaviour
{
    public AnimationCurve floatCurve, sinkCurve;
    public float delay, changeTime, force = 500;
    [SerializeField]
    private bool _lockInPlace = true;
    public bool lockInPlace {
        get { return _lockInPlace; }
        set {
            if (_lockInPlace == value) return;
            _lockInPlace = value;
            if(_lockInPlace){
                
                StartCoroutine(Float());

            } else {
                
                StartCoroutine(Sink());
                //buoy.BuoyancyCollider.enabled = false;

            }
        }
    }
    public Vector3 startPosition;
    ConstantForce constantForce;
    Transform myTransform;

    BuoyancyForce buoy;
	// Use this for initialization
	void Start()
	{
        startPosition = transform.Find("Target").position;
        constantForce = GetComponent<ConstantForce>();
        myTransform = transform;
        buoy = GetComponent<BuoyancyForce>();

	}

    IEnumerator Float(){
        
        //Debug.Log(name + " float");
        yield return new WaitForSeconds(delay);
        AkSoundEngine.SetSwitch("PlatformUpDown", "PlatformUp", gameObject);
        AkSoundEngine.PostEvent("PlatformUpDown", gameObject);
        buoy.BuoyancyCollider.enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        float startWeight = buoy.WeightFactor;
        float timeout = changeTime;
        while(timeout>0){
            buoy.WeightFactor = Mathf.Lerp(startWeight,1.5f,floatCurve.Evaluate(1 - (timeout / changeTime)));
            timeout -= Time.deltaTime;
            yield return null;

        }
    }

    IEnumerator Sink()
    {
        //Debug.Log(name + " sink");
        yield return new WaitForSeconds(delay);
        AkSoundEngine.SetSwitch("PlatformUpDown", "PlatformUp", gameObject);
        AkSoundEngine.PostEvent("PlatformUpDown", gameObject);
        float timeout = changeTime;
        while (timeout > 0)
        {
            buoy.WeightFactor = 1.5f * sinkCurve.Evaluate(1 - (timeout / changeTime));
            timeout -= Time.deltaTime;
            yield return null;

        }
        buoy.BuoyancyCollider.enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }

	// Update is called once per frame
	void Update()
	{
        if(transform.position.y<-5){
            Vector3 pos = transform.position;
            pos.y = 5;
            transform.position = pos;
        }
        if(lockInPlace){
            Vector3 direction = startPosition - myTransform.position;
            direction.y = 0;
            constantForce.force = direction * force;

        } else {
            //move away from the play area if the octopus/bears are in play

            //buoy.BuoyancyCollider.enabled = false;
            Vector3 direction = new Ray(Vector3.zero, transform.position).GetPoint(10);
            direction.y = 0;
            constantForce.force = direction;
        }

	}
}
