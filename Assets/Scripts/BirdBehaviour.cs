using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BirdBehaviour : MonoBehaviour
{
    public enum BirdType {Seagull, Duck}
    public BirdType type = BirdType.Seagull;
	// Use this for initialization
	void Start()
	{

        AkSoundEngine.PostEvent("SeagullFly", gameObject);
        StartCoroutine(Speak(Random.Range(30, 50)));
	}

    IEnumerator Speak(float waitTime){
        yield return new WaitForSeconds(waitTime);
        string eventName = (type==BirdType.Seagull) ? "SeagullVocal" : "DuckVocal";
        AkSoundEngine.PostEvent(eventName, gameObject);
        StartCoroutine(Speak(Random.Range(30,50)));
    }

	// Update is called once per frame
	void Update()
	{
			
	}
}
