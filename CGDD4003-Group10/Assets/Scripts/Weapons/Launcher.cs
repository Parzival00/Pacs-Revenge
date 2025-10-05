using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : Weapon
{
    [Header("Launcher Settings")]
    [SerializeField] Transform projectileSpawnPoint;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] int maxAmmo = 5;
    [SerializeField] float cooldownTime = 1.5f;
    [SerializeField] float explodingTime = 3f;
    [SerializeField] GameObject wallHitEffect;
    [SerializeField] Camera fpsCam;
    [SerializeField] CameraShake camShake;
    [SerializeField] float camShakeFrequency = 1f;
    [SerializeField] float camShakeDuration = 0.4f;
    [SerializeField] LayerMask targetingMask;

    List<LauncherProjectile> projectiles = new List<LauncherProjectile>();

    float cooldownTimer = 0;
    float explodingTimer = 0;

    int ammoCount;

    public override void OnMouseDownEvent()
    {
        if (cooldownTimer > cooldownTime && explodingTimer > explodingTime)
        {
            if (camShake)
            {
                camShake.ShakeCamera(camShakeFrequency, 0.5f, camShakeDuration);
            }


            if (ammoCount > 0)
            {
                ammoCount--;

                //Insert audio here
                weaponSound.PlayOneShot(gunshotSFX);

                gunAnimator.SetTrigger("Shoot");

                StartCoroutine(ShootRoutine());

                cooldownTimer = 0;
            }
            else
            {
                explodingTimer = 0;
                StartCoroutine(ExplodeRoutine());

                gunAnimator.SetTrigger("Exploding");
            }
        }
    }

    IEnumerator ShootRoutine()
    {
        yield return new WaitForSeconds(0.4f);

        //gunAnimator.SetFloat("Speed", 1.5f / cooldownTime);


        GameObject projObj = Instantiate(projectilePrefab, projectileSpawnPoint.position, transform.rotation);
        LauncherProjectile proj = projObj.GetComponent<LauncherProjectile>();

        if (proj != null)
        {
            proj.Initialize(weaponInfo, this);
            projectiles.Add(proj);
            gunAnimator.SetFloat("Ammo", ammoCount);
        }
        else
        {
            Destroy(projObj);
        }
    }

    public void ProjectileExploded(LauncherProjectile projectile)
    {
        projectiles.Remove(projectile);
    }

    IEnumerator ExplodeRoutine()
    {
        while(projectiles.Count > 0)
        {
            LauncherProjectile proj = projectiles[0];
            proj.ForceExplosion();
            projectiles.Remove(proj);
            yield return new WaitForSeconds(0.5f);
        }

        ammoCount = maxAmmo;
        gunAnimator.SetFloat("Ammo", ammoCount);

        explodingTimer = explodingTime;
    }

    public override void OnMouseEvent()
    {
    }

    public override void OnMouseUpEvent()
    {
    }

    public override void OnNoMouseEvent()
    {
    }

    public override void OnPassiveEvent()
    {
        cooldownTimer += Time.deltaTime;
        explodingTimer += Time.deltaTime;
    }

    public override void ResetWeapon()
    {
        cooldownTimer = 0;
        explodingTimer = explodingTime;

        ammoCount = maxAmmo;
        gunAnimator.SetFloat("Ammo", ammoCount);
    }

    public override void OnTimerEvent(float progress)
    {
        gunAnimator.SetFloat("Charge", 1 - progress);
    }

    private void OnDrawGizmosSelected()
    {
        if (fpsCam)
        {
            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        }
    }
}
