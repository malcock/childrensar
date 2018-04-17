using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AttractController : MonoBehaviour {

    public enum AttractState { Polar, Aline }

    public AttractState attractMode = AttractState.Polar;
    public string LevelToLoad = "Burns Stencil";

	// Use this for initialization
	void Start () {
        if(attractMode == AttractState.Polar){
            AkSoundEngine.PostEvent("PolarAmbience", gameObject);
        }

	}

    // Update is called once per frame
    void Update()
    {
        
        if(SystemInfo.batteryStatus == BatteryStatus.Discharging){
            //LoadLevel();
        }

        if (Input.GetKeyUp(KeyCode.Return))
        {
            Debug.Log("Manual Switch");
            LoadLevel();
        }
    }

    public void LoadLevel(){
        GameControl.Instance.LoadScene(LevelToLoad);
    }
}
