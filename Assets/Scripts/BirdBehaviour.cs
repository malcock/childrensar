using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(InteractableObject))]
public class BirdBehaviour : MonoBehaviour
{

    private InteractableObject interactableObj;

    public enum BirdType {Seagull, Duck}
    public BirdType type = BirdType.Seagull;
	// Use this for initialization
	void Start()
	{
        interactableObj = GetComponent<InteractableObject>();
        interactableObj.OnTap.AddListener(TapSpeak);
        //AkSoundEngine.PostEvent("SeagullFly", gameObject);
        StartCoroutine(Speak(Random.Range(30, 50)));
	}

    IEnumerator Speak(float waitTime){
        yield return new WaitForSeconds(waitTime);
        string eventName = (type==BirdType.Seagull) ? "SeagullVocal" : "DuckVocal";
        AkSoundEngine.PostEvent(eventName, gameObject);
        StartCoroutine(Speak(Random.Range(30,50)));
    }

    public void TapSpeak(){
        string eventName = (type == BirdType.Seagull) ? "SeagullVocal" : "DuckVocal";
        AkSoundEngine.PostEvent(eventName, gameObject);
    }

	// Update is called once per frame
	void Update()
	{
			
	}
}
