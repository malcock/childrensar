using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour {

    public float fadeTime=2, arrowWait=5, arrowTime=4, tapWait=20, tapTime=4, swipeWait=40, swipeTime=6;
    public CanvasGroup arrows;
    public SpriteRenderer tap;
    public SpriteRenderer swipe;

	// Use this for initialization
	void Start () {
        StartCoroutine(arrowHandler());
        StartCoroutine(SpriteHandler(tap,tapWait,tapTime));
        StartCoroutine(SpriteHandler(swipe,swipeWait,swipeTime));
	}

    IEnumerator arrowHandler(){
        arrows.alpha = 0;
        yield return new WaitForSeconds(arrowWait);
        float timeout = fadeTime;
        while(timeout>0){
            arrows.alpha = 1 - (timeout / fadeTime);
            timeout -= Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(arrowTime);
        timeout = fadeTime;
        while(timeout>0){
            arrows.alpha = timeout / fadeTime;
            timeout -= Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator SpriteHandler(SpriteRenderer s, float wait, float hold){
        Color col = s.color;
        col.a = 0;
        s.color = col;
        yield return new WaitForSeconds(wait);
        float timeout = fadeTime;

        while(timeout>0){
            col.a = 1 - (timeout / fadeTime);
            s.color = col;
            timeout -= Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(hold);
        timeout = fadeTime;
        while(timeout>0){
            col.a = timeout / fadeTime;
            s.color = col;
            timeout -= Time.deltaTime;
            yield return null;

        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
