using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Splashable : MonoBehaviour {

    Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Water")
        {
            if(rb!=null){
                
            }
            AkSoundEngine.SetSwitch("WaterSplash", "SmallSoft", gameObject);
            AkSoundEngine.PostEvent("WaterSplash", gameObject);
            GameObject splash = Instantiate(Resources.Load("Splash", typeof(GameObject)), transform.position, Quaternion.identity) as GameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Water")
        {
            GameObject splash = Instantiate(Resources.Load("Splash", typeof(GameObject)), transform.position, Quaternion.identity) as GameObject;
        }
    }


}
