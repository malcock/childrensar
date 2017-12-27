using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolarManager : MonoBehaviour {

    public PenguinController[] penguins;
    public FloeController[] floes;

    public WhalePath whale;
    public OctopusController octopus;
    public float WhaleTimeout = 120;
    public float OctopusTimeout = 240;
    public float OctopusGameLength = 240;

	// Use this for initialization
	void Awake () {
        whale.timeout = WhaleTimeout;
        octopus.gameTime = OctopusGameLength;

        StartCoroutine(OctopusTimer());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RestartOctopus(){
        for (int i = 0; i < penguins.Length;i++){
            penguins[i].ReturnToLand();
            floes[i].OctopusActive = false;
        }
        StartCoroutine(OctopusTimer());
    }

    IEnumerator OctopusTimer(){
        
        yield return new WaitForSeconds(OctopusTimeout);
        for (int i = 0; i < penguins.Length;i++){
            penguins[i].Escape();
            floes[i].OctopusActive = true;
        }
        octopus.gameObject.SetActive(true);
        octopus.BeginGame();
    }

}
