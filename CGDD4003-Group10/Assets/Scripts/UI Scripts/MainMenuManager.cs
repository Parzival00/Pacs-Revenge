using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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
    [SerializeField] Toggle tutorialToggle;

    [Header("Audio Settings")]
    [SerializeField] Slider masterVolume;
    [SerializeField] Slider musicVolume;
    [SerializeField] Slider weaponVolume;
    [SerializeField] Slider enemyVolume;
    [SerializeField] Slider playerVolume;
    [SerializeField] Slider pickupVolume;
    [SerializeField] Slider uiVolume;
    [SerializeField] Slider miscVolume;
    [Header("Audio Tests")]
    [SerializeField] AudioSource masterSource;
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource weaponSource;
    [SerializeField] AudioSource enemySource;
    [SerializeField] AudioSource playerSource;
    [SerializeField] AudioSource pickupSource;
    [SerializeField] AudioSource uiSource;
    [SerializeField] AudioSource miscSource;

    [Header("GamePlay Settings")]
    [SerializeField] Slider MouseSensitivity;
    [SerializeField] Toggle viewBobbingToggle;

    [Header("HowTo Screens")]
    [SerializeField] GameObject movement;
    [SerializeField] GameObject shooting;
    [SerializeField] GameObject enemies;
    [SerializeField] GameObject endConditions;
    [Header("End Statistics Menus")]
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] GameObject endStatsScreen;
    [SerializeField] TMP_Text playTime;
    [SerializeField] TMP_Text railgunNumFired;
    [SerializeField] TMP_Text stungunNumFired;
    [SerializeField] TMP_Text ghostKilled;
    [SerializeField] TMP_Text shieldUsed;
    [SerializeField] TMP_Text livesUsed;
    [SerializeField] TMP_Text totalPellets;


    GlobalAudioController audioController;
    PlayerController player;

    private void Awake()
    {
        AudioListener.volume = 1;

        player = GameObject.FindObjectOfType<PlayerController>();
        audioController = GameObject.FindObjectOfType<GlobalAudioController>();

        if (SceneManager.GetActiveScene().name == "Main Menu" || SceneManager.GetActiveScene().name == "GameOverScene" || 
            SceneManager.GetActiveScene().name == "ScoreScreen" || SceneManager.GetActiveScene().name == "Credits" || SceneManager.GetActiveScene().name == "End")
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } else
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            isGamePaused = false;

            if(options != null)
                options?.SetActive(false);
            if(menu != null)
                menu?.SetActive(false);
        }

        /*if (screenResolution != null)
        {
            screenResolution.onValueChanged.AddListener(delegate
            {
                SetResolution(screenResolution.value);
            });
        }*/
        if (gameOverScreen != null && endStatsScreen != null) 
        {
            LoadEndStatistics();
        }
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
        PlayerPrefs.SetFloat("UIVolume", uiVolume.value / 4);
        PlayerPrefs.SetFloat("MiscVolume", miscVolume.value / 4);
        PlayerPrefs.SetInt("Resolution", screenResolution.value);
        PlayerPrefs.SetInt("TutorialPrompts", tutorialToggle.isOn ? 1 : 0);

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

        if(player!=null)
            player.ApplyGameSettings();
        if (audioController != null)
            audioController.ApplyAudioSettings();
    }

    public void LoadGameScene(int sceneIndex) 
    {
        //UIAudio.PlayOneShot(buttonClick);
        StartCoroutine(LoadScene(sceneIndex));
        //SceneManager.LoadScene(sceneIndex);

    }
    public void LoadGameScene(string sceneName)
    {
        //UIAudio.PlayOneShot(buttonClick);
        StartCoroutine(LoadScene(sceneName));
        //SceneManager.LoadScene(sceneName);
    }

    IEnumerator LoadScene(int sceneIndex)
    {
        UIAudio.PlayOneShot(buttonClick);
        yield return new WaitForSecondsRealtime(0.3f);
        SceneManager.LoadScene(sceneIndex);
    }
    IEnumerator LoadScene(string sceneName)
    {
        UIAudio.PlayOneShot(buttonClick);
        yield return new WaitForSecondsRealtime(0.3f);
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
        uiVolume.value = PlayerPrefs.GetFloat("UIVolume", uiVolume.value / 4) * 4;
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

        if (!PlayerPrefs.HasKey("TutorialPrompts") || PlayerPrefs.GetInt("TutorialPrompts") == 1)
            tutorialToggle.isOn = true;
        else
            tutorialToggle.isOn = false;
    }
    public void DisplayScoreBoard() 
    {
        UIAudio.PlayOneShot(buttonClick);
    }
    public void ExitGame() 
    {
        AchievementManager.save();
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

        if (options.activeSelf)
            SaveSettings();

        options.SetActive(false);
        menu.SetActive(false);
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
    public void DisplayEndStatistics()
    {
        UIAudio.PlayOneShot(buttonClick);
        gameOverScreen.SetActive(false);
        endStatsScreen.SetActive(true);
    }
    public void LoadEndStatistics()
    {
        int hours = Mathf.RoundToInt(Score.totalTimePlayed / 60 / 60);
        int minutes = Mathf.RoundToInt(Score.totalTimePlayed / 60 % 60);
        int seconds = Mathf.RoundToInt(Score.totalTimePlayed % 60);
        string hoursString = hours < 10 ? "0" + hours : hours.ToString();
        string minutesString = minutes < 10 ? "0" + minutes : minutes.ToString();
        string secondsString = seconds < 10 ? "0" + seconds : seconds.ToString();

        if (hours > 0)
        {
            playTime.text = hoursString + ":" + minutesString + ":" + secondsString;
        } else
        {
            playTime.text = minutesString + ":" + secondsString;
        }

        railgunNumFired.text = "" + Score.totalShotsFired;
        stungunNumFired.text = "" + Score.totalStunsFired;
        ghostKilled.text = "" + Score.totalGhostKilled;
        shieldUsed.text = "" + Score.totalShieldsRecieved;
        livesUsed.text = "" + Score.totalLivesConsumed;
        totalPellets.text = "" + Score.totalPelletsCollected;
    } 
    public void ReturnToGameOverScreen() 
    {
        UIAudio.PlayOneShot(buttonClick);
        endStatsScreen.SetActive(false);
        gameOverScreen.SetActive(true);
    }

    public void PlayClickAudio()
    {
        UIAudio.PlayOneShot(buttonClick);
    }

    #region Audio Volume Testing Functions
    public void PlayMasterSoundTest()
    {
        masterSource.PlayOneShot(masterSource.clip);
        PlayerPrefs.SetFloat("MastVolume", masterVolume.value / 4);

        audioController.ApplyAudioSettings();
    }
    public void PlayMusicSoundTest()
    {
        musicSource.PlayOneShot(musicSource.clip);
        PlayerPrefs.SetFloat("MusVolume", musicVolume.value / 4);

        audioController.ApplyAudioSettings();
    }
    public void PlayWeaponSoundTest()
    {
        weaponSource.PlayOneShot(weaponSource.clip);
        PlayerPrefs.SetFloat("WVolume", weaponVolume.value / 4);

        audioController.ApplyAudioSettings();
    }
    public void PlayEnemySoundTest()
    {
        enemySource.PlayOneShot(enemySource.clip);
        PlayerPrefs.SetFloat("EVolume", enemyVolume.value / 4);

        audioController.ApplyAudioSettings();
    }
    public void PlayPlayerSoundTest()
    {
        playerSource.PlayOneShot(playerSource.clip);
        PlayerPrefs.SetFloat("PlVolume", playerVolume.value / 4);

        audioController.ApplyAudioSettings();
    }
    public void PlayPickUpSoundTest()
    {
        pickupSource.PlayOneShot(pickupSource.clip);
        PlayerPrefs.SetFloat("PiVolume", pickupVolume.value / 4);

        audioController.ApplyAudioSettings();
    }
    public void PlayUISoundTest()
    {
        uiSource.PlayOneShot(uiSource.clip);
        PlayerPrefs.SetFloat("UIVolume", uiVolume.value / 4);

        audioController.ApplyAudioSettings();
    }
    public void PlayMiscSoundTest()
    {
        miscSource.PlayOneShot(miscSource.clip);
        PlayerPrefs.SetFloat("MiscVolume", miscVolume.value / 4);

        audioController.ApplyAudioSettings();
    }
    #endregion
}
