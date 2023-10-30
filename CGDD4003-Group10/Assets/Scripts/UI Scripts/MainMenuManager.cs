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
    [SerializeField] GameObject howToPlayUI;
    [SerializeField] TMP_InputField uiInput;
    [SerializeField] TMP_Text highScoreDisplay;

    [Header("Resolution Settings")]
    [SerializeField] TMP_Dropdown screenResolution;
    [SerializeField] Slider fov;
    [SerializeField] Toggle fullScreenToggle;

    [Header("Audio Settings")]
    [SerializeField] Slider masterVolume;
    [SerializeField] Slider musicVolume;
    [SerializeField] Slider weaponVolume;
    [SerializeField] Slider enemyVolume;
    [SerializeField] Slider playerVolume;
    [SerializeField] Slider pickupVolume;

    [Header("GamePlay Settings")]
    [SerializeField] Slider MouseSensitivity;

    [Header("HowTo Screens")]
    [SerializeField] GameObject movement;
    [SerializeField] GameObject shooting;
    [SerializeField] GameObject enemies;
    [SerializeField] GameObject endConditions;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("FOV", fov.value);
        PlayerPrefs.SetFloat("Sensitivity", MouseSensitivity.value);
        PlayerPrefs.SetFloat("MastVolume", masterVolume.value);
        PlayerPrefs.SetFloat("MusVolume", musicVolume.value);
        PlayerPrefs.SetFloat("WVolume", weaponVolume.value);
        PlayerPrefs.SetFloat("EVolume", enemyVolume.value);
        PlayerPrefs.SetFloat("PlVolume", playerVolume.value);
        PlayerPrefs.SetFloat("PiVolume", pickupVolume.value);

        PlayerPrefs.Save();

        PlayerController player = GameObject.FindObjectOfType<PlayerController>();
        GlobalAudioController audio = GameObject.FindObjectOfType<GlobalAudioController>();
        if(player!=null)
            player.ApplyGameSettings();
        if (audio != null)
            audio.ApplyAudioSettings();
    }

    public void LoadGameScene(int sceneIndex) 
    {
        SceneManager.LoadScene(sceneIndex);
    }
    public void LoadGameScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void DisplayOptions() 
    {
        menu.SetActive(false);
        options.SetActive(true);

        fov.value = PlayerPrefs.GetFloat("FOV");
        MouseSensitivity.value = PlayerPrefs.GetFloat("Sensitivity");
        masterVolume.value = PlayerPrefs.GetFloat("MastVolume", masterVolume.value);
        musicVolume.value = PlayerPrefs.GetFloat("MusVolume", musicVolume.value);
        weaponVolume.value = PlayerPrefs.GetFloat("WVolume", weaponVolume.value);
        enemyVolume.value = PlayerPrefs.GetFloat("EVolume", enemyVolume.value);
        playerVolume.value = PlayerPrefs.GetFloat("PlVolume", playerVolume.value);
        pickupVolume.value = PlayerPrefs.GetFloat("PiVolume", pickupVolume.value);
        fullScreenToggle.isOn = Screen.fullScreen;
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

    public void ReturnToMainMenu(int whichMenu)
    {
        switch (whichMenu)
        {
            case 1:
                SaveSettings();
                options.SetActive(false);
                menu.SetActive(true);
                break;
            case 2:
                howToPlayUI.SetActive(false);
                menu.SetActive(true);
                break;
        }
    }
    public void DisplayHowToPlayMenu() 
    {
        menu.SetActive(false);
        howToPlayUI.SetActive(true);
    }
    public void ToMovementPage() 
    {
        shooting.SetActive(false);
        movement.SetActive(true);
    }
    public void ToShootingPage()
    {
        movement.SetActive(false);
        shooting.SetActive(true);
        enemies.SetActive(false);
    }
    public void ToEnemiesPage()
    {
        shooting.SetActive(false);
        enemies.SetActive(true);
        endConditions.SetActive(false);
    }
    public void ToEndConditionsPage() 
    {
        enemies.SetActive(false);
        endConditions.SetActive(true);
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
