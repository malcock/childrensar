using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Splashable : MonoBehaviour {

    Rigidbody rb;

    public string ForceSize = string.Empty;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (!isActiveAndEnabled) return;
        if (other.tag == "Water")
        {
            string splashSize = "SmallSoft";
            if (ForceSize == string.Empty)
            {
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
            } else {
                splashSize = ForceSize;
            }

            Splash splash = Instantiate(Resources.Load("Splash", typeof(Splash)), transform.position, Quaternion.identity) as Splash;
            splash.splashSize = splashSize;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!isActiveAndEnabled) return;
        if (other.tag == "Water")
        {
            GameObject splash = Instantiate(Resources.Load("Splash", typeof(GameObject)), transform.position, Quaternion.identity) as GameObject;
        }
    }


}
