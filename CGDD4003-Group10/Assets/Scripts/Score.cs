using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Score : MonoBehaviour
{
    public static int score { get; private set; }
    public static int pelletsCollected {get; private set; }

    [SerializeField] TMP_Text scoreUI;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip pelletSound;
    [SerializeField] GameObject cherryObject;
    [SerializeField] int cherrySpawn1, cherrySpawn2;

    private int totalPellets;
    void Start()
    {
        pelletsCollected = 0;
        score = 0;
        GameObject[] pellets = GameObject.FindGameObjectsWithTag("Pellet");
        totalPellets = pellets.Length;
        cherryObject.SetActive(false);
    }


    void Update()
    {
        UpdateScore();
    }

    /*private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "powerPellet") 
        {
            other.gameObject.SetActive(false);
            //Destroy(other.gameObject);
            score += 50;
        }

        if (other.gameObject.tag == "Cherry") 
        {
            other.gameObject.SetActive(false);
            //Destroy(other.gameObject);
            score += 100;
        }
    }*/
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Pellet")
        {
            other.gameObject.SetActive(false);
            audioSource.PlayOneShot(pelletSound);
            pelletsCollected += 1;
            //Destroy(other.gameObject);
            score += 50;
            if (pelletsCollected >= totalPellets)
            {
                SceneManager.LoadScene(2);
            }
            else if (pelletsCollected == cherrySpawn1 || pelletsCollected == cherrySpawn2)
            {
                cherryObject.SetActive(true);
            }
        }

        if (other.gameObject.tag == "Cherry")
        {
            other.gameObject.SetActive(false);
            //Destroy(other.gameObject);
            score += 1000;
        }
    }


    void UpdateScore() 
    {
        scoreUI.text = "Score: " + score;
    }

    public static void AddToScore(int amount)
    {
        score += amount;
    }
}
