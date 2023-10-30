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

    [SerializeField] TMP_Text scoreUI;
    [SerializeField] TMP_Text pelletRemaining;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip pelletSound;
    [SerializeField] int pelletsPerAmmo;
    [SerializeField] float indicatorTimerThreshold = 10;
    //[SerializeField] GameObject cherryObject;
    //[SerializeField] int cherrySpawn1, cherrySpawn2;

    private int totalPellets;

    float timeSinceLastPellet;

    PlayerController playerController;

    void Awake()
    {
        pelletsCollected = 0;
        score = 0;
        GameObject[] pellets = GameObject.FindGameObjectsWithTag("Pellet");
        totalPellets = pellets.Length;

        pelletsLeft = totalPellets - pelletsCollected;

        currentLevel = SceneManager.GetActiveScene().buildIndex; //in the build settings, the game levels come right after the main menu so the build index corresponds with the level number

        playerController = this.gameObject.GetComponent<PlayerController>();
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
            score += 50;
            if (pelletsCollected >= totalPellets)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
                score += fruitInfo.pointsWorth;
                switch(fruitInfo.powerUp)
                {
                    case FruitController.PowerUpType.Shield:
                        playerController.AddShields();
                        break;
                    case FruitController.PowerUpType.Invisibility:
                        playerController.ActivateInvisibility();
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
        score += amount;
    }

    public void UpdatePelletsRemaining() 
    {
         pelletsLeft = totalPellets - pelletsCollected;
         pelletRemaining.text = "" + pelletsLeft;
    }
}
