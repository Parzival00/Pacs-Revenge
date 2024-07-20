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

    bool respawning;
    bool spawning;

    private void Start()
    {
        if (mainTransform == null)
            mainTransform = transform;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

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

        if (!respawning && !spawning)
        {
            if (angleBtwPlayer < northMaxThreshold - thresholdPadding && angleBtwPlayer > northMinThreshold + thresholdPadding)
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
        }

        spriteTransform.transform.LookAt(new Vector3(player.position.x, mainTransform.position.y, player.position.z));
    }

    public void StartDeathAnimation(bool faceForward)
    {
        animator.SetTrigger("Death");

        if(faceForward)
        {
            switch(orientation)
            {
                case Orientation.Southeast:
                    animator.SetBool("TurnEast", true);
                    break;
                case Orientation.Southwest:
                    animator.SetBool("TurnWest", true);
                    break;
                case Orientation.South:
                    if (Random.Range(0, 2) == 1)
                        animator.SetBool("TurnEast", true);
                    else
                        animator.SetBool("TurnWest", true);
                    break;
            }
        } else
        {
            switch (orientation) {
                case Orientation.Northeast:
                    animator.SetBool("TurnEast", true);
                    break;
                case Orientation.Northwest:
                    animator.SetBool("TurnWest", true);
                    break;
                case Orientation.North:
                    if (Random.Range(0, 2) == 1)
                        animator.SetBool("TurnEast", true);
                    else
                        animator.SetBool("TurnWest", true);
                    break;
            }
        }

        animator.SetBool("Respawning", true);

        respawning = true;
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
        StopMovingCorpse();
        animator.SetTrigger("Reform");
    }

    public void EndRespawning()
    {
        animator.SetBool("Respawning", false);
        respawning = false;

        animator.ResetTrigger("Death");
        animator.ResetTrigger("Reform");
        animator.SetBool("TurnWest", false);
        animator.SetBool("TurnEast", false);
        animator.SetBool("MoveCorpse", false);
    }


    public void ResetParameters()
    {
        animator.ResetTrigger("Death");
        animator.ResetTrigger("Reform");
        animator.SetBool("MoveCorpse", false);
        animator.SetBool("TurnWest", false);
        animator.SetBool("TurnEast", false);
        animator.SetBool("Respawning", false);
        respawning = false;

        animator.SetTrigger("Reset");
    }

    public void ActivateColliders()
    {
        collidersActive = true;
    }

    public void DeactivateColliders()
    {
        collidersActive = false;
    }

    public void BossfightSpawnStart()
    {
        spawning = true;
        animator.SetBool("Spawning", true);
    }
    public void BossfightSpawn()
    {
        animator.SetTrigger("BossSpawn");
        animator.SetBool("Spawning", false);
    }
    public void BossfightSpawnEnd()
    {
        spawning = false;
    }

    //Debug Function
    public void PrintParameters()
    {
        print("MoveCorpse: " + animator.GetBool("MoveCorpse"));
    }
}
