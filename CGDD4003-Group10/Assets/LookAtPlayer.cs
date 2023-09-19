using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    Transform player;
    private void Start()
    {
        player = GameObject.Find("Player").transform;
    }
    void Update()
    {
        this.transform.LookAt(player);   
    }
}
