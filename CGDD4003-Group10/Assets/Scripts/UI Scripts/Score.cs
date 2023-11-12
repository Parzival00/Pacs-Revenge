using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Threading;

public class Score : MonoBehaviour
{
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

    [SerializeField] TMP_Text scoreUI;
    [SerializeField] TMP_Text pelletRemaining;
    [SerializeField] TMP_Text pointValueIndicator;
    [SerializeField] AudioSource munchAudioSource;
    [SerializeField] AudioSource radarAudioSource;
    [SerializeField] AudioClip pelletSound;
    [SerializeField] AudioClip radarPing;
    [SerializeField] int pelletsPerAmmo;
    [SerializeField] float indicatorTimerThreshold = 10; //radar pellet indicator timer threshold
    [SerializeField] float pointsAddedIndicatorLength = 1;
    [SerializeField] RenderTexture gameSceneRenderTex;

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

    //Various Stats
    public static int totalGhostKilled { get;  set; }
    public static int totalPelletsCollected { get; private set; }
    public static int totalShieldsRecieved { get;  set; }
    public static int totalLivesConsumed { get;  set; }
    public static int totalShotsFired { get; set; }
    public static int totalStunsFired { get; set; }
    public static float totalTimePlayed { get; private set; }

    static float sceneStartTime;

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

        wonLevel = false;

        pointValueIndicator.text = "";
        pointsIndicatorAmount = 0;
        previousScore = score;

        sceneStartTime = Time.time;
    }


    void Update()
    {
        if (!insanityEnding)
        {
            UpdateScore();
            UpdatePelletsRemaining();
            UpdatePointIndicator();

            timeSinceLastPellet += Time.deltaTime;

            indicatorActive = timeSinceLastPellet >= indicatorTimerThreshold;
        }
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
            if (pelletsCollected >= totalPellets)
            {
                playerController.SaveLives();
                StartCoroutine(SceneEnd());
            }
            else if (pelletsCollected % pelletsPerAmmo == 0)
            {
                playerController.AddAmmo();
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
        pointValueIndicator.text = "" + pointsIndicatorAmount;
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
            pointValueIndicator.text = "+" + pointsIndicatorAmount;
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

    public void SaveFruitsCollected()
    {
        PlayerPrefs.SetInt("FruitsCollected", fruitsCollected);
    }

    public IEnumerator SceneEnd()
    {
        gameSceneRenderTex.Release();
        gameSceneRenderTex.DiscardContents();

        gameSceneRenderTex.width = Screen.width;
        gameSceneRenderTex.height = Screen.height;

        yield return null;

        //gameSceneCamera.gameObject.SetActive(true);

        //gameSceneCamera.fieldOfView = PlayerPrefs.GetFloat("FOV");

        gameSceneCamera.targetTexture = gameSceneRenderTex;
        gameSceneCamera.Render();
        gameSceneCamera.targetTexture = null;
        wonLevel = true;

        gameSceneRenderTex.Create();

        SaveFruitsCollected();

        if(currentLevel == 8)
        {
            if(fruitsCollected == 16)
            {
                totalTimePlayed += Time.time - sceneStartTime;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            } else
            {
                GameEnd();
            }
        } else
        {
            totalTimePlayed += Time.time - sceneStartTime;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
}
