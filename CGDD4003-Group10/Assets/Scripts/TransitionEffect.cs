using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionEffect : MonoBehaviour
{
    [SerializeField] GameObject gameSceneTexture;
    [SerializeField] GameObject deathSceneTexture;
    [SerializeField] Material transitionMat;
    [SerializeField] float transitionLength;

    [SerializeField] Material deathTransitionMat;

    Material currentTransitionalMat;

    // Start is called before the first frame update
    void Start()
    {
        gameSceneTexture.SetActive(false);

        if(SceneManager.GetActiveScene().name == "GameOverScene" || SceneManager.GetActiveScene().name == "End")
        {
            if(PlayerPrefs.GetInt("Lives") == 0 || Score.insanityEnding)
            {
                currentTransitionalMat = deathTransitionMat;
                deathSceneTexture.SetActive(true);
            } else
            {
                currentTransitionalMat = transitionMat;
                gameSceneTexture.SetActive(true);
                deathSceneTexture.SetActive(false);
            }

            currentTransitionalMat.SetFloat("_TransitionProgress", 0);
            StartCoroutine(Transition());
        } 
        else // if (Score.currentLevel != 1)
        {
            currentTransitionalMat = transitionMat;
            gameSceneTexture.SetActive(true);
            if(deathSceneTexture)
                deathSceneTexture.SetActive(false);
            currentTransitionalMat.SetFloat("_TransitionProgress", 0);
            StartCoroutine(Transition());
        }
    }

    public void DeathTransition()
    {
        currentTransitionalMat = deathTransitionMat;
        deathSceneTexture.SetActive(true);
        gameSceneTexture.SetActive(false);

        currentTransitionalMat.SetFloat("_TransitionProgress", 0);
        StartCoroutine(Transition());
    }

    IEnumerator Transition()
    {
        yield return new WaitForSeconds(transitionLength / 8);

        float stepSize = 1 / transitionLength;
        float progress = 0;
        while(progress < 1)
        {
            progress += stepSize * Time.deltaTime;
            currentTransitionalMat.SetFloat("_TransitionProgress", progress);
            yield return null;
        }

        currentTransitionalMat.SetFloat("_TransitionProgress", 1);
        yield return null;
        gameSceneTexture.SetActive(false);

        if(deathSceneTexture)
            deathSceneTexture?.SetActive(false);
        currentTransitionalMat.SetFloat("_TransitionProgress", 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
