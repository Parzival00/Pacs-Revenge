using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    Transform player;
    bool isPlayerAvailable;
    private void Start()
    {
        player = GameObject.Find("Player").transform;
    }
    void Update()
    {
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
    }
}
