using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAudioController : MonoBehaviour
{
    AudioSource playerMusic;
    Vector3 playerPosition;
    float originalVol;
    float rolloffStartDistance;
    
    void Start()
    {
        playerMusic = GameObject.Find("Music").GetComponent<AudioSource>();
        playerPosition = GameObject.Find("Player").transform.position;
        originalVol = playerMusic.volume;
        rolloffStartDistance = this.gameObject.GetComponent<AudioSource>().maxDistance * 1.25f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(this.transform.position, playerPosition) <= rolloffStartDistance && !PlayerController.gunActivated)
        {
            playerMusic.volume = originalVol * Mathf.Log(Vector3.Distance(this.transform.position, playerPosition), rolloffStartDistance * 3);
        }
    }
}
