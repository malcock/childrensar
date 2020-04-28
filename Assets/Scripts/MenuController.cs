using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using UnityEngine.UI;
public class MenuController : MonoBehaviour
{
    public Slider gameATime, gameBTime, fishPerCharacter,feedNumber;
    public GameObject passwordPanel;
    public InputField passwordField;
    public Text feedback;
	// Use this for initialization
	void Start()
	{
        if(PlayerPrefs.HasKey("PasswordConfirmed")){
            passwordPanel.SetActive(false);
        } else {
            passwordPanel.SetActive(true);
        }
        gameATime.value = (PlayerPrefs.HasKey("GameATime")) ? (int) PlayerPrefs.GetInt("GameATime") : 3;
        SetGameATime((int) gameATime.value);
        gameBTime.value = (PlayerPrefs.HasKey("GameBTime")) ? (int)PlayerPrefs.GetInt("GameBTime") : 3;
        SetGameBTime((int) gameBTime.value);
        fishPerCharacter.value = (PlayerPrefs.HasKey("FishPerCharacter")) ? (int) PlayerPrefs.GetInt("FishPerCharacter") : 2;
        SetFishPerCharacter((int) fishPerCharacter.value);
        feedNumber.value = (PlayerPrefs.HasKey("FeedNumber")) ? (int)PlayerPrefs.GetInt("FeedNumber") : 2;
        SetFeedNumber((int) feedNumber.value);


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
        GameControl.Instance.gametimeA = v*60;
        PlayerPrefs.SetInt("GameATime", v);


    }

    public void CheckPassword(){
        if (passwordField.text == "Artf3lt*")
        {
            PlayerPrefs.SetString("PasswordConfirmed", "Artf3lt*");
            passwordPanel.SetActive(false);
        } else {
            feedback.text = "Sorry, wrong password";
        }
    }



    public void SetGameBTime(float val)
    {
        int v = (int)val;
        GameControl.Instance.gametimeB = v*60;
        PlayerPrefs.SetInt("GameBTime", v);
    }

    public void SetFishPerCharacter(float val)
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
        PlayerPrefs.SetInt("FishPerCharacter", v);

    }

    public void SetFeedNumber(float val)
    {
        int v = (int)val;
        switch (v)
        {
            case 1:
                GameControl.Instance.feedNumber = 2;
                break;
            case 2:
                GameControl.Instance.feedNumber = 3;
                break;
            case 3:
                GameControl.Instance.feedNumber = 5;
                break;
        }
        PlayerPrefs.SetInt("FeedNumber", v);

    }
}
