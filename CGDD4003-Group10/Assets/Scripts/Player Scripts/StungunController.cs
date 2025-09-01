using System;
using UnityEngine;

public class StungunController : MonoBehaviour
{
    [SerializeField] Score score;

    [Header("Weapon Audio")]
    [SerializeField] AudioClip stunShotSFX;
    [SerializeField] AudioClip stunShotEmpty;
    [SerializeField] AudioSource weaponSound;

    [Header("Stun-Gun Settings")]
    [SerializeField] StungunVFX stungunVFX;
    [SerializeField] Animator stungunAnimator;
    Projectile stunProjectile;
    [SerializeField] float timeBtwStunShots;
    [SerializeField] int pelletsPerStunAmmo = 10;
    [SerializeField] Transform bulletOrigin;
    [SerializeField] GameObject stunGun;
    int stunAmmoCount;
    [SerializeField] int maxAmmoCount = 1;
    //The amount of pellets per ammo is set in the score script
    private float stunFireTimer;
    private int pelletCountSinceLastShot;

    public event Action<int> OnStunAmmoChanged;

    // Start is called before the first frame update
    void Start()
    {
        stunAmmoCount = 0;
        stunProjectile = Resources.Load<Projectile>("StunShot");
    }

    /// <summary>
    /// Handles the firing of the stungun, spawning the projectile and initializing it's motion.
    /// </summary>
    public void StungunFire()
    {
        if (stunFireTimer <= 0)
        {
            if (stunAmmoCount > 0)
            {
                Instantiate(stunProjectile, bulletOrigin.position, bulletOrigin.rotation);

                Score.totalStunsFired++;

                if (stunAmmoCount == maxAmmoCount)
                {
                    pelletCountSinceLastShot = Score.pelletsCollected;
                }

                stunAmmoCount--;

                //stungunVFX.Shoot();
                if (stungunAnimator != null)
                {
                    stungunAnimator.SetTrigger("Shoot");
                    Invoke("SetChargeAnimator", 32f / 60);
                }

                stunFireTimer = timeBtwStunShots;
                weaponSound.PlayOneShot(stunShotSFX);

                OnStunAmmoChanged?.Invoke(Mathf.RoundToInt(Mathf.Clamp(stunAmmoCount + ((Score.pelletsCollected - pelletCountSinceLastShot) % pelletsPerStunAmmo) / (float)pelletsPerStunAmmo, 0, maxAmmoCount) * 100));
                SetChargeAnimator();
                //ammoText.text = "" + Mathf.RoundToInt(Mathf.Clamp(stunAmmoCount + ((Score.pelletsCollected - pelletCountSinceLastShot) % pelletsPerStunAmmo) / (float)pelletsPerStunAmmo, 0, maxAmmoCount) * 100) + "%";
            }
            else
            {
                stungunAnimator.SetTrigger("Empty");
                weaponSound.PlayOneShot(stunShotEmpty);
                stunFireTimer = timeBtwStunShots;
            }
        }
        else if (stunFireTimer > 0)
        {
            stunFireTimer -= Time.deltaTime;
        }
    }

    //An animation event to set the charge float to ensure consistency
    public void SetChargeAnimator()
    {
        if (stungunAnimator != null)
        {
            stungunAnimator.SetFloat("Charge", Mathf.Min(3, stunAmmoCount));
        }
    }

    public void CheckToAddStunAmmo()
    {
        if ((Score.pelletsCollected - pelletCountSinceLastShot) > 0 && (Score.pelletsCollected - pelletCountSinceLastShot) % pelletsPerStunAmmo == 0)
        {
            if (stunAmmoCount < maxAmmoCount)
            {
                stunAmmoCount++;
                SetChargeAnimator();
            }
        }
        OnStunAmmoChanged?.Invoke(Mathf.RoundToInt(Mathf.Clamp(stunAmmoCount + ((Score.pelletsCollected - pelletCountSinceLastShot) % pelletsPerStunAmmo) / (float)pelletsPerStunAmmo, 0, maxAmmoCount) * 100));
        //ammoText.text = "" + Mathf.RoundToInt(Mathf.Clamp(stunAmmoCount + ((Score.pelletsCollected - pelletCountSinceLastShot) % pelletsPerStunAmmo) / (float)pelletsPerStunAmmo, 0, maxAmmoCount) * 100) + "%";
    }

    public void HandleGunState(bool gunActivated)
    {
        if(gunActivated)
        {
            stungunAnimator.SetTrigger("Unequip");
        }
        else
        {
            stungunAnimator.SetTrigger("Equip");
        }
    }

    public void HandlePelletPickup()
    {
        CheckToAddStunAmmo();
    }

    private void OnEnable()
    {
        //score.OnPelletPickup += HandlePelletPickup;
    }
    private void OnDisable()
    {
        //score.OnPelletPickup -= HandlePelletPickup;
    }
}
