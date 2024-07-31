using System.Collections;
using UnityEngine;

public class BossArenaSetUp : MonoBehaviour
{
    [SerializeField] float startDelay = 2.5f;
    [Header("Camera Shake")]
    [SerializeField] CameraShake cameraShake;
    [SerializeField] float shakeStrength;

    [Header("Wall Variables")]
    [SerializeField] Transform allInnerWalls;
    [SerializeField] Vector3 wTargetY = new Vector3(0,-10,0);
    [SerializeField] float wSpeed = 0.5f;

    [Header("Pillar Variables")]
    [SerializeField] Transform mapPillars;
    [SerializeField] Vector3 pTargetY = new Vector3(0, -10, 0);
    [SerializeField] float pSpeed = 0.5f;

    [Header("Audio")]
    [SerializeField] AudioSource wallRevealSound;

    void Start()
    {
        Invoke("BossStart", startDelay);
    }

    private void BossStart()
    {
        wallRevealSound.Play();

        allInnerWalls = GetComponent<Transform>();
        Debug.Log("Got empty objects transform");
        StartCoroutine(LowerInnerWalls());
        Debug.Log("Walls have moved down?");
        StartCoroutine(RaisePillars());
    }

    public IEnumerator LowerInnerWalls() 
    {
        while (allInnerWalls.position != wTargetY) 
        {
            Debug.Log("Method has runned");

            allInnerWalls.position = Vector3.MoveTowards(allInnerWalls.position,wTargetY,wSpeed * Time.deltaTime);

            yield return null;
        }
    }
    public IEnumerator RaisePillars()
    {
        cameraShake.ShakeCamera(shakeStrength, 0.5f, Mathf.Abs(mapPillars.position.y - pTargetY.y) / pSpeed, false);
        while (mapPillars.position != pTargetY) 
        {
            Debug.Log("Winner Winner Chicken Dinner");

            mapPillars.position = Vector3.MoveTowards(mapPillars.position,pTargetY, pSpeed * Time.deltaTime);

            yield return null;
        }

        Destroy(allInnerWalls.gameObject,6);
    }
}
