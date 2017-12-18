using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhalePath : MonoBehaviour {

    public float timeout = 10;

	// Use this for initialization
	void Start () {
        Reset();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RandomRotation(){
        GetComponent<Animator>().SetBool("Swimming", true);
        transform.parent.rotation = Quaternion.Euler(0, Random.value * 360, 0);
    }

    public void Reset(){
        GetComponent<Animator>().SetBool("Swimming",false);
        StartCoroutine(DoTimeout());
    }

    IEnumerator DoTimeout(){
        yield return new WaitForSeconds(timeout);
        Debug.Log("do rotation");
        RandomRotation();

    }
}
