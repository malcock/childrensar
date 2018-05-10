using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuController : MonoBehaviour
{
	// Use this for initialization
	void Start()
	{

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
}
