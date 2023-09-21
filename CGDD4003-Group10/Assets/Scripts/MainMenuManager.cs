using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame() 
    {
        SceneManager.LoadScene("Game");
    }
    public void DisplayOptions() 
    {
        
    }
    public void DisplayScoreBoard() 
    {
        SceneManager.LoadScene(null);
    }
    public void ExitGame() 
    {
        //Exit for actual build
        Application.Quit();

        //Simulate Exit while testing in play mode
        #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        #endif
    }
}
