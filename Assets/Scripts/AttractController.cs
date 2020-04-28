using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AttractController : MonoBehaviour {

    public enum AttractState { Polar, Alpine }

    public AttractState attractMode = AttractState.Polar;
    public string LevelToLoad = "Burns Stencil";
    bool isUp = false;
    bool isBlack = false;

    IEnumerator blackFade;

    public CanvasGroup fadeGroup;
    Image fadeImage;
    public float fadeOutTime = 2; 

	// Use this for initialization
	void Start () {
        AkSoundEngine.SetRTPCValue("MasterVolume", 100);
        if(attractMode == AttractState.Polar){
            AkSoundEngine.PostEvent("PolarAmbience", gameObject);
        } else {
            AkSoundEngine.PostEvent("AlpineAmbience", gameObject);
        }

        fadeImage = fadeGroup.GetComponentInChildren<Image>();
        LoadLevel();
	}

    // Update is called once per frame
    void Update()
    {
        
        if(SystemInfo.batteryStatus == BatteryStatus.Discharging && !isUp){
            LoadLevel();
            isUp = true;
        }

        if (Input.GetKeyUp(KeyCode.Return))
        {
            Debug.Log("Manual Switch");
            LoadLevel();
        }
    }

    public void LoadLevel(){

        StartCoroutine(playhandler());
    }

    public void FadeScreen(){
        if (blackFade != null) StopCoroutine(blackFade);
        if(isBlack){
            blackFade = fadeFromBlack();
        } else {
            blackFade = fadeToBlack();
        }
        StartCoroutine(blackFade);
    }

    IEnumerator fadeFromBlack(){
        isBlack = false;
        float timeout = fadeOutTime;
        float audioFade = 0;
        while (timeout > 0)
        {
            fadeGroup.alpha = (timeout / fadeOutTime);
            audioFade = 1-(timeout / fadeOutTime);
            AkSoundEngine.SetRTPCValue("MasterVolume", audioFade * 100);
            timeout -= Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator fadeToBlack(){
        isBlack = true;
        fadeImage.color = Color.black;
        float timeout = fadeOutTime;
        float audioFade = 1;
        while(timeout>0){
            fadeGroup.alpha = 1 - (timeout / fadeOutTime);
            audioFade =  (timeout / fadeOutTime);
            AkSoundEngine.SetRTPCValue("MasterVolume", audioFade * 100);
            timeout -= Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator playhandler(){
        fadeImage.color = Color.white;
        //AkSoundEngine.PostEvent("TabletPickup", gameObject);
        float timeout = fadeOutTime;
        //Vector3 origScale = transform.localScale;
        while (timeout > 0)
        {
            //transform.localScale = origScale * (timeout / fadeOutTime);

            fadeGroup.alpha = 1 - (timeout / fadeOutTime);
            timeout -= Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1);
        float audioFade = 1;
        while(audioFade>0){
            AkSoundEngine.SetRTPCValue("MasterVolume", audioFade * 100);
            audioFade -= Time.deltaTime;
            yield return null;
        }
        AkSoundEngine.StopAll();
        GameControl.Instance.LoadScene(LevelToLoad);
    }
}
