using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusController : MonoBehaviour {

    Animator animator;

    public enum State {Start, Begin,Active,End}
    public State state = State.Start;
    PolarManager manager;

    float time = 0;

    public float gameTime = 240;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Use this for initialization
    void Start () {
        manager = FindObjectOfType<PolarManager>();
	}
	
    public void BeginGame(){
        StartCoroutine(PlayGame());
    }

    IEnumerator PlayGame(){
        yield return new WaitForSeconds(1);
        state = State.Begin;
        animator.SetTrigger("Begin");
        yield return new WaitForSeconds(2);
        state = State.Active;
        yield return new WaitForSeconds(gameTime);
        state = State.End;
        manager.RestartOctopus();
    }
    // Update is called once per frame
	void Update () {
        switch(state){
            case State.Start:
                break;
            case State.Begin:
                
                break;
            case State.Active:
                animator.SetBool("Active", true);
                for (int l = 1; l < animator.layerCount; l++)
                {
                    animator.SetLayerWeight(l, Mathf.Abs(Mathf.Sin(time + (l * 20))));
                    time += Time.deltaTime * 0.25f;
                }
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
