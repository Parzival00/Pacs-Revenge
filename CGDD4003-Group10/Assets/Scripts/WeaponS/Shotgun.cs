using UnityEngine;

public class Shotgun : Weapon
{
    [Header("Shotgun Settings")]
    [SerializeField] int numOfShots = 8;
    [SerializeField] float shotSpread = 0.08f;
    [SerializeField] float cooldownTime = 1.5f;
    [SerializeField] float weaponRange = 5f;
    [SerializeField] GameObject wallHitEffect;
    [SerializeField] Camera fpsCam;
    [SerializeField] CameraShake camShake;
    [SerializeField] float camShakeFrequency = 1f;
    [SerializeField] float camShakeDuration = 0.4f;
    [SerializeField] LayerMask targetingMask;
    [SerializeField] Animator shotgunAnimator;

    float cooldownTimer = 0;

    public override void OnMouseDownEvent()
    {
        if (cooldownTimer > cooldownTime)
        {
            if (camShake)
            {
                camShake.ShakeCamera(camShakeFrequency, 0.5f, camShakeDuration);
            }

            gunAnimator.SetTrigger("Shoot");
            gunAnimator.SetFloat("Speed", 1.5f / cooldownTime);

            //Insert audio here
            weaponSound.PlayOneShot(gunshotSFX);

            for (int i = 0; i < numOfShots; i++)
            {
                Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

                Vector2 spread = Random.insideUnitCircle * shotSpread;
                Vector3 rayEnd = rayOrigin + transform.forward * 0.1f + transform.right * spread.x + transform.up * spread.y;

                Vector3 rayDir = (rayEnd - rayOrigin).normalized;
                RaycastHit hit;
                //Detect hit on enemy
                if (Physics.Raycast(rayOrigin, rayDir, out hit, weaponRange, targetingMask))
                {
                    //print("Hit: " + hit.collider.gameObject.name);

                    TargetAreaCollider targetAreaCollider = hit.collider.GetComponent<TargetAreaCollider>();
                    BossCollider bossCollider = hit.collider.GetComponent<BossCollider>();
                    CaptureTentacle captureTentacle = hit.collider.GetComponent<CaptureTentacle>();
                    Barrel barrel = hit.collider.GetComponent<Barrel>();

                    if (targetAreaCollider != null && captureTentacle == null)
                    {
                        Ghost.HitInformation hitInformation = targetAreaCollider.OnShot(weaponInfo.damageMultiplier / numOfShots, weaponInfo.scoreMultiplier / numOfShots);
                        Score.AddToScore(Color.gray, (int)((hitInformation.pointWorth + hitInformation.targetArea.pointsAddition) * (weaponInfo.scoreMultiplier / numOfShots)));

                        SpawnBlood(hitInformation.bigBlood, hitInformation.smallBlood, hitInformation.targetArea.difficulty, hit);
                    }
                    else if (bossCollider != null)
                    {
                        Boss.BossHitInformation hitInformation = bossCollider.boss.GotHit(hit.point, bossCollider.HeadID, weaponInfo.damageMultiplier / numOfShots, weaponInfo.scoreMultiplier / numOfShots);
                        if (hitInformation.pointWorth > 0)
                            Score.AddToScore(Color.gray, (int)(hitInformation.pointWorth * (weaponInfo.scoreMultiplier / numOfShots)));
                    }
                    else if (captureTentacle != null)
                    {
                        captureTentacle.TakeDamage(50);
                    }
                    else if (barrel != null && (barrel.gameObject.tag == "ExplosiveBarrel" || barrel.gameObject.tag == "ShockBarrel"))
                    {
                        barrel.StartExplosion();
                    }
                    else
                    {
                        Instantiate(wallHitEffect, hit.point + hit.normal * 0.1f, Quaternion.identity);
                    }
                }
            }

            Score.totalShotsFired++;

            cooldownTimer = 0;
        }
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
    }

    public override void ResetWeapon()
    {
        cooldownTimer = 0;
    }

    public override void OnTimerEvent(float progress)
    {
        shotgunAnimator.SetFloat("Charge", progress);
    }

    private void OnDrawGizmosSelected()
    {
        if (fpsCam)
        {
            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

            Vector2 spread = Vector2.one * shotSpread;
            Gizmos.DrawLine(rayOrigin, rayOrigin + transform.forward * 0.1f);
            Gizmos.DrawLine(rayOrigin, rayOrigin + transform.forward * 0.1f + transform.right * spread.x);
            Gizmos.DrawLine(rayOrigin, rayOrigin + transform.forward * 0.1f - transform.right * spread.x);
            Gizmos.DrawLine(rayOrigin, rayOrigin + transform.forward * 0.1f + transform.up * spread.y);
            Gizmos.DrawLine(rayOrigin, rayOrigin + transform.forward * 0.1f - transform.up * spread.y);
        }
    }
}
