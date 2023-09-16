using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostSpriteController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform player;
    [SerializeField] Transform mainTransform;
    [SerializeField] Transform spriteTransform;
    //[SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator animator;

    [Header("Sprite GameObjects")]
    [SerializeField] RuntimeAnimatorController north;
    [SerializeField] AnimatorOverrideController northeast;
    [SerializeField] AnimatorOverrideController east;
    [SerializeField] AnimatorOverrideController southeast;
    [SerializeField] AnimatorOverrideController south;
    [SerializeField] AnimatorOverrideController southwest;
    [SerializeField] AnimatorOverrideController west;
    [SerializeField] AnimatorOverrideController northwest;

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

    private void Start()
    {
        if (mainTransform == null)
            mainTransform = transform;

        animator.runtimeAnimatorController = north;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = mainTransform.forward;
        Vector3 dirToPlayer = (new Vector3(player.position.x, 0, player.position.z) - new Vector3(mainTransform.position.x, 0, mainTransform.position.z)).normalized; //to - from
        float angleBtwPlayer = Vector3.SignedAngle(forward, dirToPlayer, mainTransform.up);

        if(angleBtwPlayer < northMaxThreshold - thresholdPadding && angleBtwPlayer > northMinThreshold + thresholdPadding)
        {
            animator.runtimeAnimatorController = north;
            //north.SetActive(true);
            //spriteRenderer.sprite = north;
            //currentDirection = Vector3.forward;
        } 
        else if (angleBtwPlayer < northeastMaxThreshold - thresholdPadding && angleBtwPlayer > northeastMinThreshold + thresholdPadding)
        {
            animator.runtimeAnimatorController = northeast;
            //northeast.SetActive(true);
            //spriteRenderer.sprite = northeast;
            //currentDirection = new Vector3(-1, 0, 1).normalized;
        }
        else if (angleBtwPlayer < eastMaxThreshold - thresholdPadding && angleBtwPlayer > eastMinThreshold + thresholdPadding)
        {
            animator.runtimeAnimatorController = east;
            //east.SetActive(true);
            //spriteRenderer.sprite = east;
            //currentDirection = Vector3.left;
        }
        else if (angleBtwPlayer < southeastMaxThreshold - thresholdPadding && angleBtwPlayer > southeastMinThreshold + thresholdPadding)
        {
            animator.runtimeAnimatorController = southeast;
            //southeast.SetActive(true);
            //spriteRenderer.sprite = southeast;
            //currentDirection = new Vector3(-1, 0, -1).normalized;
        }
        else if (angleBtwPlayer < southMaxThreshold - thresholdPadding || angleBtwPlayer > southMinThreshold + thresholdPadding) //Special case
        {
            animator.runtimeAnimatorController = south;
            //south.SetActive(true);
            //spriteRenderer.sprite = south;
            //currentDirection = Vector3.back;
        }
        else if (angleBtwPlayer < southwestMaxThreshold - thresholdPadding && angleBtwPlayer > southwestMinThreshold + thresholdPadding)
        {
            animator.runtimeAnimatorController = southwest;
            //southwest.SetActive(true);
            //spriteRenderer.sprite = southwest;
            //currentDirection = new Vector3(1, 0, -1).normalized;
        }
        else if (angleBtwPlayer < westMaxThreshold - thresholdPadding && angleBtwPlayer > westMinThreshold + thresholdPadding)
        {
            animator.runtimeAnimatorController = west;
            //west.SetActive(true);
            //spriteRenderer.sprite = west;
            //currentDirection = Vector3.right;
        }
        else if (angleBtwPlayer < northwestMaxThreshold - thresholdPadding && angleBtwPlayer > northwestMinThreshold + thresholdPadding)
        {
            animator.runtimeAnimatorController = northwest;
            //northwest.SetActive(true);
            //spriteRenderer.sprite = northwest;
            //currentDirection = new Vector3(1, 0, 1).normalized;
        }

        spriteTransform.transform.LookAt(new Vector3(player.position.x, mainTransform.position.y, player.position.z));
        //spriteRenderer.transform.rotation *= Quaternion.LookRotation(dirToPlayer, Vector3.up);//  * spriteRenderer.transform.rotation;
    }
}
