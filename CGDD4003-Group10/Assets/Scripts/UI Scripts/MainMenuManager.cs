using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{

    public static bool isGamePaused;
    [Header("Menu Interaction Settings")]
    [SerializeField] AudioSource UIAudio;
    [SerializeField] AudioClip buttonClick;
    [SerializeField] AudioClip buttonHover;
    

    [Header("Menu Screens")]
    [SerializeField] GameObject menu;
    [SerializeField] GameObject options;
    [SerializeField] GameObject difficultyScreen;
    [SerializeField] GameObject howToPlayUI;

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
    [SerializeField] Slider miscVolume;

    [Header("GamePlay Settings")]
    [SerializeField] Slider MouseSensitivity;
    [SerializeField] Toggle viewBobbingToggle;

    [Header("HowTo Screens")]
    [SerializeField] GameObject movement;
    [SerializeField] GameObject shooting;
    [SerializeField] GameObject enemies;
    [SerializeField] GameObject endConditions;

    private void Awake()
    {
        AudioListener.volume = 1;

        if (SceneManager.GetActiveScene().name == "Main Menu" || SceneManager.GetActiveScene().name == "GameOverScene" || SceneManager.GetActiveScene().name == "ScoreScreen")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } else
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            isGamePaused = false;

            options?.SetActive(false);
            menu?.SetActive(false);
        }

        /*if (screenResolution != null)
        {
            screenResolution.onValueChanged.AddListener(delegate
            {
                SetResolution(screenResolution.value);
            });
        }*/

    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("FOV", fov.value);
        PlayerPrefs.SetFloat("Sensitivity", MouseSensitivity.value);
        PlayerPrefs.SetFloat("MastVolume", masterVolume.value / 4);
        PlayerPrefs.SetFloat("MusVolume", musicVolume.value / 4);
        PlayerPrefs.SetFloat("WVolume", weaponVolume.value / 4);
        PlayerPrefs.SetFloat("EVolume", enemyVolume.value / 4);
        PlayerPrefs.SetFloat("PlVolume", playerVolume.value / 4);
        PlayerPrefs.SetFloat("PiVolume", pickupVolume.value / 4);
        PlayerPrefs.SetFloat("MiscVolume", miscVolume.value / 4);
        PlayerPrefs.SetInt("Resolution", screenResolution.value);

        SetResolution(screenResolution.value);

        if(fullScreenToggle.isOn)
            PlayerPrefs.SetInt("Fullscreen", 1);
        else
            PlayerPrefs.SetInt("Fullscreen", 0);

        if (viewBobbingToggle.isOn)
            PlayerPrefs.SetInt("HeadBob", 1);
        else
            PlayerPrefs.SetInt("HeadBob", 0);

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
        UIAudio.PlayOneShot(buttonClick);
        menu.SetActive(false);
        options.SetActive(true);

        fov.value = PlayerPrefs.GetFloat("FOV");
        MouseSensitivity.value = PlayerPrefs.GetFloat("Sensitivity");
        masterVolume.value = PlayerPrefs.GetFloat("MastVolume", masterVolume.value / 4) * 4;
        musicVolume.value = PlayerPrefs.GetFloat("MusVolume", musicVolume.value / 4) * 4;
        weaponVolume.value = PlayerPrefs.GetFloat("WVolume", weaponVolume.value / 4) * 4;
        enemyVolume.value = PlayerPrefs.GetFloat("EVolume", enemyVolume.value / 4) * 4;
        playerVolume.value = PlayerPrefs.GetFloat("PlVolume", playerVolume.value / 4) * 4;
        pickupVolume.value = PlayerPrefs.GetFloat("PiVolume", pickupVolume.value / 4) * 4;
        miscVolume.value = PlayerPrefs.GetFloat("MiscVolume", miscVolume.value / 4) * 4;

        screenResolution.value = PlayerPrefs.GetInt("Resolution");

        if(PlayerPrefs.GetInt("Fullscreen") == 1)
            fullScreenToggle.isOn = true;
        else
            fullScreenToggle.isOn = false;

        if (PlayerPrefs.GetInt("HeadBob") == 1)
            viewBobbingToggle.isOn = true;
        else
            viewBobbingToggle.isOn = false;
    }
    public void DisplayScoreBoard() 
    {
        SceneManager.LoadScene(null);
    }
    public void ExitGame() 
    {
        UIAudio.PlayOneShot(buttonClick);
        //Exit for actual build
        Application.Quit();

        //Simulate Exit while testing in play mode
        #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        #endif
    }
    public void PauseGame()
    {
        UIAudio.PlayOneShot(buttonClick);
        Time.timeScale = 0.0f;

        menu.SetActive(true);

        isGamePaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void ResumeGame() 
    {
        UIAudio.PlayOneShot(buttonClick);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isGamePaused = false;

        options.SetActive(false);
        menu.SetActive(false);

        SaveSettings();
    }

    public void ReturnToMainMenu(int whichMenu)
    {
        UIAudio.PlayOneShot(buttonClick);
        switch (whichMenu)
        {
            case 1:
                SaveSettings();
                options.SetActive(false);
                menu.SetActive(true);
                if(difficultyScreen)
                    difficultyScreen?.SetActive(false);
                break;
            case 2:
                howToPlayUI.SetActive(false);
                menu.SetActive(true);
                if (difficultyScreen)
                    difficultyScreen?.SetActive(false);
                break;
        }
    }
    public void DisplayHowToPlayMenu() 
    {
        UIAudio.PlayOneShot(buttonClick);
        menu.SetActive(false);
        howToPlayUI.SetActive(true);
    }
    public void ToMovementPage() 
    {
        UIAudio.PlayOneShot(buttonClick);
        shooting.SetActive(false);
        movement.SetActive(true);
    }
    public void ToShootingPage()
    {
        UIAudio.PlayOneShot(buttonClick);
        movement.SetActive(false);
        shooting.SetActive(true);
        enemies.SetActive(false);
    }
    public void ToEnemiesPage()
    {
        UIAudio.PlayOneShot(buttonClick);
        shooting.SetActive(false);
        enemies.SetActive(true);
        endConditions.SetActive(false);
    }
    public void ToEndConditionsPage() 
    {
        UIAudio.PlayOneShot(buttonClick);
        enemies.SetActive(false);
        endConditions.SetActive(true);
    }
    public void DisplayDifficultyScreen()
    {
        UIAudio.PlayOneShot(buttonClick);
        menu.SetActive(false);
        howToPlayUI.SetActive(false);
        options.SetActive(false);
        difficultyScreen.SetActive(true);
    }

    public void SetResolution(int res)
    {
        
        switch (res)
        {
            case 0:
                Screen.SetResolution(3840, 2160, fullScreenToggle.isOn);
                print("1Resolution after at least a frame is " + Screen.width + "x" + Screen.height);
                break;
            case 1:
                Screen.SetResolution(2560, 1440, fullScreenToggle.isOn);
                print("2Resolution after at least a frame is " + Screen.width + "x" + Screen.height);
                break; ;
            case 2:
                Screen.SetResolution(1920, 1080, fullScreenToggle.isOn);
                print("3Resolution after at least a frame is " + Screen.width + "x" + Screen.height);
                break;
            case 3:
                Screen.SetResolution(1600, 900, fullScreenToggle.isOn);
                print("4Resolution after at least a frame is " + Screen.width + "x" + Screen.height);
                break;
            case 4:
                Screen.SetResolution(1280, 720, fullScreenToggle.isOn);
                print("5Resolution after at least a frame is " + Screen.width + "x" + Screen.height);
                break;

        }
    }

    public void SetDifficulty(int value)
    {
        UIAudio.PlayOneShot(buttonClick);
        Score.SetDifficulty(value);
    }
}
