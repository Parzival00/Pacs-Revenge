using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Jobs;

public class BossArenaSetUp : MonoBehaviour
{
    [SerializeField]
    [Header("Wall Variables")]
    Transform allInnerWalls;

    public Vector3 wTargetY = new Vector3(0,-10,0);

    public float wSpeed = 0.5f;

    [Header("Pillar Variables")]
    public Transform mapPillars;

    public Vector3 pTargetY = new Vector3(0, -10, 0);

    public float pSpeed = 0.5f;

    void Start()
    {

        allInnerWalls = GetComponent<Transform>();
        Debug.Log("Got empty objects transform");
        StartCoroutine(LowerInnerWalls());
        Debug.Log("Walls have moved down?");
        StartCoroutine(RaisePillars());
    }

    void Update()
    {

    }

    public IEnumerator LowerInnerWalls() 
    {
        while (allInnerWalls.position != wTargetY) 
        {
            Debug.Log("Method has runned");

            allInnerWalls.position = Vector3.MoveTowards(allInnerWalls.position,wTargetY,wSpeed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }
    }
    public IEnumerator RaisePillars()
    {
        while(mapPillars.position != pTargetY) 
        {
            Debug.Log("Winner Winner Chicken Dinner");

            mapPillars.position = Vector3.MoveTowards(mapPillars.position,pTargetY, pSpeed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }

        Destroy(allInnerWalls.gameObject,5);
    }
}
