using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using UnityEngine.UI;
public class MenuController : MonoBehaviour
{
    public Slider gameATime, gameBTime, difficulty;
	// Use this for initialization
	void Start()
	{

        gameATime.value = (PlayerPrefs.HasKey("GameATime")) ? (int) PlayerPrefs.GetInt("GameATime") : 240;
        SetGameATime((int) gameATime.value);
        gameBTime.value = (PlayerPrefs.HasKey("GameBTime")) ? (int)PlayerPrefs.GetInt("GameBTime") : 240;
        SetGameBTime((int) gameBTime.value);
        difficulty.value = (PlayerPrefs.HasKey("Difficulty")) ? (int) PlayerPrefs.GetInt("Difficulty") : 2;
        SetDifficulty((int) difficulty.value);

	}

	// Update is called once per frame
	void Update()
	{
        //if (GameControl.Instance.CharacterBehaviour == GameControl.CharacterMode.Leave)
	}

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void SetGameATime(float val)
    {
        int v = (int)val;
        GameControl.Instance.gametimeA = v;
        PlayerPrefs.SetInt("GameATime", v);


    }

    public void SetGameBTime(float val)
    {
        int v = (int)val;
        GameControl.Instance.gametimeB = v;
        PlayerPrefs.SetInt("GameBTime", v);
    }

    public void SetDifficulty(float val)
    {
        int v = (int)val;
        switch(v){
            case 1:
                GameControl.Instance.fishMax = 1;
                break;
            case 2:
                GameControl.Instance.fishMax = 3;
                break;
            case 3:
                GameControl.Instance.fishMax = 5;
                break;
        }
        PlayerPrefs.SetInt("Difficulty", v);

    }
}
