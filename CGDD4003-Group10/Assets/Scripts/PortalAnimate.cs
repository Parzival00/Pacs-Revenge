using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalAnimate : MonoBehaviour
{
    [SerializeField] float rotateSpeed = 20;
    [SerializeField] int frameRate = 8;

    float timer = 0;

    float interval;

    private void Start()
    {
        interval = 1f / frameRate;
    }

    // Update is called once per frame
    void Update()
    {
        if(timer >= interval)
        {
            transform.Rotate(Vector3.forward * rotateSpeed * interval, Space.Self);
            timer = 0;
        }
        timer += Time.deltaTime;
    }
}
