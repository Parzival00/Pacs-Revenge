using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class MainMenuManager : MonoBehaviour
{
    private string playerInput;
    private StreamReader scoresForScoreBoard;

    [Header("Menu Screens")]
    [SerializeField] GameObject menu;
    [SerializeField] GameObject options;
    [SerializeField] TMP_InputField uiInput;
    [SerializeField] TMP_Text highScoreDisplay;

    [Header("Resolution Settings")]
    [SerializeField] GameObject screenResolution;
    [SerializeField] GameObject fOV;

    [Header("Audio Settings")]
    [SerializeField] GameObject audioPlaceHolder;

    [Header("GamePlay Settings")]
    [SerializeField] GameObject MouseSensitivity;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadGameScene(int sceneIndex) 
    {
        SceneManager.LoadScene(sceneIndex);
    }
    public void DisplayOptions() 
    {
        menu.SetActive(false);
        options.SetActive(true);

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
    public void ResumeGame() 
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ReturnToMainMenu() 
    {
        options.SetActive(false);
        menu.SetActive(true);
    }
    /// <summary>
    /// Using PlayerPrefs || Saves current selection of settings
    /// </summary>
    public void SaveSettings() 
    {
        
    }
}
