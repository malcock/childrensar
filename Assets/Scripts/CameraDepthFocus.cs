using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;


[RequireComponent(typeof(PostProcessingBehaviour))]
public class CameraDepthFocus : MonoBehaviour {

    PostProcessingProfile m_Profile;
    float m_TargetValue = 10;

    void OnEnable()
    {
        var behaviour = GetComponent<PostProcessingBehaviour>();

        if (behaviour.profile == null)
        {
            enabled = false;
            return;
        }

        m_Profile = Instantiate(behaviour.profile);
        behaviour.profile = m_Profile;
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//on update cast a ray from center of screen and find whatever it intersects with
        RaycastHit hit;
        var cameraCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, Camera.main.nearClipPlane));
        if (Physics.Raycast(cameraCenter, this.transform.forward, out hit, 1000))
        {
            m_TargetValue = Vector3.Distance(cameraCenter, hit.transform.position);
        }
        var dof = m_Profile.depthOfField.settings;
        dof.focusDistance = Mathf.Lerp(dof.focusDistance, m_TargetValue, Time.deltaTime*3);
        m_Profile.depthOfField.settings = dof;

	}
}
