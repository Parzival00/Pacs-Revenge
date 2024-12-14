using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserWallActivator : MonoBehaviour
{
    [SerializeField] LaserWall laserWall;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartCooldown()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            if (!laserWall.activated && !laserWall.onCooldown)
            {
                laserWall.ActivateLaser(this);
            }
        }
    }
}
