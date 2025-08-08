using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;
using System;

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
    [SerializeField] GameObject achievementsScreen;
    [SerializeField] GameObject introSkipScreen;
    [SerializeField] AchievementDisplay achievementDisplay;

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

    [Header("WeaponSelect Screen")]
    [SerializeField] WeaponInfo[] weaponInfos;
    [SerializeField] Image weaponImage;
    [SerializeField] TMP_Text weaponName;
    [SerializeField] TMP_Text weaponDescription;
    [SerializeField] Image damageSlider;
    [SerializeField] Image speedSlider;
    [SerializeField] Image rangeSlider;
    [SerializeField] GameObject weaponSelectScreen;
    int weaponSelection;

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

    [Header("Starting UI Buttons")]
    [SerializeField] GameObject mainMenuStart;
    [SerializeField] TMP_Text mainMenuContinue;
    [SerializeField] Button continueButton;
    [SerializeField] GameObject weaponSelectStart;
    [SerializeField] GameObject difficultySelectStart;
    [SerializeField] GameObject optionsStart;
    [SerializeField] GameObject skipIntroStart;
    [SerializeField] GameObject pauseGameStart;

    public static Action OnOptionsChanged;
    public static Action OnPause;
    public static Action OnResume;


    GlobalAudioController audioController;
    PlayerController player;

    private void Awake()
    {
        AudioListener.volume = 1;

        player = GameObject.FindObjectOfType<PlayerController>();
        audioController = GameObject.FindObjectOfType<GlobalAudioController>();
        SaveData.Load();

        if (SceneManager.GetActiveScene().name == "Main Menu" || SceneManager.GetActiveScene().name == "GameOverScene" || 
            SceneManager.GetActiveScene().name == "ScoreScreen" || SceneManager.GetActiveScene().name == "Credits" || 
            SceneManager.GetActiveScene().name == "End" || SceneManager.GetActiveScene().name == "Start" || SceneManager.GetActiveScene().name == "DemoEnd")
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } 
        else
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            isGamePaused = false;

            if (options != null)
            {
                options.SetActive(false);
            }
            if (menu != null)
            {
                menu.SetActive(false);
            }
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

        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            if (SaveData.getSaveExists() && SaveData.getLevel() > 1)//enables the continue game button and its visuals if there is a save file present
            {
                continueButton.enabled = true;
                mainMenuContinue.color = new Color(255,255,255,255);
            }

            weaponSelection = PlayerPrefs.GetInt("Weapon", 0);
            PlayerPrefs.SetInt("Weapon", weaponSelection);
            NavigateWeaponSelection(0);
        }
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("FOV", fov.value);
        PlayerPrefs.SetFloat("Sensitivity", MouseSensitivity.value);
        PlayerPrefs.SetFloat("MastVolume", masterVolume.value >= 0 ? masterVolume.value / 4 : masterVolume.value);
        PlayerPrefs.SetFloat("MusVolume", musicVolume.value >= 0 ? musicVolume.value / 4 : musicVolume.value);
        PlayerPrefs.SetFloat("WVolume", weaponVolume.value >= 0 ? weaponVolume.value / 4 : weaponVolume.value);
        PlayerPrefs.SetFloat("EVolume", enemyVolume.value >= 0 ? enemyVolume.value / 4 : enemyVolume.value);
        PlayerPrefs.SetFloat("PlVolume", playerVolume.value >= 0 ? playerVolume.value / 4 : playerVolume.value);
        PlayerPrefs.SetFloat("PiVolume", pickupVolume.value >= 0 ? pickupVolume.value / 4: pickupVolume.value);
        PlayerPrefs.SetFloat("UIVolume", uiVolume.value >= 0 ? uiVolume.value / 4 : uiVolume.value);
        PlayerPrefs.SetFloat("MiscVolume", miscVolume.value >= 0 ? miscVolume.value / 4 : miscVolume.value);
        PlayerPrefs.SetInt("Resolution", screenResolution.value);
        PlayerPrefs.SetInt("TutorialPrompts", tutorialToggle.isOn ? 1 : 0);

        SetResolution(screenResolution.value);

        if (fullScreenToggle.isOn)
        {
            PlayerPrefs.SetInt("Fullscreen", 1);
        }
        else
        {
            PlayerPrefs.SetInt("Fullscreen", 0);
        }

        if (viewBobbingToggle.isOn)
        {
            PlayerPrefs.SetInt("HeadBob", 1);
        }
        else
        {
            PlayerPrefs.SetInt("HeadBob", 0);
        }

        PlayerPrefs.Save();

        if (player != null)
        {
            player.ApplyGameSettings();
        }
        if (audioController != null)
        {
            audioController.ApplyAudioSettings();
        }

        if(OnOptionsChanged != null)
        {
            OnOptionsChanged.Invoke();
        }
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

    public void ContinueGame()
    {
        SaveData.Load();

        Score.SetDifficulty(SaveData.getDifficulty());
        weaponSelection = SaveData.getCurrentWeapon();
        print("Setting difficulty to " + SaveData.getDifficulty() + " Setting weapon to " + weaponSelection);

        SceneManager.LoadScene(SaveData.getLevel());
    }

    public void DisplayOptions() 
    {
        UIAudio.PlayOneShot(buttonClick);
        menu.SetActive(false);
        options.SetActive(true);

        //Clear Selected
        EventSystem.current.SetSelectedGameObject(null);

        //Select default starting button
        EventSystem.current.SetSelectedGameObject(optionsStart);

        fov.value = PlayerPrefs.GetFloat("FOV", 70);
        MouseSensitivity.value = PlayerPrefs.GetFloat("Sensitivity", 100);
        masterVolume.value = PlayerPrefs.GetFloat("MastVolume", masterVolume.value >= 0 ? masterVolume.value / 4 : masterVolume.value);
        masterVolume.value = masterVolume.value >= 0 ? masterVolume.value * 4 : masterVolume.value;
        musicVolume.value = PlayerPrefs.GetFloat("MusVolume", musicVolume.value >= 0 ? musicVolume.value / 4 : musicVolume.value);
        musicVolume.value = musicVolume.value >= 0 ? musicVolume.value * 4 : musicVolume.value;
        weaponVolume.value = PlayerPrefs.GetFloat("WVolume", weaponVolume.value >= 0 ? weaponVolume.value / 4 : weaponVolume.value);
        weaponVolume.value = weaponVolume.value >= 0 ? weaponVolume.value * 4 : weaponVolume.value;
        enemyVolume.value = PlayerPrefs.GetFloat("EVolume", enemyVolume.value >= 0 ? enemyVolume.value / 4 : enemyVolume.value);
        enemyVolume.value = enemyVolume.value >= 0 ? enemyVolume.value * 4 : enemyVolume.value;
        playerVolume.value = PlayerPrefs.GetFloat("PlVolume", playerVolume.value >= 0 ? playerVolume.value / 4 : playerVolume.value);
        playerVolume.value = playerVolume.value >= 0 ? playerVolume.value * 4 : playerVolume.value;
        pickupVolume.value = PlayerPrefs.GetFloat("PiVolume", pickupVolume.value >= 0 ? pickupVolume.value / 4 : pickupVolume.value);
        pickupVolume.value = pickupVolume.value >= 0 ? pickupVolume.value * 4 : pickupVolume.value;
        uiVolume.value = PlayerPrefs.GetFloat("UIVolume", uiVolume.value >= 0 ? uiVolume.value / 4 : uiVolume.value);
        uiVolume.value = uiVolume.value >= 0 ? uiVolume.value * 4 : uiVolume.value;
        miscVolume.value = PlayerPrefs.GetFloat("MiscVolume", miscVolume.value >= 0 ? miscVolume.value / 4 : miscVolume.value);
        miscVolume.value = miscVolume.value >= 0 ? miscVolume.value * 4 : miscVolume.value;

        screenResolution.value = PlayerPrefs.GetInt("Resolution", 0);

        fullScreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        viewBobbingToggle.isOn = PlayerPrefs.GetInt("HeadBob", 1) == 1;

        /*if (!PlayerPrefs.HasKey("TutorialPrompts") || PlayerPrefs.GetInt("TutorialPrompts") == 1)
            tutorialToggle.isOn = true;
        else
            tutorialToggle.isOn = false;*/
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

        //Clear Selected
        EventSystem.current.SetSelectedGameObject(null);

        //Select default starting button
        EventSystem.current.SetSelectedGameObject(pauseGameStart);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //Clear Selected
        EventSystem.current.SetSelectedGameObject(null);

        //Select default starting button
        EventSystem.current.SetSelectedGameObject(pauseGameStart);

        if(OnPause != null)
        {
            OnPause.Invoke();
        }
    }
    public void ResumeGame() 
    {
        UIAudio.PlayOneShot(buttonClick);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isGamePaused = false;

        if (options.activeSelf)
        {
            SaveSettings();
        }

        options.SetActive(false);
        menu.SetActive(false);

        if (OnResume != null)
        {
            OnResume.Invoke();
        }
    }

    public void ReturnToMainMenu(int whichMenu)
    {
        UIAudio.PlayOneShot(buttonClick);
        switch (whichMenu)
        {
            case 1:
                SaveSettings();

                //Clear Selected
                EventSystem.current.SetSelectedGameObject(null);

                //Select default starting button
                EventSystem.current.SetSelectedGameObject(mainMenuStart);

                options.SetActive(false);
                menu.SetActive(true);
                if(difficultyScreen) difficultyScreen?.SetActive(false);
                if (achievementsScreen) achievementsScreen.SetActive(false);
                if (weaponSelectScreen) weaponSelectScreen.SetActive(false);

                break;
            case 2:
                howToPlayUI.SetActive(false);
                menu.SetActive(true);

                //Clear Selected
                EventSystem.current.SetSelectedGameObject(null);

                //Select default starting button
                EventSystem.current.SetSelectedGameObject(weaponSelectStart);

                if (difficultyScreen) difficultyScreen?.SetActive(false);
                if(achievementsScreen) achievementsScreen.SetActive(false);
                if (weaponSelectScreen) weaponSelectScreen.SetActive(false);
                break;
        }
    }
    public void DisplayHowToPlayMenu() 
    {
        UIAudio.PlayOneShot(buttonClick);
        menu.SetActive(false);
        howToPlayUI.SetActive(true);
        achievementsScreen.SetActive(false);
        introSkipScreen.SetActive(false);

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
        //Clear Selected
        EventSystem.current.SetSelectedGameObject(null);

        //Select default starting button
        EventSystem.current.SetSelectedGameObject(difficultySelectStart);

        UIAudio.PlayOneShot(buttonClick);
        menu.SetActive(false);
        howToPlayUI.SetActive(false);
        options.SetActive(false);
        difficultyScreen.SetActive(true);
        achievementsScreen.SetActive(false);
        weaponSelectScreen.SetActive(false);
        introSkipScreen.SetActive(false);
    }
    public void DisplayAchievementsScreen()
    {
        UIAudio.PlayOneShot(buttonClick);
        menu.SetActive(false);
        howToPlayUI.SetActive(false);
        options.SetActive(false);
        difficultyScreen.SetActive(false);
        achievementsScreen.SetActive(true);
        weaponSelectScreen.SetActive(false);
        introSkipScreen.SetActive(false);

        achievementDisplay.DisplayAchievements();
    }
    public void DisplayWeaponSelectScreen()
    {
        //Clear Selected
        EventSystem.current.SetSelectedGameObject(null);

        //Select default starting button
        EventSystem.current.SetSelectedGameObject(weaponSelectStart);

        UIAudio.PlayOneShot(buttonClick);
        menu.SetActive(false);
        howToPlayUI.SetActive(false);
        options.SetActive(false);
        difficultyScreen.SetActive(false);
        achievementsScreen.SetActive(false);
        weaponSelectScreen.SetActive(true);
        introSkipScreen.SetActive(false);
    }
    public void DisplayIntroSkipScreen()
    {
        //Clear Selected
        EventSystem.current.SetSelectedGameObject(null);

        //Select default starting button
        EventSystem.current.SetSelectedGameObject(skipIntroStart);

        UIAudio.PlayOneShot(buttonClick);
        menu.SetActive(false);
        howToPlayUI.SetActive(false);
        options.SetActive(false);
        difficultyScreen.SetActive(false);
        achievementsScreen.SetActive(false);
        weaponSelectScreen.SetActive(false);
        introSkipScreen.SetActive(true);
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

    public void NavigateWeaponSelection(int dir)
    {
        if(dir != 0)
        {
            UIAudio.PlayOneShot(buttonClick);
        }

        weaponSelection += dir;
        if (weaponSelection < 0)
        {
            weaponSelection = weaponInfos.Length - 1;
        }
        weaponSelection = weaponSelection % weaponInfos.Length;

        if (weaponSelection <= 2)
        {
            weaponImage.sprite = weaponInfos[weaponSelection].gunIcon;
            weaponImage.color = Color.white;
            weaponName.text = weaponInfos[weaponSelection].weaponName;
            weaponDescription.text = weaponInfos[weaponSelection].weaponDescription;

            damageSlider.fillAmount = weaponInfos[weaponSelection].damageRating / 10f;
            speedSlider.fillAmount = weaponInfos[weaponSelection].speedRating / 10f;
            rangeSlider.fillAmount = weaponInfos[weaponSelection].rangeRating / 10f;

            PlayerPrefs.SetInt("Weapon", weaponSelection);
            SaveData.updateCurrentWeapon(weaponSelection);
        } else
        {
            weaponImage.sprite = weaponInfos[weaponSelection].gunIcon;
            weaponImage.color = Color.black;
            weaponName.text = "???";
            weaponDescription.text = "Classified";
            damageSlider.fillAmount = 0 / 10f;
            speedSlider.fillAmount = 0 / 10f;
            rangeSlider.fillAmount = 0 / 10f;
        }
    }

    public void SetDifficulty(int value)
    {
        print("selected weapon: " + PlayerPrefs.GetInt("Weapon"));
        //UIAudio.PlayOneShot(buttonClick);
        Score.SetDifficulty(value);
        SaveData.updateCurrentDifficulty(value);

        //Wah Wah! achievement (Play Baby mode)
        if(value == 0)
        {
            AchievementManager.displayAchievement("Wah Wah!");
        }
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
