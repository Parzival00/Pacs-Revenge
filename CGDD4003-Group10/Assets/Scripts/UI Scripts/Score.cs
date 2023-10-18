using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Score : MonoBehaviour
{
    public static int score { get; private set; }
    public static int pelletsCollected {get; private set; }

    public static int pelletsLeft;

    [SerializeField] TMP_Text scoreUI;
    [SerializeField] TMP_Text pelletRemaining;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip pelletSound;
    [SerializeField] int pelletsPerAmmo;
    //[SerializeField] GameObject cherryObject;
    //[SerializeField] int cherrySpawn1, cherrySpawn2;

    private int totalPellets;
    void Start()
    {
        pelletsCollected = 0;
        score = 0;
        GameObject[] pellets = GameObject.FindGameObjectsWithTag("Pellet");
        totalPellets = pellets.Length;
        
    }


    void Update()
    {
        UpdateScore();
        UpdatePelletsRemaining();
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Pellet")
        {
            other.gameObject.SetActive(false);
            audioSource.PlayOneShot(pelletSound);
            pelletsCollected += 1;
            score += 50;
            if (pelletsCollected >= totalPellets)
            {
                SceneManager.LoadScene(2);
            }
            else if (pelletsCollected % pelletsPerAmmo == 0)
            {
                PlayerController pc = this.gameObject.GetComponent<PlayerController>();
                pc.AddAmmo();
            }
        }

        if (other.gameObject.tag == "Fruit")
        {
            FruitController fruitController = other.gameObject.GetComponent<FruitController>();

            if(fruitController)
            {
                score += fruitController.CollectFruit();
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
