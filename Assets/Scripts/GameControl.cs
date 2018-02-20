using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{

    public enum CharacterMode { Stay, Leave }

    public CharacterMode CharacterBehaviour = CharacterMode.Stay;

    public enum FlickMode { Final, Drag, Tap }

    public FlickMode FlickBehaviour = FlickMode.Final;

    public enum OctoMode { Final, Crazy, Boring }

    public OctoMode OctoBehaviour = OctoMode.Final;

    public float gametimeA = 240, gametimeB = 240;

    public static GameControl Instance = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void SetCharacterBehaviour(int val)
    {
        Instance.CharacterBehaviour = (GameControl.CharacterMode)val;
    }

    public void SetFlickBehaviour(int val)
    {
        Instance.FlickBehaviour = (GameControl.FlickMode)val;
    }

    public void SetOctoBehaviour(int val)
    {
        Instance.OctoBehaviour = (GameControl.OctoMode)val;
    }

    public void SetGameATime(string val)
    {
        if (val.Length > 0)
        {
            try
            {
                float v = float.Parse(val);
                v = Mathf.Max(v, 30);
                gametimeA = v;
            }
            catch (Exception e)
            {

            }

        }

    }

    public void SetGameBTime(string val)
    {
        if (val.Length > 0)
        {
            try
            {
                float v = float.Parse(val);
                v = Mathf.Max(v, 30);
                gametimeB = v;
            }
            catch (Exception e)
            {

            }

        }
    }
}
