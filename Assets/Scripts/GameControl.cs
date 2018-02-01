using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour {

    public enum CharacterMode {Stay,Leave}

    public CharacterMode CharacterBehaviour = CharacterMode.Stay;

    public enum FlickMode {Drag,Tap}

    public FlickMode FlickBehaviour = FlickMode.Drag;

    public enum OctoMode {Crazy,Boring}

    public OctoMode OctoBehaviour = OctoMode.Crazy;

    public float gametimeA = 240, gametimeB = 240;

    public static GameControl Instance = null;

    private void Awake()
    {
        if(Instance==null){
            Instance = this;
        } else if(Instance!=null){
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    // Update is called once per frame
    void Update () {
		
	}

    public void LoadScene(string name){
        SceneManager.LoadScene(name);
    }

    public void SetCharacterBehaviour(int val){
        Instance.CharacterBehaviour = (GameControl.CharacterMode)val;
    }

    public void SetFlickBehaviour(int val){
        Instance.FlickBehaviour = (GameControl.FlickMode)val;
    }

    public void SetOctoBehaviour(int val){
        Instance.OctoBehaviour = (GameControl.OctoMode)val;
    }

    public void SetGameATime(string val)
    {
        gametimeA = float.Parse(val);
    }

    public void SetGameBTime(string val){
        gametimeB = float.Parse(val);
    }
}
