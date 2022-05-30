using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveBtnScript : MonoBehaviour
{
    public string newGameScene;
    public string saveGameMenu;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewGame()
    {
        SceneManager.LoadScene(newGameScene);
    }

    public void SaveMenu()
    {
        SceneManager.LoadScene(saveGameMenu);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
