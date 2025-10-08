using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptedGun : MonoBehaviour
{
    [System.Serializable]
    public struct DifficultySetting
    {
        public int tentacleKillsRequired;
    }

    [SerializeField] CaptureTentacle[] tentacles;
    [SerializeField] bool activate;
    [SerializeField] AudioClip activateSound;
    [SerializeField] DifficultySetting[] difficultySettings;
    DifficultySetting currentDifficultySettings;

    PlayerController player;
    AudioSource audio;

    int totalTentacleAmount;
    int killedTentacleCount;

    // Start is called before the first frame update
    void Start()
    {
        tentacles = GetComponentsInChildren<CaptureTentacle>();
        totalTentacleAmount = tentacles.Length;
        audio = GetComponent<AudioSource>();

        killedTentacleCount = 0;

        for (int i = 0; i < totalTentacleAmount; i++)
        {
            tentacles[i].gameObject.SetActive(false);
        }

        if (Score.difficulty < difficultySettings.Length)
        {
            currentDifficultySettings = difficultySettings[Score.difficulty];
        }
        else
        {
            currentDifficultySettings = difficultySettings[0];
        }
    }

    private void Update()
    {
        if(activate)
        {
            ActivateEntrapment(FindObjectOfType<PlayerController>());
            activate = false;
        }
    }

    public void ActivateEntrapment(PlayerController player)
    {
        if (deactivation != null)
        {
            StopCoroutine(deactivation);
        }

        this.player = player;

        transform.position = player.transform.position;

        audio.PlayOneShot(activateSound);

        player.SetTrapped(true);

        for (int i = 0; i < totalTentacleAmount; i++)
        {
            tentacles[i].gameObject.SetActive(true);
            tentacles[i].ActivateTentacle();
        }

        killedTentacleCount = 0;
    }

    public void TentacleKilled()
    {
        killedTentacleCount++;

        if(killedTentacleCount >= Mathf.Min(currentDifficultySettings.tentacleKillsRequired, totalTentacleAmount))
        {
            deactivation = StartCoroutine(DeactivateEntrapment());
        }
    }

    Coroutine deactivation;
    public IEnumerator DeactivateEntrapment()
    {
        if (player)
            player.SetTrapped(false);

        for (int i = 0; i < totalTentacleAmount; i++)
        {
            tentacles[i].RetreatTentacle();
        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < totalTentacleAmount; i++)
        {
            tentacles[i].gameObject.SetActive(false);
        }

        deactivation = null;
    }
}
