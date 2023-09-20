using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostSpriteController : MonoBehaviour
{
    public enum Orientation
    {
        North,
        Northeast,
        East,
        Southeast,
        South,
        Southwest,
        West,
        Northwest
    }

    public Orientation orientation { get; private set; }

    private bool collidersActive = true;

    [Header("References")]
    [SerializeField] Transform player;
    [SerializeField] Transform mainTransform;
    [SerializeField] Transform spriteTransform;
    [SerializeField] Animator animator;

    [Header("Sprite Animator Controllers")]
    [SerializeField] RuntimeAnimatorController north;
    [SerializeField] AnimatorOverrideController northeast;
    [SerializeField] AnimatorOverrideController east;
    [SerializeField] AnimatorOverrideController southeast;
    [SerializeField] AnimatorOverrideController south;
    [SerializeField] AnimatorOverrideController southwest;
    [SerializeField] AnimatorOverrideController west;
    [SerializeField] AnimatorOverrideController northwest;

    [Header("Collider Parent GameObjects")]
    [SerializeField] GameObject northColliders;
    [SerializeField] GameObject northeastColliders;
    [SerializeField] GameObject eastColliders;
    [SerializeField] GameObject southeastColliders;
    [SerializeField] GameObject southColliders;
    [SerializeField] GameObject southwestColliders;
    [SerializeField] GameObject westColliders;
    [SerializeField] GameObject northwestColliders;

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

    private void Start()
    {
        if (mainTransform == null)
            mainTransform = transform;

        animator.runtimeAnimatorController = north;
        orientation = Orientation.North;

        ActivateColliders();
    }

    // Update is called once per frame
    void Update()
    {
        northColliders.SetActive(false);
        northeastColliders.SetActive(false);
        eastColliders.SetActive(false);
        southeastColliders.SetActive(false);
        southColliders.SetActive(false);
        southwestColliders.SetActive(false);
        westColliders.SetActive(false);
        northwestColliders.SetActive(false);

        Vector3 forward = mainTransform.forward;
        Vector3 dirToPlayer = (new Vector3(player.position.x, 0, player.position.z) - new Vector3(mainTransform.position.x, 0, mainTransform.position.z)).normalized; //to - from
        float angleBtwPlayer = Vector3.SignedAngle(forward, dirToPlayer, mainTransform.up);

        if(angleBtwPlayer < northMaxThreshold - thresholdPadding && angleBtwPlayer > northMinThreshold + thresholdPadding)
        {
            animator.runtimeAnimatorController = north;
            orientation = Orientation.North;

            if (collidersActive)
                northColliders.SetActive(true);
        }
        else if (angleBtwPlayer < northeastMaxThreshold - thresholdPadding && angleBtwPlayer > northeastMinThreshold + thresholdPadding)
        {
            animator.runtimeAnimatorController = northeast;
            orientation = Orientation.Northeast;

            if (collidersActive)
                northeastColliders.SetActive(true);
        }
        else if (angleBtwPlayer < eastMaxThreshold - thresholdPadding && angleBtwPlayer > eastMinThreshold + thresholdPadding)
        {
            animator.runtimeAnimatorController = east;
            orientation = Orientation.East;

            if (collidersActive)
                eastColliders.SetActive(true);
        }
        else if (angleBtwPlayer < southeastMaxThreshold - thresholdPadding && angleBtwPlayer > southeastMinThreshold + thresholdPadding)
        {
            animator.runtimeAnimatorController = southeast;
            orientation = Orientation.Southeast;

            if (collidersActive)
                southeastColliders.SetActive(true);
        }
        else if (angleBtwPlayer < southMaxThreshold - thresholdPadding || angleBtwPlayer > southMinThreshold + thresholdPadding) //Special case
        {
            animator.runtimeAnimatorController = south;
            orientation = Orientation.South;

            if (collidersActive)
                southColliders.SetActive(true);
        }
        else if (angleBtwPlayer < southwestMaxThreshold - thresholdPadding && angleBtwPlayer > southwestMinThreshold + thresholdPadding)
        {
            animator.runtimeAnimatorController = southwest;
            orientation = Orientation.Southwest;

            if (collidersActive)
                southwestColliders.SetActive(true);
        }
        else if (angleBtwPlayer < westMaxThreshold - thresholdPadding && angleBtwPlayer > westMinThreshold + thresholdPadding)
        {
            animator.runtimeAnimatorController = west;
            orientation = Orientation.West;

            if (collidersActive)
                westColliders.SetActive(true);
        }
        else if (angleBtwPlayer < northwestMaxThreshold - thresholdPadding && angleBtwPlayer > northwestMinThreshold + thresholdPadding)
        {
            animator.runtimeAnimatorController = northwest;
            orientation = Orientation.Northwest;

            if (collidersActive)
                northwestColliders.SetActive(true);
        }

        spriteTransform.transform.LookAt(new Vector3(player.position.x, mainTransform.position.y, player.position.z));
    }

    public void StartDeathAnimation()
    {
        animator.SetTrigger("Death");
        animator.ResetTrigger("Reform");
    }

    public void StartMovingCorpse()
    {
        animator.SetBool("MoveCorpse", true);
    }

    public void StopMovingCorpse()
    {
        animator.SetBool("MoveCorpse", false);
    }

    public void StartReformAnimation()
    {
        animator.SetTrigger("Reform");
        animator.ResetTrigger("Death");
    }

    public void ActivateColliders()
    {
        collidersActive = true;
    }

    public void DeactivateColliders()
    {
        collidersActive = false;
    }
}
