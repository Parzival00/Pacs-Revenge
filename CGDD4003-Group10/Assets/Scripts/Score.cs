using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    int powerPellet = 0;
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision other)
    {
        //Make if statement with tag
        Destroy(other.gameObject);
        powerPellet++;
    }
    public void getScore() 
    {
        //Add all points up
    }
}
