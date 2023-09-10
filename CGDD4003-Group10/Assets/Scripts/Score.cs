using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    static int score = 0;
    public GameObject textUI;
    void Start()
    {
        
    }

    
    void Update()
    {
        updateScore();
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
    }


    void updateScore() 
    {
        textUI.GetComponent<Text>().text = "Score: " + score;
    }

    public static void AddToScore(int amount)
    {
        score += amount;
    }
}
