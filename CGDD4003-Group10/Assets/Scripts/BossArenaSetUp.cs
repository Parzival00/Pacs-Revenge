using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Jobs;

public class BossArenaSetUp : MonoBehaviour
{

    Transform allInnerWalls;

    Vector3 varToLowerWalls;

    void Start()
    {
        allInnerWalls = GetComponent<Transform>();
        Debug.Log("Got empty objects transform");
        LowerInnerWalls();
        Debug.Log("Walls have moved down?");
    }

    void Update()
    {

    }

    public void LowerInnerWalls() 
    {
        while (allInnerWalls.position.y >= -10) 
        {
            allInnerWalls.Translate(-(transform.up) * Time.deltaTime );
        }
    }
}
