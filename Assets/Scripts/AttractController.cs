using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AttractController : MonoBehaviour {

    public enum AttractState { Polar, Alpine }

    public AttractState attractMode = AttractState.Polar;
    public string LevelToLoad = "Burns Stencil";

	// Use this for initialization
	void Start () {
        if(attractMode == AttractState.Polar){
            AkSoundEngine.PostEvent("PolarAmbience", gameObject);
        } else {
            AkSoundEngine.PostEvent("AlpineAmbience", gameObject);
        }

	}

    // Update is called once per frame
    void Update()
    {
        
        if(SystemInfo.batteryStatus == BatteryStatus.Discharging){
            LoadLevel();
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

    public IEnumerator playhandler(){
        AkSoundEngine.PostEvent("TabletPickup", gameObject);
        yield return new WaitForSeconds(3);
        GameControl.Instance.LoadScene(LevelToLoad);
    }
}
