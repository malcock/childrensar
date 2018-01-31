using UnityEngine;
using System.Collections;

public class CatchTrigger : MonoBehaviour
{
    MainCharacter parent;
	// Use this for initialization
	void Start()
	{
        parent = GetComponentInParent<MainCharacter>();
	}

	// Update is called once per frame
	void Update()
	{
			
	}

    private void OnTriggerEnter(Collider other)
    {
        
        if(other.gameObject.GetComponentInParent<Edible>()){
            Debug.Log("edible");
            parent.Catch();
            Destroy(other.gameObject);
        }

    }
}
