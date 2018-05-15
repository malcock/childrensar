using UnityEngine;
using System.Collections;

public class BearCatchTrigger : MonoBehaviour
{
    BearController parent;
	// Use this for initialization
	void Start()
	{
        parent = GetComponentInParent<BearController>();

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
