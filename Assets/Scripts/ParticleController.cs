using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractableObject))]
public class ParticleController : MonoBehaviour {

    public ParticleSystem particles;
    public string soundFx;
    public float timeOut = 0.5f;

    bool available = true;

	// Use this for initialization
	void Start () {
        if (particles == null) Debug.LogWarning(name + " missing an assigned particle system");
        particles.Stop();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ActivateParticles(){
        if(available){
            if(soundFx!=string.Empty)
                AkSoundEngine.PostEvent(soundFx,gameObject);
            particles.Play();
            available = false;
            StartCoroutine(Timeout());
        }
            
    }

    IEnumerator Timeout(){
        yield return new WaitForSeconds(timeOut);
        available = true;
    }
}
