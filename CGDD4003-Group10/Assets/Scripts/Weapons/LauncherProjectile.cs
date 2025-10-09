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
    [SerializeField] AudioSource explosionSource;
    [SerializeField] AudioSource activateSource;
    [SerializeField] AudioClip explosionClip;
    [SerializeField] AudioClip activateClip;

    Vector3 launchDir;

    Collider collider;

    WeaponInfo weaponInfo;
    Launcher launcher;

    bool moving;

    bool activated = false;
    bool oneTimeActivation = false;

    bool exploding = false;

    // Start is called before the first frame update
    void Start()
    {
        moving = true;

        animator = GetComponent<Animator>();
        collider = GetComponent<Collider>();
    }

    public void Initialize(WeaponInfo weaponInfo, Launcher launcher, Vector3 launchDir)
    {
        this.weaponInfo = weaponInfo;
        this.launcher = launcher;
        this.launchDir = launchDir.normalized;
    }

    void Activate()
    {
        activateSource.PlayOneShot(activateClip);
        activated = true;
        animator.SetTrigger("Activate");
        oneTimeActivation = true;
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
        transform.Translate(launchDir * projectileSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wall" || other.tag == "Corner" || other.tag == "T Wall" || other.tag == "Straight" || other.tag == "Floor" || other.tag == "Boss" || other.tag == "Enemy")
        {
            moving = false;

            if (!oneTimeActivation)
            {
                Invoke("Activate", activationDelay);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!activated)
        {
            return;
        }

        //print($"{other.name} overlapping with Projectile");

        if(other.tag == "Enemy" || other.tag == "Boss")
        {
            if (exploding)
            {
                return;
            }

            activated = false;

            StartCoroutine(ExplodeRoutine());
        }
    }

    public void ForceExplosion()
    {
        if(exploding)
        {
            return;
        }

        if(!activated)
        {
            Activate();
        }

        activated = false;

        StartCoroutine(ExplodeRoutine());
    }

    IEnumerator ExplodeRoutine()
    {
        collider.enabled = false;
        exploding = true;


        animator.SetTrigger("Explode");

        launcher.ProjectileExploded(this);


        yield return new WaitForSeconds(0.4f);

        if (explosionSource && explosionClip)
        {
            explosionSource.PlayOneShot(explosionClip);
        }

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

        collider.enabled = false;

        Destroy(gameObject, 4f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }
}
