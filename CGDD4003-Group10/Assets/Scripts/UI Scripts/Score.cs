using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Score : MonoBehaviour
{
    public static int score { get; private set; }
    public static int pelletsCollected {get; private set; }
    public static int pelletsLeft { get; private set; }
    public static bool indicatorActive { get; private set; }
    public static int currentLevel { get; private set; }
    public static int difficulty { get; private set; }
    public static bool won { get; private set; }

    [SerializeField] TMP_Text scoreUI;
    [SerializeField] TMP_Text pelletRemaining;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip pelletSound;
    [SerializeField] int pelletsPerAmmo;
    [SerializeField] float indicatorTimerThreshold = 10;
    [SerializeField] RenderTexture gameSceneRenderTex;

    private int totalPellets;

    float timeSinceLastPellet;

    PlayerController playerController;
    Camera gameSceneCamera;

    HUDMessenger hudMessenger;

    static float scoreMultiplier;

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
            score = 0;

        playerController = this.gameObject.GetComponent<PlayerController>();
        gameSceneCamera = Camera.main;// GameObject.FindGameObjectWithTag("GameSceneCamera")?.GetComponent<Camera>();
        hudMessenger = FindObjectOfType<HUDMessenger>();

        won = false;
    }


    void Update()
    {
        UpdateScore();
        UpdatePelletsRemaining();

        timeSinceLastPellet += Time.deltaTime;

        indicatorActive = timeSinceLastPellet >= indicatorTimerThreshold;
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Pellet")
        {
            playerController.faceController.EatPellet();

            other.gameObject.SetActive(false);
            audioSource.PlayOneShot(pelletSound);
            pelletsCollected += 1;
            timeSinceLastPellet = 0;
            AddToScore(50);
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
                playerController.faceController.EatFruit();

                FruitController.FruitInfo fruitInfo = fruitController.CollectFruit();

                AddToScore(fruitInfo.pointsWorth);
                switch (fruitInfo.powerUp)
                {
                    case FruitController.PowerUpType.Shield:
                        playerController.AddShields();
                        hudMessenger.Display("Shield Obtained", 1);
                        break;
                    case FruitController.PowerUpType.Invisibility:
                        hudMessenger.Display("Invisibility Activated", 1);
                        playerController.ActivateInvisibility();
                        break;
                    case FruitController.PowerUpType.Speed:
                        hudMessenger.Display("Speed Boost Activated", 1);
                        playerController.ActivateSpeed();
                        break;
                    case FruitController.PowerUpType.EnhancedRadar:
                        hudMessenger.Display("Enhanced Radar Activated", 1);
                        Radar radar = GameObject.Find("Radar").GetComponent<Radar>();
                        radar.StartEnhancedRadar();
                        break;
                    case FruitController.PowerUpType.ExtraLife:
                        playerController.AddLives();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void UpdateScore() 
    {
        scoreUI.text = "" + score;
    }

    public static void AddToScore(int amount)
    {
        score += (int)(amount * scoreMultiplier);
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
        won = true;

        gameSceneRenderTex.Create();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
