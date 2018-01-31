using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusController : MonoBehaviour {

    Animator animator;

    public enum State {Start, Begin,Active,End}
    public State state = State.Start;
    public float wobbleAmount = 20;


    float time = 0;

    public float gameTime = 240;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        foreach (BoxCollider box in GetComponentsInChildren<BoxCollider>())
        {
            //box.gameObject.AddComponent<OctoBone>();
            //box.isTrigger = true;
        }
    }

    // Use this for initialization
    void Start () {
        
	}
	
    public void BeginGame(){
        StartCoroutine(PlayGame());
    }

    IEnumerator PlayGame(){
        yield return new WaitForSeconds(1);
        state = State.Begin;
        animator.SetTrigger("Begin");
        AkSoundEngine.SetSwitch("OctopusAppearDisappear", "Appear", gameObject);
        AkSoundEngine.PostEvent("OctopusAppearDisappear", gameObject);

        yield return new WaitForSeconds(2);
        AkSoundEngine.PostEvent("OctopusVocal",gameObject);
        state = State.Active;
        yield return new WaitForSeconds(gameTime);
        state = State.End;
        AkSoundEngine.SetSwitch("OctopusAppearDisappear", "Disappear", gameObject);
        AkSoundEngine.PostEvent("OctopusAppearDisappear", gameObject);
        AkSoundEngine.PostEvent("OctopusVocal", gameObject);
    }

    public void StopGame(){
        StopAllCoroutines();
        state = State.End;
    }

    // Update is called once per frame
	void Update () {
        switch(state){
            case State.Start:
                
                break;
            case State.Begin:
                foreach (Ring r in GetComponentsInChildren<Ring>())
                {
                    Destroy(r.gameObject);
                }
                break;
            case State.Active:
                animator.SetBool("Active", true);
                for (int l = 1; l < animator.layerCount; l++)
                {
                    animator.SetLayerWeight(l, Mathf.Abs(Mathf.Sin(time + (l * 20))));
                    time += Time.deltaTime * 0.25f;
                }
                //rotate the octopus slightly
                Vector3 rotation = transform.eulerAngles;
                float newY = 180 + Mathf.Sin(Time.time) * wobbleAmount;
                rotation.y = Mathf.Lerp(rotation.y, newY, Time.deltaTime);
                transform.eulerAngles = rotation;
                break;
            case State.End:
                int layersReady = animator.layerCount - 1;
                time = 0;
                for (int l = 1; l < animator.layerCount; l++)
                {
                    if (animator.GetLayerWeight(l) > 0.05f)
                    {
                        animator.SetLayerWeight(l, Mathf.Lerp(animator.GetLayerWeight(l), 0, Time.deltaTime));
                    }
                    else
                    {
                        animator.SetLayerWeight(l, 0);
                        layersReady--;
                    }

                }
                if (layersReady == 0){
                    animator.SetBool("Active", false);
                    animator.SetTrigger("Done");
                    state = State.Start;
                } 
                break;

        }



	}
}
