using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoopSplatControl : MonoBehaviour {

    CanvasGroup poopGroup;
    public float splatFadeIn = 0.5f, splatFadeOut = 5, poopSlide=40,poopSpread=0.15f;
    Vector3 poopOrig;

	// Use this for initialization
	void Start () {
        poopGroup = GetComponent<CanvasGroup>();
        poopOrig = poopGroup.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShowSplat(){
        StartCoroutine(DoSplat());
    }

    IEnumerator DoSplat(){
        AkSoundEngine.PostEvent("SeagullPoop",gameObject);
        Vector3 poopPos = poopGroup.transform.position;
        Vector3 poopScale = Vector3.one;

        float timeout = splatFadeIn;
        while(timeout>0){
            poopGroup.alpha = 1 - timeout / splatFadeIn;
            timeout -= Time.deltaTime;
            yield return null;
        }

        timeout = splatFadeOut;
        while (timeout > 0)
        {
            poopGroup.alpha = timeout / splatFadeIn;
            poopPos.y = poopOrig.y - (poopSlide * (1 - (timeout / splatFadeOut)));
            poopScale.y=1 + (poopSpread * (1 - (timeout / splatFadeOut)));

            poopGroup.transform.position = poopPos;
            poopGroup.transform.localScale = poopScale;

            timeout -= Time.deltaTime;
            yield return null;
        }

        poopGroup.alpha = 0;
        poopGroup.transform.position = poopOrig;
        poopGroup.transform.localScale = Vector3.one;
    }
}
