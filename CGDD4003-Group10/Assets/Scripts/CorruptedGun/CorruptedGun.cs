using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptedGun : MonoBehaviour
{
    [SerializeField] CaptureTentacle[] tentacles;
    [SerializeField] bool activate;

    PlayerController player;

    int totalTentacleAmount;
    int killedTentacleCount;

    // Start is called before the first frame update
    void Start()
    {
        tentacles = GetComponentsInChildren<CaptureTentacle>();
        totalTentacleAmount = tentacles.Length;

        killedTentacleCount = 0;

        for (int i = 0; i < totalTentacleAmount; i++)
        {
            tentacles[i].gameObject.SetActive(false);
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
        if (deactivation != null) StopCoroutine(deactivation);

        this.player = player;

        transform.position = player.transform.position;

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

        if(killedTentacleCount >= totalTentacleAmount)
        {
            deactivation = StartCoroutine(DeactivateEntrapment());
        }
    }

    Coroutine deactivation;
    public IEnumerator DeactivateEntrapment()
    {
        player.SetTrapped(false);

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < totalTentacleAmount; i++)
        {
            tentacles[i].gameObject.SetActive(false);
        }

        deactivation = null;
    }
}
