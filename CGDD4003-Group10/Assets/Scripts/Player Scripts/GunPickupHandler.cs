using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickupHandler : MonoBehaviour
{
    [SerializeField] PlayerCombat playerCombat;
    [SerializeField] PlayerStateManager playerState;
    [SerializeField] CorruptedGunController corruptedGunController;

    [Header("Weapon Settings")]
    [SerializeField] float gunTimeAmount = 5f;
    [SerializeField] string railgunAlertMessage = "Advanced Targeting System Activated";
    [SerializeField] float railgunAlertLength = 2;

    //[Header("Stun-Gun Settings")]
    //[SerializeField] Animator stungunAnimator;

    //bool lostShield;

    private float gunTimer;

    public float GunTimer01 { get => (gunTimer / gunTimeAmount); }

    //shield settings
   // int shieldsRemaining;

    //[Header("GameObject Refereneces")]
    //[SerializeField] GameObject gun;
    //[SerializeField] GameObject hud;
    //[SerializeField] ShieldEffectAnimator shieldAnimator;
    //[SerializeField] Animator deathAnimator;
    //[SerializeField] TransitionEffect transitionEffect;
    //[SerializeField] TutorialController tutorial;
    //bool doTutorials;

    public FaceController faceController { get; private set; }

    Ghost[] ghosts;

    //[Header("Player Animator")]
    //[SerializeField] Animator animator;

    [Header("Music Settings")]
    [SerializeField] float powerMusicVolBoost;
    [SerializeField] AudioClip powerMusic;
    [SerializeField] AudioClip gameStart;
    [SerializeField] AudioClip bossFightMusic;
    [SerializeField] AudioSource musicPlayer;

    //private bool speedBoostActivated = false;

    //private float originalY;

    bool inDeathSequence;
    bool gunActivated;

    HUDMessenger hudMessenger;

    //int permenantGhostsKilled = 0;

    //bool invisibilityActivated;

    public void OnTriggerStay(Collider other)
    {
        if (Score.wonLevel)
            return;

        if (other.tag == "Weapon" && !gunActivated)
        {
            WeaponPickup weaponPickup = other.GetComponent<WeaponPickup>();
            if (weaponPickup != null && weaponPickup.isCorrupted)
            {
                //corruptedGunController.corruptedGun.ActivateEntrapment(this);
            }
            Destroy(other.gameObject);
            gunActivateCoroutine = StartCoroutine(ActivateGun());
        }
    }

    Coroutine gunTimerCoroutine;
    Coroutine gunActivateCoroutine;
    /// <summary>
    /// Activates the gun and any related visuals and start the gun timer coroutine
    /// </summary>
    IEnumerator ActivateGun()
    {
        if (gunTimerCoroutine != null)
        {
            StopCoroutine(gunTimerCoroutine);
        }

        playerState.SetGunActivated(true);
        gunActivated = true;

        /*if (stungunAnimator != null)
        {
            //stungunAnimator.SetTrigger("Unequip");

            //yield return new WaitForSeconds(0.3f);
            //stunGun.SetActive(false);
        }
        else
        {
            //stunGun.SetActive(false);
        }*/

        yield return new WaitForSeconds(0.3f);

        if (Score.bossEnding)
        {
            WeaponSpawner ws = FindObjectOfType<WeaponSpawner>();
            ws.Reset();
        }
        else
        {
            musicPlayer.Stop();
            musicPlayer.PlayOneShot(powerMusic);
        }

        //gunActivated = true;
        //currentWeapon.gameObject.SetActive(true);
        //yield return null;
        //currentWeapon.ResetWeapon();

        //Deactivate stun gun
        //stunGun.SetActive(false);

        if (faceController)
        {
            faceController.RailgunPickup();
        }

        if (hudMessenger)
        {
            //hudMessenger.Display(railgunAlertMessage, railgunAlertLength);
        }

        //playerCombat.CurrentWeapon.GunAnimator.ResetTrigger("Unequip");
        //playerCombat.CurrentWeapon.GunAnimator.SetTrigger("Equip");

        /*if (invisibilityActivated)
        {
            //currentWeapon.OnInvisibilityStart();
        }
        else
        {
            //currentWeapon.OnInvisibilityEnd();
        }*/

        foreach (Ghost ghost in ghosts)
        {
            ghost.InitiateScatter();
        }
        gunTimerCoroutine = StartCoroutine(GunTimer());

        //AddShields();
        //lostShield = false;

        yield return new WaitForSeconds(0.5f);

        gunActivateCoroutine = null;

        /*if (doTutorials && gunActivated && Score.totalShotsFired <= 0 && tutorial)
        {
            //tutorial.ToggleShootPrompt(true);
        }*/
    }

    /// <summary>
    /// Waits a certain amount before deactivating the gun
    /// </summary>
    IEnumerator GunTimer()
    {
        gunTimer = gunTimeAmount;

        while (gunTimer >= 0)
        {
            gunTimer -= Time.deltaTime;
            playerCombat.CurrentWeapon.OnTimerEvent(gunTimer / gunTimeAmount);
            yield return null;
        }

        StartCoroutine(DeactivateGun());
        gunTimer = gunTimeAmount;
    }

    /// <summary>
    /// Deactivates the gun and any related visuals
    /// </summary>
    public IEnumerator DeactivateGun()
    {

        if (!Score.bossEnding)
        {
            musicPlayer.Stop();
        }

        playerState.SetGunActivated(false);

        //musicPlayer.volume = musicPlayer.volume / powerMusicVolBoost;

        //Activates Stun-Gun again
        //stunGun.SetActive(true);

        if (gunActivateCoroutine != null)
        {
            StopCoroutine(gunActivateCoroutine);
        }

        if (gunTimerCoroutine != null)
        {
            StopCoroutine(gunTimerCoroutine);
        }

        //currentWeapon.GunAnimator.ResetTrigger("Equip");
        //currentWeapon.GunAnimator.SetTrigger("Unequip");

        yield return new WaitForSeconds(0.4f);

        //currentWeapon.gameObject.SetActive(false);

        foreach (Ghost ghost in ghosts)
        {
            ghost.DeactivateScatter();
        }

        gunTimerCoroutine = null;

        gunActivated = false;

        //Deactivate any remaining target outline
        //targetOutlineController.DeactivateOutline();

        /*if (shieldsRemaining > 0 && !lostShield)
        {
            shieldsRemaining--;
            print("shields: " + shieldsRemaining);
            if (shieldAnimator != null && shieldsRemaining == 0)
            {
                shieldAnimator.PlayShieldDown();
            }
        }*/

        /*if (!inDeathSequence && stungunAnimator != null)
        {
            //stunGun.SetActive(true);
            //stungunAnimator.SetTrigger("Equip");
            //stungunAnimator.SetFloat("Charge", Mathf.Min(3, stunAmmoCount));
            //yield return new WaitForSeconds(0.3f);
        }
        else
        {
            //stunGun.SetActive(true);
        }*/

        //gunActivated = false;
    }

    void HandleOnDeath()
    {
        inDeathSequence = true;
        playerState.SetGunActivated(false);
    }
    void HandleOnRespawn()
    {
        inDeathSequence = false;
    }

    private void OnEnable()
    {
        playerState.OnDeath += HandleOnDeath;
        inDeathSequence = false;
        playerState.OnRespawn += HandleOnRespawn;
    }
    private void OnDisable()
    {
        playerState.OnDeath -= HandleOnDeath;
        playerState.OnRespawn -= HandleOnRespawn;
    }
}
