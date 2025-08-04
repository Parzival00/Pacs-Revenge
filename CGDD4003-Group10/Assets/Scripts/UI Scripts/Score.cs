using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [System.Serializable]
    public struct DifficultySettings
    {
        public int difficultyLevel;
        public float bossStungunRechargeRate;
        public float bossTimerAmount;
    }

    public static int score { get; private set; }
    public static int pelletsCollected {get; private set; }
    public static int pelletsLeft { get; private set; }
    
    public static bool indicatorActive { get; private set; }
    public static int fruitsCollected { get; private set; }
    public static int currentLevel { get; private set; }
    public static int difficulty { get; private set; }
    public static bool wonLevel { get; private set; }
    public static bool insanityEnding { get; private set; }
    public static bool bossEnding { get; private set; }
    public static int ending { get; private set; }

    public static bool bossTimerEnded { get; private set; }
    public float bossfightProgress { get { return 1 - bossTimer / (currentDifficultySettings.bossTimerAmount * 60f); } }

    [SerializeField] TMP_Text scoreUI;
    [SerializeField] TMP_Text pelletRemaining;
    [SerializeField] TMP_Text pointValueIndicator;
    [SerializeField] GameObject bossTimerParent;
    [SerializeField] TMP_Text bossFightTimerText;
    [SerializeField] Image bossTimerEndRedFlash;
    [SerializeField] Color bossTimerEndBackgroundColor;
    [SerializeField] GameObject bossfightPortal;
    [SerializeField] AudioSource munchAudioSource;
    [SerializeField] AudioSource radarAudioSource;
    [SerializeField] AudioClip pelletSound;
    [SerializeField] AudioClip radarPing;
    [SerializeField] float indicatorTimerThreshold = 10; //radar pellet indicator timer threshold
    [SerializeField] float pointsAddedIndicatorLength = 1;
    [SerializeField] RenderTexture gameSceneRenderTex;
    [SerializeField] float bossFightStartDelay = 15f;
    [SerializeField] float bossFightPortalMaxSize = 15f;
    [SerializeField] DifficultySettings[] difficultySettings;
    [SerializeField] bool demo;

    Vector3 bossfightPortalStartSize;

    DifficultySettings currentDifficultySettings;

    private int totalPellets;
    private static int pointsIndicatorAmount;
    private static Color currentTarget = Color.yellow;
    private float pointsIndicatorTimer = 0;
    private static int previousScore;
    
    float timeSinceLastPellet;

    PlayerController playerController;
    Camera gameSceneCamera;

    HUDMessenger hudMessenger;

    static float scoreMultiplier;

    float bossTimer;

    public static bool startBossTimer { get; set; }

    //Various Stats
    public static int totalGhostKilled { get;  set; }
    public static int totalPelletsCollected { get; private set; }
    public static int totalShieldsRecieved { get;  set; }
    public static int totalLivesConsumed { get;  set; }
    public static int totalShotsFired { get; set; }
    public static int totalStunsFired { get; set; }
    public static float totalTimePlayed { get; private set; }
    public static int timesOverheated { get; set; }

    static float sceneStartTime;

    float bossStungunRechargeTimer;

    void Awake()
    {
        pelletsCollected = 0;
        GameObject[] pellets = GameObject.FindGameObjectsWithTag("Pellet");
        totalPellets = pellets.Length;

        pelletsLeft = totalPellets - pelletsCollected;

        currentLevel = SceneManager.GetActiveScene().buildIndex; //in the build settings, the game levels come right after the main menu so the build index corresponds with the level number

        difficulty = PlayerPrefs.GetInt("Difficulty");

        switch(difficulty)
        {
            case 0:
                scoreMultiplier = 0.5f;
                break;
            case 1:
                scoreMultiplier = 1;
                break;
            case 2:
                scoreMultiplier = 2;
                break;
            default:
                scoreMultiplier = 1;
                break;
        }

        if (currentLevel == 1)
        {
            score = 0;
            totalGhostKilled = 0;
            totalPelletsCollected = 0;
            totalShieldsRecieved = 0;
            totalLivesConsumed = 0;
            totalShotsFired = 0;
            totalStunsFired = 0;
            totalTimePlayed = 0;

            fruitsCollected = 0;
            PlayerPrefs.SetInt("FruitsCollected", fruitsCollected);
        }
        else
        {
            fruitsCollected = PlayerPrefs.GetInt("FruitsCollected");
        }

        insanityEnding = currentLevel == 9;
        bossEnding = currentLevel == 10;

        playerController = this.gameObject.GetComponent<PlayerController>();
        gameSceneCamera = Camera.main;
        hudMessenger = FindObjectOfType<HUDMessenger>();

        Invoke("DisplayLevelNumber", 0.5f);

        AudioListener.volume = 1;
        wonLevel = false;

        pointValueIndicator.text = "";
        pointsIndicatorAmount = 0;
        previousScore = score;

        sceneStartTime = Time.time;

        if (Score.difficulty < difficultySettings.Length)
        {
            currentDifficultySettings = difficultySettings[Score.difficulty];
        }
        else
        {
            currentDifficultySettings = difficultySettings[0];
        }

        if (bossEnding)
        {
            bossStungunRechargeTimer = currentDifficultySettings.bossStungunRechargeRate;
            bossTimerEnded = false;
            bossTimer = currentDifficultySettings.bossTimerAmount * 60f;

            int seconds = Mathf.RoundToInt(bossTimer % 60f);
            int minutes = Mathf.RoundToInt(bossTimer / 60);
            bossFightTimerText.text = string.Format("{0:00.}", minutes) + ":" + string.Format("{0:00.}", seconds);

            bossfightPortalStartSize = bossfightPortal.transform.localScale;

            Invoke("StartBossTimer", bossFightStartDelay);
        }
    }

    void DisplayLevelNumber()
    {
        if (currentLevel <= 8)
        {
            hudMessenger.Display("Level " + currentLevel, 3);
            hudMessenger.Display("Collect all the pellets", 3);
        }
    }

    void StartBossTimer()
    {
        startBossTimer = true;

        bossTimerParent.SetActive(true);
    }


    void Update()
    {
        if (!insanityEnding && !bossEnding)
        {
            UpdateScore();
            UpdatePelletsRemaining();
            UpdatePointIndicator();

            timeSinceLastPellet += Time.deltaTime;

            indicatorActive = timeSinceLastPellet >= indicatorTimerThreshold;
        }
        else if (bossEnding)
        {
            UpdateScore();
            pelletRemaining.text = "99$%&!*";

            bossStungunRechargeTimer -= Time.deltaTime;

            if(bossStungunRechargeTimer <= 0)
            {
                pelletsCollected++;
                playerController.CheckToAddStunAmmo();

                bossStungunRechargeTimer = currentDifficultySettings.bossStungunRechargeRate;
            }

            if(startBossTimer == true && PlayerController.playerLives > 0 && !Boss.bossDead)
            {
                if (bossfightPortal) bossfightPortal.transform.localScale = Vector3.Lerp(bossfightPortalStartSize, Vector3.one * bossFightPortalMaxSize, bossfightProgress);

                bossTimer -= Time.deltaTime;
                if (bossFightTimerText) 
                {
                    if (bossTimer > 0)
                    {
                        int seconds = Mathf.RoundToInt(bossTimer % 60f);
                        int minutes = Mathf.FloorToInt(bossTimer / 60);
                        bossFightTimerText.text = string.Format("{0:00.}", minutes) + ":" + string.Format("{0:00.}", seconds);
                    } else
                    {
                        bossFightTimerText.text ="00:00";
                    }
                }

                if(bossTimer <= 0)
                {
                    bossTimerEnded = true;

                    if (!timerEndSequenceStarted) StartCoroutine(TimerEndSequence());
                }
            } else
            {
                if (bossTimerParent) bossTimerParent.SetActive(false);
            }
        }
    }
    bool timerEndSequenceStarted = false;

    IEnumerator TimerEndSequence()
    {
        timerEndSequenceStarted = true;

        bossTimerEndRedFlash.gameObject.SetActive(true);

        float timer = 0;
        Color newColor = bossTimerEndRedFlash.color;
        newColor.a = 0;
        bossTimerEndRedFlash.color = newColor;
        while (timer <= 0.35f)
        {
            timer += Time.deltaTime;
            yield return null;
            newColor.a = timer / 0.35f;
            bossTimerEndRedFlash.color = newColor;
        }
        newColor.a = 1;
        bossTimerEndRedFlash.color = newColor;

        gameSceneCamera.backgroundColor = bossTimerEndBackgroundColor;
        bossfightPortal.SetActive(false);

        yield return new WaitForSeconds(0.15f);

        timer = 0;
        while (timer <= 0.35f)
        {
            timer += Time.deltaTime;
            yield return null;
            newColor.a = 1 - timer / 0.35f;
            bossTimerEndRedFlash.color = newColor;
        }
        newColor.a = 0;
        bossTimerEndRedFlash.color = newColor;

        bossTimerEndRedFlash.gameObject.SetActive(false);
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Pellet" && !insanityEnding)
        {
            playerController.faceController.EatPellet();

            other.gameObject.SetActive(false);
            munchAudioSource.PlayOneShot(pelletSound);
            pelletsCollected += 1;
            totalPelletsCollected++;
            timeSinceLastPellet = 0;
            AddToScore(Color.white, 50);
            playerController.CheckToAddStunAmmo();

            if (pelletsCollected >= totalPellets)
            {
                playerController.SaveLives();
                StartCoroutine(SceneEnd(false));
            }
        }

        if (other.gameObject.tag == "Fruit")
        {
            FruitController fruitController = other.gameObject.GetComponent<FruitController>();

            if(fruitController)
            {
                if (insanityEnding)
                {
                    Ghost[] ghosts = new Ghost[4];
                    ghosts = FindObjectsOfType<Ghost>();

                    for (int i = 0; i < ghosts.Length; i++)
                    {
                        ghosts[i].EnableCorruptionEnding();
                    }

                    playerController.EnableCorruptionEnding();

                    fruitController.CollectFruit();
                    hudMessenger.CorruptedDisplay("Devour Them All!", 2);
                }
                else
                {
                    playerController.faceController.EatFruit();

                    FruitController.FruitInfo fruitInfo = fruitController.CollectFruit();
                    AddToScore(Color.white, fruitInfo.pointsWorth);
                    switch (fruitInfo.powerUp)
                    {
                        case FruitController.PowerUpType.Shield:
                            playerController.AddShields();
                            hudMessenger.Display("Shield Obtained", 1);
                            fruitsCollected++;
                            break;
                        case FruitController.PowerUpType.Invisibility:
                            hudMessenger.Display("Invisibility Activated", 1);
                            playerController.ActivateInvisibility();
                            fruitsCollected++;
                            break;
                        case FruitController.PowerUpType.Speed:
                            hudMessenger.Display("Speed Boost Activated", 1);
                            playerController.ActivateSpeed();
                            fruitsCollected++;
                            break;
                        case FruitController.PowerUpType.EnhancedRadar:
                            radarAudioSource.PlayOneShot(radarPing);
                            hudMessenger.Display("Enhanced Radar Activated", 1);
                            Radar radar = GameObject.Find("Radar").GetComponent<Radar>();
                            radar.StartEnhancedRadar();
                            fruitsCollected++;
                            break;
                        case FruitController.PowerUpType.ExtraLife:
                            playerController.AddLives();
                            fruitsCollected++;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    void UpdateScore() 
    {
        scoreUI.text = "" + score;
        //pointValueIndicator.text = "" + pointsIndicatorAmount;
    }

    public static void AddToScore(Color targetColor, int amount)
    {
        currentTarget = targetColor;
        int pointsToAdd = (int)(amount * scoreMultiplier);
        pointsIndicatorAmount += pointsToAdd;
        previousScore = score;
        score += pointsToAdd;
    }

    public void UpdatePointIndicator()
    {
        if (previousScore != score)
        {
            pointsIndicatorTimer = Time.time + pointsAddedIndicatorLength;
            previousScore = score;
        }

        if (pointsIndicatorTimer >= Time.time)
        {
            pointValueIndicator.color = currentTarget;
            //pointValueIndicator.text = "+" + pointsIndicatorAmount;
        } else
        {
            pointValueIndicator.text = "";
            pointsIndicatorAmount = 0;
        }
    }

    public void UpdatePelletsRemaining() 
    {
         pelletsLeft = totalPellets - pelletsCollected;
         pelletRemaining.text = "" + pelletsLeft;
    }

    public static void SetDifficulty(int value)
    {
        difficulty = value;
        PlayerPrefs.SetInt("Difficulty", difficulty);
    }

    public static void SaveFruitsCollected()
    {
        PlayerPrefs.SetInt("FruitsCollected", fruitsCollected);
    }

    public IEnumerator SceneEnd(bool playerDied)
    {
        if (!playerDied && !insanityEnding)
        {
            gameSceneRenderTex.Release();
            gameSceneRenderTex.DiscardContents();

            gameSceneRenderTex.width = Screen.width;
            gameSceneRenderTex.height = Screen.height;
        }

        wonLevel = true;

        hudMessenger.Display("Level Cleared", 2f);

        float audioLevel = 1;
        while(audioLevel > 0)
        {
            AudioListener.volume = audioLevel;
            audioLevel -= 0.5f * Time.deltaTime;
            yield return null;
        }

        audioLevel = 0;
        AudioListener.volume = audioLevel;

        yield return new WaitForSeconds(0.25f);

        if (!playerDied && !insanityEnding)
        {
            gameSceneCamera.targetTexture = gameSceneRenderTex;
            gameSceneCamera.Render();
            gameSceneCamera.targetTexture = null;

            gameSceneRenderTex.Create();
        }

        SaveFruitsCollected();

        if (playerDied)
        {
            if (bossEnding)
            {
                PlayerPrefs.SetInt("Ending", 0);
                SaveData.updateLevel(0);
                totalTimePlayed += Time.time - sceneStartTime;
                SceneManager.LoadScene("End");
            }
            else
            {
                SaveData.updateLevel(0);
                GameEnd();
            }
        }
        else
        {
            if(demo && currentLevel == 2)
            {
                PlayerPrefs.SetInt("Ending", 1);
                totalTimePlayed += Time.time - sceneStartTime;
                SaveData.updateLevel(0);
                SceneManager.LoadScene("DemoEnd");
            }
            else if (currentLevel == 8)
            {
                if (fruitsCollected >= 14)
                {
                    totalTimePlayed += Time.time - sceneStartTime;
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                }
                else
                {
                    totalTimePlayed += Time.time - sceneStartTime;
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
                }
            }
            else if (bossEnding)
            {
                PlayerPrefs.SetInt("Ending", 1);
                SaveData.updateLevel(0);
                totalTimePlayed += Time.time - sceneStartTime;
                SceneManager.LoadScene("End");
            }
            else if (insanityEnding)
            {
                PlayerPrefs.SetInt("Ending", 2);
                SaveData.updateLevel(0);
                totalTimePlayed += Time.time - sceneStartTime;
                SceneManager.LoadScene("End");
            }
            else
            {
                totalTimePlayed += Time.time - sceneStartTime;

                SaveData.updateLevel(SceneManager.GetActiveScene().buildIndex);

                switch (SceneManager.GetActiveScene().buildIndex)
                {
                    case 2:
                        SaveData.addWeaponUnlock("Rifle");
                        break;
                    case 5: 
                        SaveData.addWeaponUnlock("Shotgun");
                        break;
                    //Can add more weapons here later...
                }

                SaveData.Save();

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }

    /// <summary>
    /// End of the game so switch to gameover scene
    /// </summary>
    public static void GameEnd()
    {
        totalTimePlayed += Time.time - sceneStartTime;
        SceneManager.LoadScene("GameOverScene");
    }

    public static void BossEnd()
    {
        SaveFruitsCollected();
        totalTimePlayed += Time.time - sceneStartTime;
        //SceneManager.LoadScene("GameOverScene");
    }
}
