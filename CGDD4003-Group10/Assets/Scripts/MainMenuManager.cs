using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] TMP_Dropdown screenResolution;
    [SerializeField] Slider fov;
    [SerializeField] Toggle fullScreenToggle;

    [Header("Audio Settings")]
    [SerializeField] Slider volume;

    [Header("GamePlay Settings")]
    [SerializeField] Slider MouseSensitivity;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("FOV", fov.value);
        PlayerPrefs.SetFloat("Sensitivity", MouseSensitivity.value);
        PlayerPrefs.SetFloat("Volume", volume.value);


        PlayerPrefs.Save();

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

        options.SetActive(false);
        menu.SetActive(false);
    }

    public void ReturnToMainMenu() 
    {
        options.SetActive(false);
        menu.SetActive(true);
    }

    public void SetResolution()
    {
        switch (screenResolution.value)
        {
            case 0:
                Screen.SetResolution(3840, 2160, fullScreenToggle);
                break;
            case 1:
                Screen.SetResolution(2560, 1440, fullScreenToggle);
                break; ;
            case 2:
                Screen.SetResolution(1920, 1080, fullScreenToggle);
                break;
            case 3:
                Screen.SetResolution(1600, 900, fullScreenToggle);
                break;
            case 4:
                Screen.SetResolution(1280, 720, fullScreenToggle);
                break;

        }
    }
}
