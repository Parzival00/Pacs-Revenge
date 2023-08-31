using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureOverlay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform player;
    [SerializeField] SpriteRenderer spriteRenderer;

    [Header("Sprites")]
    [SerializeField] Sprite north;
    [SerializeField] Sprite northeast;
    [SerializeField] Sprite east;
    [SerializeField] Sprite southeast;
    [SerializeField] Sprite south;
    [SerializeField] Sprite southwest;
    [SerializeField] Sprite west;
    [SerializeField] Sprite northwest;

    [Header("Rotation Thresholds")]
    [SerializeField] [Range(0, 11.25f)] float thresholdPadding = 2f;
    [SerializeField] [Range(-180f, 180f)] float northMaxThreshold = 22.5f;
    [SerializeField] [Range(-180f, 180f)] float northMinThreshold = -22.5f;
    [SerializeField] [Range(-180f, 180f)] float northeastMaxThreshold = -22.5f;
    [SerializeField] [Range(-180f, 180f)] float northeastMinThreshold = -67.5f;
    [SerializeField] [Range(-180f, 180f)] float eastMaxThreshold = -67.5f;
    [SerializeField] [Range(-180f, 180f)] float eastMinThreshold = -112.5f;
    [SerializeField] [Range(-180f, 180f)] float southeastMaxThreshold = -112.5f;
    [SerializeField] [Range(-180f, 180f)] float southeastMinThreshold = -157.5f;
    [SerializeField] [Range(-180f, 180f)] float southMaxThreshold = -157.5f;
    [SerializeField] [Range(-180f, 180f)] float southMinThreshold = 157.5f;
    [SerializeField] [Range(-180f, 180f)] float southwestMaxThreshold = 157.5f;
    [SerializeField] [Range(-180f, 180f)] float southwestMinThreshold = 112.5f;
    [SerializeField] [Range(-180f, 180f)] float westMaxThreshold = 112.5f;
    [SerializeField] [Range(-180f, 180f)] float westMinThreshold = 67.5f;
    [SerializeField] [Range(-180f, 180f)] float northwestMaxThreshold = 67.5f;
    [SerializeField] [Range(-180f, 180f)] float northwestMinThreshold = 22.5f;

    //Vector3 currentDirection;

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = transform.forward;
        Vector3 dirToPlayer = (new Vector3(player.position.x, 0, player.position.z) - new Vector3(transform.position.x, 0, transform.position.y)).normalized; //to - from
        float angleBtwPlayer = Vector3.SignedAngle(forward, dirToPlayer, transform.up);

        if(angleBtwPlayer < northMaxThreshold - thresholdPadding && angleBtwPlayer > northMinThreshold + thresholdPadding)
        {
            spriteRenderer.sprite = north;
            //currentDirection = Vector3.forward;
        } 
        else if (angleBtwPlayer < northeastMaxThreshold - thresholdPadding && angleBtwPlayer > northeastMinThreshold + thresholdPadding)
        {
            spriteRenderer.sprite = northeast;
            //currentDirection = new Vector3(-1, 0, 1).normalized;
        }
        else if (angleBtwPlayer < eastMaxThreshold - thresholdPadding && angleBtwPlayer > eastMinThreshold + thresholdPadding)
        {
            spriteRenderer.sprite = east;
            //currentDirection = Vector3.left;
        }
        else if (angleBtwPlayer < southeastMaxThreshold - thresholdPadding && angleBtwPlayer > southeastMinThreshold + thresholdPadding)
        {
            spriteRenderer.sprite = southeast;
            //currentDirection = new Vector3(-1, 0, -1).normalized;
        }
        else if (angleBtwPlayer < southMaxThreshold - thresholdPadding && angleBtwPlayer > southMinThreshold + thresholdPadding)
        {
            spriteRenderer.sprite = south;
            //currentDirection = Vector3.back;
        }
        else if (angleBtwPlayer < southwestMaxThreshold - thresholdPadding && angleBtwPlayer > southwestMinThreshold + thresholdPadding)
        {
            spriteRenderer.sprite = southwest;
            //currentDirection = new Vector3(1, 0, -1).normalized;
        }
        else if (angleBtwPlayer < westMaxThreshold - thresholdPadding && angleBtwPlayer > westMinThreshold + thresholdPadding)
        {
            spriteRenderer.sprite = west;
            //currentDirection = Vector3.right;
        }
        else if (angleBtwPlayer < northwestMaxThreshold - thresholdPadding && angleBtwPlayer > northwestMinThreshold + thresholdPadding)
        {
            spriteRenderer.sprite = northwest;
            //currentDirection = new Vector3(1, 0, 1).normalized;
        }

        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        //spriteRenderer.transform.rotation *= Quaternion.LookRotation(dirToPlayer, Vector3.up);//  * spriteRenderer.transform.rotation;
    }
}
