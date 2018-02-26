using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splash : MonoBehaviour {

    ParticleSystem ps;
    public string splashSize = "SmallSoft";

	// Use this for initialization
	void Start () {
        ps = GetComponent<ParticleSystem>();
        ps.Play();
        if(splashSize!=""){
			AkSoundEngine.SetSwitch("WaterSplash", splashSize, gameObject);
			AkSoundEngine.PostEvent("WaterSplash", gameObject);
            
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (!ps.IsAlive(true)){
            Destroy(gameObject);
        }
	}
}
