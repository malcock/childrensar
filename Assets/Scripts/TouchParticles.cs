using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.iOS;

public class TouchParticles : MonoBehaviour
{

    ParticleSystem ps, ps2;
    public Transform m_HitTransform;
    public int touchNum = 0;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        if(transform.childCount>0)
            ps2 = transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    bool HitTestWithResultType(ARPoint point, ARHitTestResultType resultTypes)
    {

        List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point, resultTypes);
        if (hitResults.Count > 0)
        {
            foreach (var hitResult in hitResults)
            {
                Debug.Log("Got hit!");

                m_HitTransform.position = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
                m_HitTransform.rotation = UnityARMatrixOps.GetRotation(hitResult.worldTransform);
                Debug.Log(string.Format("x:{0:0.######} y:{1:0.######} z:{2:0.######}", m_HitTransform.position.x, m_HitTransform.position.y, m_HitTransform.position.z));
                return true;
            }
        }
        return false;




    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > touchNum && m_HitTransform != null)
        {
            if (!ps.isPlaying)
            {
                ps.Play();
                if(ps2!=null) ps2.Play();
            }
            var touch = Input.GetTouch(touchNum);
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                //lets check if we hit a virtual object first tho
                RaycastHit hit;
                Ray touchPosition = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(touchPosition, out hit, 100, LayerMask.GetMask("World", "Actor")))
                {
                    m_HitTransform.position = hit.point;


                } else {
                    var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
                    ARPoint point = new ARPoint
                    {
                        x = screenPosition.x,
                        y = screenPosition.y
                    };



                    // prioritize reults types
                    ARHitTestResultType[] resultTypes = {
                        //ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
                        //// if you want to use infinite planes use this:
                        ////ARHitTestResultType.ARHitTestResultTypeExistingPlane,
                        //ARHitTestResultType.ARHitTestResultTypeHorizontalPlane,
                        ARHitTestResultType.ARHitTestResultTypeFeaturePoint
                    };

                    foreach (ARHitTestResultType resultType in resultTypes)
                    {
                        if (HitTestWithResultType(point, resultType))
                        {
                            return;
                        }
                    }
                }



            }
        }
        else
        {
            ps.Stop();
            if(ps2!=null)ps2.Stop();
        }
    }

}
