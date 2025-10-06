using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class LauncherProjectile : MonoBehaviour
{
    Animator animator;
    [Header("Projectile Settings")]
    [SerializeField] float projectileSpeed = 2f;
    [SerializeField] float activationDelay = 2f;
    [SerializeField] float blastRadius = 1f;
    [SerializeField] LayerMask explodeMask;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip explosionClip;

    Collider collider;

    WeaponInfo weaponInfo;
    Launcher launcher;

    bool moving;

    bool activated = false;

    // Start is called before the first frame update
    void Start()
    {
        moving = true;

        animator = GetComponent<Animator>();
        collider = GetComponent<Collider>();
    }

    public void Initialize(WeaponInfo weaponInfo, Launcher launcher)
    {
        this.weaponInfo = weaponInfo;
        this.launcher = launcher;
    }

    void Activate()
    {
        activated = true;
        animator.SetTrigger("Activate");
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
            Move();
    }

    /// <summary>
    /// Applies force and moves the shot
    /// </summary>
    public void Move()
    {
        transform.Translate(transform.forward * projectileSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wall" || other.tag == "Corner" || other.tag == "T Wall" || other.tag == "Straight" || other.tag == "Floor")
        {
            moving = false;

            Invoke("Activate", activationDelay);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!activated)
        {
            return;
        }

        if(other.tag == "Enemy")
        {
            activated = false;

            StartCoroutine(ExplodeRoutine());
        }
    }

    public void ForceExplosion()
    {
        if(!activated)
        {
            Activate();
        }

        activated = false;

        StartCoroutine(ExplodeRoutine());
    }

    IEnumerator ExplodeRoutine()
    {
        if(audioSource && explosionClip)
        {
            audioSource.PlayOneShot(explosionClip);
        }
        animator.SetTrigger("Explode");

        launcher.ProjectileExploded(this);

        yield return new WaitForSeconds(0.25f);

        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius, explodeMask);
        for (int i = 0; i < colliders.Length; i++) 
        {
            Ghost ghost = colliders[i].GetComponent<Ghost>();
            BossCollider bossCollider = colliders[i].GetComponent<BossCollider>();
            CaptureTentacle captureTentacle = colliders[i].GetComponent<CaptureTentacle>();
            Barrel barrel = colliders[i].GetComponent<Barrel>();

            if (ghost != null && captureTentacle == null)
            {
                Ghost.HitInformation hitInformation = ghost.GotHit(ghost.GetInstakillTargetAreaType(), weaponInfo.damageMultiplier);
                Score.AddToScore(Color.gray, (int)((hitInformation.pointWorth + hitInformation.targetArea.pointsAddition) * (weaponInfo.scoreMultiplier)));

                ghost.SpawnBlood(10);
            }
            else if (bossCollider != null)
            {
                Vector3 nearestPoint = colliders[i].ClosestPoint(transform.position);
                Boss.BossHitInformation hitInformation = bossCollider.boss.GotHit(nearestPoint, bossCollider.HeadID, weaponInfo.damageMultiplier);
                if (hitInformation.pointWorth > 0)
                    Score.AddToScore(Color.gray, (int)(hitInformation.pointWorth * (weaponInfo.scoreMultiplier)));
            }
            else if (captureTentacle != null)
            {
                captureTentacle.TakeDamage(50);
            }
            else if (barrel != null && (barrel.gameObject.tag == "ExplosiveBarrel" || barrel.gameObject.tag == "ShockBarrel"))
            {
                barrel.StartExplosion();
            }
        }

        Destroy(gameObject, 2f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }
}
