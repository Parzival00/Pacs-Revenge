using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.GraphicsBuffer;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerController : MonoBehaviour
{
    public Transform playerCam;
    public Transform playerT;

    MainMenuManager mainMenuManager;

    public bool canMove { get; private set; }

    [Header("Movement Settings")]
    [SerializeField] float baseSpeed;
    [SerializeField] float sprintMultiplier;
    [SerializeField] float gunSpeedMultiplier;
    [SerializeField] Transform playerSpawnPoint;
    private float speed;
    private float moveSmoothTime = 0.1f;
    private Vector2 currentDirection;
    private Vector2 currentVelocity;
    private CharacterController character;
    [HideInInspector]
    public Vector3 velocity = Vector3.zero; //used for view bobbing as well

    float cameraPitch;
    [Header("Mouse Settings")]
    [SerializeField] bool paused;
    [SerializeField] float sensitivity;

    [Header("Weapon Audio")]
    [SerializeField] AudioClip gunshotSFX;
    [SerializeField] AudioClip stunShotSFX;
    [SerializeField] AudioClip overheatSFX;
    [SerializeField] AudioClip chargeupSFX;
    [SerializeField] AudioClip chargeReadySFX;
    [SerializeField] AudioSource weaponSound;
    [SerializeField] AudioSource weaponChargeSound;
    [SerializeField] AudioSource weaponChargeReadySound;

    [Header("Railgun Settings")]
    [SerializeField] string railgunAlertMessage = "Advanced Targeting System Activated";
    [SerializeField] float railgunAlertLength = 2;
    [SerializeField] RailgunVFX railGunVFX;
    [SerializeField] GameObject wallHitEffect;
    [SerializeField] Animator railgunAnimator;
    [SerializeField] float chargeTime;
    [SerializeField] float maxWeaponTemp = 5;
    [SerializeField] float overheatSpeed = 1;
    [SerializeField] float cooldownSpeed = 1;
    [SerializeField] float dechargeTime = .25f;
    [SerializeField] float gunTimeAmount = 5f;
    [SerializeField] Camera fpsCam;
    [SerializeField] WaitForSeconds shotDuration = new WaitForSeconds(0.07f);
    [SerializeField] float camShakeFrequency = 1f;
    [SerializeField] float camShakeDuration = 0.4f;
    [SerializeField] CameraShake camShake;
    [SerializeField] float weaponRange;
    [SerializeField] LayerMask targetingMask;

    public CameraShake CamShake { get => camShake; }

    [Header("Stun-Gun Settings")]
    [SerializeField] StungunVFX stungunVFX;
    [SerializeField] Animator stungunAnimator;
    [SerializeField] float timeBtwStunShots;
    [SerializeField] int pelletsPerStunAmmo = 10;
    [SerializeField] Transform bulletOrigin;
    [SerializeField] GameObject stunGun;
    int ammoCount;
    [SerializeField] int maxAmmoCount = 1;
    //The amount of pellets per ammo is set in the score script
    private float stunFireTimer;
    private int pelletCountSinceLastShot;

    private float weaponCharge;
    private float weaponDecharge;
    private float weaponTemp;
    private bool overheated = false;
    private bool weaponDecharging = false;

    bool lostShield;

    private float gunTimer;

    public float WeaponCharge { get => weaponCharge; }
    public float WeaponDecharge { get => weaponDecharge; }
    public float WeaponTemp01 { get => (weaponTemp / maxWeaponTemp); }
    public bool Overheated { get => overheated; }
    public bool WeaponDecharging { get => weaponDecharging; }
    public float GunTimer01 { get => (gunTimer / gunTimeAmount); }

    public bool StunGunCanFire { get; private set; }
    public int StunAmmoCount { get => ammoCount; }
    public int StunAmmoPerPellets { get => pelletsPerStunAmmo; }
    public bool StunGunAmmoEmpty { get => ammoCount <= 0; }

    //shield settings
    int shieldsRemaining;

    [Header("GameObject Refereneces")]
    [SerializeField] GameObject gun;
    [SerializeField] GameObject hud;
    [SerializeField] TMP_Text LivesText;
    [SerializeField] TMP_Text ammoText;
    [SerializeField] TMP_Text shieldText;
    [SerializeField] ShieldEffectAnimator shieldAnimator;
    [SerializeField] Animator deathAnimator;
    [SerializeField] TutorialController tutorial;
    bool doTutorials;

    public FaceController faceController { get; private set; }

    Ghost[] ghosts;

    [Header("Player Settings")]
    [SerializeField] float deathSequenceLength = 1;
    [SerializeField] int defaultPlayerLives = 3;

    [Header("Player Animator")]
    [SerializeField] Animator animator;

    [Header("Target Outline Controller")]
    [SerializeField] TargetOutlineController targetOutlineController;
    [SerializeField] Image crosshair;
    [SerializeField] Color targetingColor = Color.yellow;
    private Color tempColorSave = Color.grey;

    [Header("Invisibility Settings")]
    [SerializeField] float invisibilityLength = 30;
    [SerializeField] AudioSource invisibilitySource;
    [SerializeField] AudioClip invisibilityActivate;
    [SerializeField] AudioClip invisibilityDeactivate;

    [Header("Speed PowerUp Settings")]
    [SerializeField] float speedPowerUpLength = 15;
    [SerializeField] float speedIncrease = 1.5f;
    [SerializeField] AudioSource speedPowerUpSource;
    [SerializeField] AudioClip speedActivate;
    [SerializeField] AudioClip speedDeactivate;

    [Header("Extra Life Settings")]
    [SerializeField] Image extraLifeFlash;
    [SerializeField] float flashSpeed = 0.5f;
    [SerializeField] AudioSource extraLifeSource;
    [SerializeField] AudioClip extraLifeSound;

    [Header("Music Settings")]
    [SerializeField] float powerMusicVolBoost;
    [SerializeField] AudioClip powerMusic;
    [SerializeField] AudioClip gameStart;
    [SerializeField] AudioSource musicPlayer;

    [Header("Insanity Ending Settings")]
    [SerializeField] PlayerCorruptionEffect corruptionEffect;
    [SerializeField] AudioSource eatSoundSource;
    [SerializeField] AudioClip eatSound;

    [Header("Debug Settings")]
    [SerializeField] bool usePlayerPrefsSettings;

    public static bool gunActivated { get; private set; }
    public static bool invisibilityActivated { get; private set; }
    public static int playerLives { get; private set; }

    private bool speedBoostActivated = false;

    private bool canFire = true;

    private bool inDeathSequence;

    private float originalY;

    Projectile stunProjectile;

    HUDMessenger hudMessenger;

    int permenantGhostsKilled = 0;

    bool canBeDamaged = true;

    // Start is called before the first frame update
    void Start()
    {
        AudioListener.volume = 1;

        if (!Score.insanityEnding)
        {
            Init();
        }
        else
        {
            permenantGhostsKilled = 0;

            originalY = transform.position.y;

            character = this.GetComponent<CharacterController>();

            canFire = false;
            speed = baseSpeed;

            canMove = true;

            if (corruptionEffect == null)
                corruptionEffect = FindObjectOfType<PlayerCorruptionEffect>();

            mainMenuManager = FindObjectOfType<MainMenuManager>();

            hudMessenger = FindObjectOfType<HUDMessenger>();

            if (usePlayerPrefsSettings)
            {
                ApplyGameSettings();
            }
            else AudioListener.volume = 50;
        }
    }

    private void Init()
    {
        originalY = transform.position.y;

        character = this.GetComponent<CharacterController>();

        mainMenuManager = FindObjectOfType<MainMenuManager>();

        hudMessenger = FindObjectOfType<HUDMessenger>();

        if (extraLifeFlash == null)
            extraLifeFlash = GameObject.FindGameObjectWithTag("ExtraLifeFlash")?.GetComponent<Image>();

        if (extraLifeFlash != null)
            extraLifeFlash.gameObject.SetActive(false);

        speed = baseSpeed;

        stunProjectile = Resources.Load<Projectile>("StunShot");

        faceController = FindObjectOfType<FaceController>();

        if (hud == null)
            hud = GameObject.FindGameObjectWithTag("HUD");

        if (crosshair == null)
            crosshair = GameObject.FindGameObjectWithTag("Crosshair").GetComponent<Image>();

        gunActivated = false;
        gun.SetActive(false);
        hud.SetActive(false);

        if (animator == null)
            animator = GetComponent<Animator>();

        if (deathAnimator == null)
            deathAnimator = GameObject.FindGameObjectWithTag("DeathAnimator").GetComponent<Animator>();

        if (stungunAnimator != null)
        {
            stungunAnimator.SetBool("Equipped", true);
        }
        else
        {
            stunGun.SetActive(true);
        }

        if (shieldAnimator == null)
            shieldAnimator = FindObjectOfType<ShieldEffectAnimator>();

        ghosts = new Ghost[4];
        ghosts = FindObjectsOfType<Ghost>();

        //if (usePlayerPrefsSettings)
        //{
        ApplyGameSettings();
        //}
        //else AudioListener.volume = 50;

        canMove = true;
        canFire = true;

        if (Score.currentLevel == 1)
        {
            playerLives = defaultPlayerLives;
        } else
        {
            playerLives = PlayerPrefs.GetInt("Lives");
            switch (Score.difficulty)
            {
                case 0:
                    if (playerLives < defaultPlayerLives)
                    {
                        playerLives = defaultPlayerLives;
                    }
                    break;
                case 1:
                    if (playerLives < defaultPlayerLives)
                    {
                        playerLives++;
                    }
                    break;
                case 2:
                    break;
            }
        }

        shieldsRemaining = 0;

        invisibilityActivated = false;

        musicPlayer.PlayOneShot(gameStart);

        LivesText.text = "" + playerLives;
    }

    // Update is called once per frame
    void Update()
    {
        if (!MainMenuManager.isGamePaused)
        {
            if (canMove)
            {
                MouseControl();
                MovementControl();
            }

            if (canFire)
            {
                if (gunActivated)
                {
                    RailgunFire();
                    OutlineTargetEnemy();
                    CheckWeaponTemp();
                }
                else
                {
                    StungunFire();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }

        if (!musicPlayer.isPlaying)
        {
            musicPlayer.Play();
            musicPlayer.loop = true;
        }

        if (!Score.insanityEnding)
            DisplayPLayerShields();
    }

    #region Move and Look
    /// <summary>
    /// This method takes the mouse position and rotates the player and camera accordingly. Using the x position of the mouse for horizontal and the y position for vertical.
    /// </summary>
    void MouseControl()
    {
        Vector2 mousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        cameraPitch -= mousePosition.y * sensitivity * Mathf.Min(0.01f, Time.deltaTime) * 30f * Time.timeScale;
        cameraPitch = Mathf.Clamp(cameraPitch, -60.0f, 60.0f);

        playerCam.localEulerAngles = Vector3.right * cameraPitch;
        playerT.Rotate(Vector3.up * mousePosition.x * sensitivity * Mathf.Min(0.01f, Time.deltaTime) * 30f * Time.timeScale);

    }

    /// <summary>
    /// Uses the input axis system to move the player's character controller
    /// Has a sprint ability activated by the L-shift key
    /// </summary>
    void MovementControl()
    {
        Vector2 targetDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDirection.Normalize();
        currentDirection = Vector2.SmoothDamp(currentDirection, targetDirection, ref currentVelocity, moveSmoothTime);

        speed = baseSpeed;

        velocity = (playerT.forward * currentDirection.y + playerT.right * currentDirection.x) * speed;
        character.enabled = true;
        character.Move(velocity * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, originalY, transform.position.z); //Ensure character stays on the ground
    }

    public void SlamHit(Vector3 forceVector, float stunLength)
    {
        StartCoroutine(Slam(forceVector, stunLength));
        //character.Move(forceVector * Time.deltaTime);
    }
    IEnumerator Slam(Vector3 force, float stunLength)
    {
        canMove = false;

        float timer = 0;
        while (timer < 0.15f)
        {
            character.Move(force * Mathf.Min(1, Time.deltaTime) * Mathf.Min(Time.deltaTime / 0.15f));
            yield return null;
            timer += Time.deltaTime;
        }

        if (shieldsRemaining <= 0)
        {
            if (!inDeathSequence)
            {
                //ghost.PlayBiteSound();
                StartCoroutine(DeathSequence());
            }
        }
        else
        {
            if (shieldAnimator != null && shieldsRemaining > 0)
            {
                if (gunActivated) lostShield = true;

                shieldsRemaining--;
                print("shields: " + shieldsRemaining);

                if (shieldsRemaining <= 0)
                {
                    shieldAnimator.PlayShieldBreak();
                }
            }

            yield return new WaitForSeconds(stunLength);
            canMove = !inDeathSequence;
        }

    }
    #endregion

    #region Gun Functionality
    bool chargeReady;
    /// <summary>
    /// When the left mouse button is pressed this generates a raycast to where the player was aiming.
    /// A sound effect is played and the raytrace is rendered
    /// </summary>
    void RailgunFire()
    {
        if (canFire == false)
            return;

        if (Input.GetMouseButtonUp(0) && !paused && !overheated)
        {
            if (weaponCharge >= 1)
            {
                if (camShake) camShake.ShakeCamera(camShakeFrequency, 0.5f, camShakeDuration);

                if (Score.totalShotsFired <= 0 && tutorial)
                {
                    //tutorial.ToggleReleasePrompt(false);
                }

                weaponSound.Stop();
                weaponChargeSound.Stop();
                weaponSound.PlayOneShot(gunshotSFX);

                Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
                RaycastHit hit;

                //Detect hit on enemy
                if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange, targetingMask))
                {
                    print("Hit: " + hit.collider.gameObject.name);

                    TargetAreaCollider targetAreaCollider = hit.collider.GetComponent<TargetAreaCollider>();

                    if (targetAreaCollider != null)
                    {
                        Ghost.HitInformation hitInformation = targetAreaCollider.OnShot();
                        Score.AddToScore(tempColorSave, (hitInformation.pointWorth + hitInformation.targetArea.pointsAddition));

                        SpawnBlood(hitInformation.bigBlood, hitInformation.smallBlood, hitInformation.targetArea.difficulty, hit);
                    } else
                    {
                        BossCollider bossCollider = hit.collider.GetComponent<BossCollider>();

                        if (bossCollider != null)
                        {
                            Boss.BossHitInformation hitInformation = bossCollider.boss.GotHit(hit.point, bossCollider.HeadID);
                            if (hitInformation.pointWorth > 0)
                                Score.AddToScore(tempColorSave, hitInformation.pointWorth);
                        }
                        else
                        {
                            Instantiate(wallHitEffect, hit.point + hit.normal * 0.1f, Quaternion.identity);
                        }
                    }
                }

                railGunVFX.Shoot(hit, weaponRange);
                railgunAnimator.SetTrigger("Shoot");
                Score.totalShotsFired++;
                StartCoroutine(Decharge());
            }
            else
            {
                //weaponChargeSound.Pause();
                weaponChargeSound.pitch = -1;
            }
        }
        else if (!paused && Input.GetMouseButtonDown(0) && !overheated)
        {
            weaponChargeSound.pitch = 1;
            weaponChargeSound.time = Mathf.Clamp01(weaponCharge);
            weaponChargeSound.clip = chargeupSFX;
            weaponChargeSound.Play();

            chargeReady = false;

            if (Score.totalShotsFired <= 0 && tutorial)
            {
                //tutorial.ToggleShootPrompt(false);
            }
        }
        else if (!paused && Input.GetMouseButton(0) && !overheated)
        {
            if(chargeReady == true && weaponCharge < 1f)
            {
                weaponChargeSound.pitch = 1;
                weaponChargeSound.time = Mathf.Clamp01(weaponCharge);
                weaponChargeSound.clip = chargeupSFX;
                weaponChargeSound.Play();

                chargeReady = false;
            }

            if (weaponCharge < 1f)
            {
                weaponCharge += (Time.deltaTime * (1 / chargeTime));
            }
            if (weaponCharge >= 1f)
            {
                if(chargeReady == false)
                {
                    weaponChargeReadySound.PlayOneShot(chargeReadySFX);
                    chargeReady = true;
                }
                if (doTutorials && Score.totalShotsFired <= 0 && tutorial)
                {
                    //tutorial.ToggleReleasePrompt(true);
                }
            }

            weaponTemp += (Time.deltaTime * (overheatSpeed));
        }
        else if (!Input.GetMouseButton(0) && !overheated)
        {
            if (weaponCharge > 0)
            {
                weaponCharge -= (Time.deltaTime * (1 / (chargeTime / 2)));
                weaponTemp -= Time.deltaTime * (overheatSpeed * 2);
            } else
            {
                weaponTemp = 0;
                weaponCharge = 0;
            }
        }
    }

    /// <summary>
    /// Decharges the gun and acts as a shot cooldown
    /// </summary>
    /// <returns></returns>
    private IEnumerator Decharge()
    {
        canFire = false;
        weaponCharge = 1;
        weaponDecharge = 0;

        weaponDecharging = true;

        while (weaponDecharge <= 1)
        {
            weaponCharge -= (Time.deltaTime * (1 / dechargeTime));
            weaponDecharge += (Time.deltaTime * (1 / dechargeTime));

            yield return null;
        }

        weaponDecharging = false;

        weaponCharge = 0;
        weaponDecharge = 1;
        weaponTemp = 0f;
        canFire = true;
    }

    /// <summary>
    /// Handles the changes to the charge UI and the sound effect for weapon overheating. Also decreases the temperature over time
    /// </summary>
    void CheckWeaponTemp()
    {
        if (weaponTemp >= maxWeaponTemp && !overheated)
        {
            if (doTutorials && Score.timesOverheated <= 1 && tutorial)
            {
                //tutorial.ToggleReleasePrompt(false);
                //tutorial.ToggleOverheatPrompt(true);
            }

            Score.timesOverheated++;

            overheated = true;
            //weaponSound.volume = .9f;
            weaponSound.PlayOneShot(overheatSFX);
            //weaponSound.volume = .1f;
        }
        else if (weaponTemp <= 0f && overheated)
        {
            weaponTemp = 0f;
            weaponCharge = 0f;
            overheated = false;
        }
        else if (overheated && !Input.GetMouseButton(0))
        {
            if (doTutorials && Score.timesOverheated <= 2 && tutorial)
            {
                //tutorial.ToggleOverheatPrompt(false);
            }

            weaponTemp -= Time.deltaTime * cooldownSpeed;
        }
    }

    void SpawnBlood(GameObject bigBlood, GameObject smallBlood, Ghost.TargetAreaDifficulty difficulty, RaycastHit hit)
    {
        float spawnRadius = 0.5f;
        if (difficulty == Ghost.TargetAreaDifficulty.Easy)
        {
            spawnRadius = 0.2f;
            GameObject blood = smallBlood;
            for (int i = 0; i < 2; i++)
            {
                Instantiate(blood, hit.point +
                    hit.transform.right * Random.Range(-spawnRadius, spawnRadius) + hit.transform.up * Random.Range(-spawnRadius / 2, spawnRadius / 2), Quaternion.identity);
            }
        }
        else if (difficulty == Ghost.TargetAreaDifficulty.Medium)
        {
            GameObject blood = smallBlood;
            for (int i = 0; i < 3; i++)
            {
                Instantiate(blood, hit.point +
                    hit.transform.right * Random.Range(-spawnRadius, spawnRadius) + hit.transform.up * Random.Range(-spawnRadius / 2, spawnRadius / 2), Quaternion.identity);
            }

            blood = bigBlood;
            for (int i = 0; i < 1; i++)
            {
                Instantiate(blood, hit.point +
                    hit.transform.right * Random.Range(-spawnRadius, spawnRadius) + hit.transform.up * Random.Range(-spawnRadius / 2, spawnRadius / 2), Quaternion.identity);
            }
        }
        else
        {
            GameObject blood = smallBlood;
            for (int i = 0; i < 4; i++)
            {
                Instantiate(blood, hit.point +
                    hit.transform.right * Random.Range(-spawnRadius, spawnRadius) + hit.transform.up * Random.Range(-spawnRadius / 2, spawnRadius / 2), Quaternion.identity);
            }

            blood = bigBlood;
            for (int i = 0; i < 3; i++)
            {
                Instantiate(blood, hit.point +
                    hit.transform.right * Random.Range(-spawnRadius, spawnRadius) + hit.transform.up * Random.Range(-spawnRadius / 2, spawnRadius / 2), Quaternion.identity);
            }
        }
    }
    #endregion

    #region Stun-Gun Functionality
    /// <summary>
    /// Handles the firing of the stungun, spawning the projectile and initializing it's motion.
    /// </summary>
    void StungunFire()
    {

        StunGunCanFire = stunFireTimer <= 0 && ammoCount > 0;

        if (stunFireTimer <= 0 && Input.GetMouseButtonDown(0) && ammoCount > 0)
        {
            Instantiate(stunProjectile, bulletOrigin.position, bulletOrigin.rotation);

            Score.totalStunsFired++;

            if (ammoCount == maxAmmoCount)
                pelletCountSinceLastShot = Score.pelletsCollected;

            stungunVFX.Shoot();
            if (stungunAnimator != null)
                stungunAnimator.SetTrigger("Shoot");

            stunFireTimer = timeBtwStunShots;
            weaponSound.PlayOneShot(stunShotSFX);
            ammoCount--;
            ammoText.text = "" + Mathf.RoundToInt(Mathf.Clamp(ammoCount + ((Score.pelletsCollected - pelletCountSinceLastShot) % pelletsPerStunAmmo) / (float)pelletsPerStunAmmo, 0, maxAmmoCount) * 100) + "%";
        }
        else if (stunFireTimer > 0)
        {
            stunFireTimer -= Time.deltaTime;
        }
    }
    public void CheckToAddStunAmmo()
    {
        if ((Score.pelletsCollected - pelletCountSinceLastShot) > 0 && (Score.pelletsCollected - pelletCountSinceLastShot) % pelletsPerStunAmmo == 0)
        {
            if (ammoCount < maxAmmoCount)
                ammoCount++;
        }
        ammoText.text = "" + Mathf.RoundToInt(Mathf.Clamp(ammoCount + ((Score.pelletsCollected - pelletCountSinceLastShot) % pelletsPerStunAmmo) / (float)pelletsPerStunAmmo, 0, maxAmmoCount) * 100) + "%";
    }
    #endregion

    /// <summary>
    /// Activates the corresponding outline for targeted area
    /// </summary>
    private void OutlineTargetEnemy()
    {
        Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit hit;

        //Detect hit on enemy
        if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange, targetingMask))
        {
            //print("Targetting: " + hit.collider.gameObject.name);

            TargetAreaCollider targetAreaCollider = hit.collider.GetComponent<TargetAreaCollider>();

            if (targetAreaCollider != null)
            {
                TargetAreaCollider.TargetInfo target = targetAreaCollider.OnTarget();
                targetOutlineController.SetTargetOutline(target);
                crosshair.color = targetingColor;

                //Set TempColorSave Variable for Score method
                tempColorSave = targetOutlineController.GetOutlineColor(target);
            } else
            {
                crosshair.color = Color.white;
                tempColorSave = Color.white;
                targetOutlineController.DeactivateOutline();
            }
        }
        else
        {
            //print("Not Targetting Anything");
            crosshair.color = Color.white;
            tempColorSave = Color.white;
            targetOutlineController.DeactivateOutline();
        }
    }

    #region Gun Powerup Functions
    Coroutine gunTimerCoroutine;
    /// <summary>
    /// Activates the gun and any related visuals and start the gun timer coroutine
    /// </summary>
    IEnumerator ActivateGun()
    {
        if (gunTimerCoroutine != null)
            StopCoroutine(gunTimerCoroutine);

        if (stungunAnimator != null)
        {
            stungunAnimator.SetBool("Equipped", false);
            yield return new WaitForSeconds(0.2f);
        } else
        {
            stunGun.SetActive(false);
        }
        if (Score.bossEnding)
        {
            WeaponSpawner ws = FindObjectOfType<WeaponSpawner>();
            ws.Reset();
        }
        gunActivated = true;
        gun.SetActive(true);
        hud.SetActive(true);

        //Deactivate stun gun
        //stunGun.SetActive(false);

        if (faceController)
            faceController.RailgunPickup();

        if (hudMessenger)
            hudMessenger.Display(railgunAlertMessage, railgunAlertLength);

        musicPlayer.Stop();
        musicPlayer.PlayOneShot(powerMusic);
        //musicPlayer.volume = musicPlayer.volume * powerMusicVolBoost;

        if (railgunAnimator != null)
        {
            railgunAnimator.ResetTrigger("UnequipGun");
            railgunAnimator.SetTrigger("EquipGun");
        }

        weaponTemp = 0;
        weaponCharge = 0;
        weaponDecharge = 0;
        overheated = false;
        weaponDecharging = false;

        if (railGunVFX != null)
            railGunVFX.ActivateEffects();

        foreach (Ghost ghost in ghosts)
        {
            ghost.InitiateScatter();
        }
        gunTimerCoroutine = StartCoroutine(GunTimer());

        baseSpeed += gunSpeedMultiplier;
        AddShields();
        lostShield = false;

        yield return new WaitForSeconds(0.5f);

        if (doTutorials && gunActivated && Score.totalShotsFired <= 0 && tutorial)
        {
            //tutorial.ToggleShootPrompt(true);
        }
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
            yield return null;
        }

        StartCoroutine(DeactivateGun());
        gunTimer = gunTimeAmount;
    }

    /// <summary>
    /// Deactivates the gun and any related visuals
    /// </summary>
    IEnumerator DeactivateGun()
    {
        gunActivated = false;
        gun.SetActive(false);
        hud.SetActive(false);

        musicPlayer.Stop();

        //musicPlayer.volume = musicPlayer.volume / powerMusicVolBoost;

        //Activates Stun-Gun again
        //stunGun.SetActive(true);

        if (gunTimerCoroutine != null)
        {
            StopCoroutine(gunTimerCoroutine);
        }

        if (railgunAnimator != null)
        {
            railgunAnimator.ResetTrigger("EquipGun");
            railgunAnimator.SetTrigger("UnequipGun");

            yield return new WaitForSeconds(0.2f);
        }

        foreach (Ghost ghost in ghosts)
        {
            ghost.DeactivateScatter();
        }

        gunTimerCoroutine = null;

        //Deactivate any remaining target outline
        targetOutlineController.DeactivateOutline();

        if (shieldsRemaining > 0 && !lostShield)
        {
            shieldsRemaining--;
            print("shields: " + shieldsRemaining);
            if (shieldAnimator != null && shieldsRemaining == 0)
            {
                shieldAnimator.PlayShieldDown();
            }
        }
        baseSpeed -= gunSpeedMultiplier;

        if (!inDeathSequence && stungunAnimator != null)
        {
            stungunAnimator.SetBool("Equipped", true);
            //yield return new WaitForSeconds(0.2f);
        } else
        {
            stunGun.SetActive(true);
        }
    }
    #endregion

    #region Collision Detection (Weapon, Fruit, Enemy)
    //Accounting for scatter ending when player is touching a ghost
    private void OnTriggerStay(Collider other)
    {
        if (Score.wonLevel)
            return;

        if (other.tag == "Weapon" && !gunActivated)
        {
            Destroy(other.gameObject);
            StartCoroutine(ActivateGun());
        }

        if (other.tag == "Enemy")
        {
            Ghost ghost = other.gameObject.transform.root.GetComponent<Ghost>();

            if (ghost.CurrentMode == Ghost.Mode.Chase || ghost.CurrentMode == Ghost.Mode.Scatter || ghost.CurrentMode == Ghost.Mode.InvisibilityPowerUp || ghost.CurrentMode == Ghost.Mode.BossfightMove)
            {
                //print("hit by " + other.gameObject.name);

                if (shieldsRemaining <= 0)
                {
                    if (!inDeathSequence)
                    {
                        ghost.PlayBiteSound();
                        StartCoroutine(DeathSequence());
                    }
                }
                else
                {
                    if (shieldAnimator != null && shieldsRemaining > 0)
                    {
                        if (gunActivated) lostShield = true;

                        shieldsRemaining--;
                        print("shields: " + shieldsRemaining);

                        //stun ghost
                        other.SendMessage("FreezeGhost");

                        if (shieldsRemaining <= 0)
                        {
                            //baseSpeed -= gunSpeedMultiplier;
                            shieldAnimator.PlayShieldBreak();
                        }
                    }
                }
            }
            else if (Score.insanityEnding)
            {
                ghost.PermenantDeath();
                baseSpeed += sprintMultiplier;
                permenantGhostsKilled++;
                //corruptedView.SetFloat("_Strength", corruptedView.GetFloat("_Strength") + 0.2f);
                corruptionEffect.ProgressCorruption();

                eatSoundSource.PlayOneShot(eatSound);

                if (permenantGhostsKilled == 4)
                {
                    StartCoroutine(InsanityEnd());
                }
            }
        } else if (canBeDamaged && other.tag == "EnemyProjectile")
        {
            Debug.LogWarning("Player hit by enemy projectile: " + other.gameObject.transform.parent);

            if (shieldsRemaining <= 0)
            {
                if (!inDeathSequence)
                {
                    //ghost.PlayBiteSound();
                    StartCoroutine(DeathSequence());
                }
            }
            else
            {
                if (shieldAnimator != null && shieldsRemaining > 0)
                {
                    if (gunActivated) lostShield = true;

                    shieldsRemaining--;
                    print("shields: " + shieldsRemaining);

                    if (shieldsRemaining <= 0)
                    {
                        shieldAnimator.PlayShieldBreak();
                    }
                }

                canBeDamaged = false;
                Invoke("CanBeDamaged", 1f);
            }
        }
    }
    public void CanBeDamaged()
    {
        canBeDamaged = true;
    }
    public void TakeDamage()
    {
        if (canBeDamaged)
        {
            if (shieldsRemaining <= 0)
            {
                if (!inDeathSequence)
                {
                    //ghost.PlayBiteSound();
                    StartCoroutine(DeathSequence());
                }
            }
            else
            {
                if (shieldAnimator != null && shieldsRemaining > 0)
                {
                    if (gunActivated) lostShield = true;

                    shieldsRemaining--;
                    print("shields: " + shieldsRemaining);

                    if (shieldsRemaining <= 0)
                    {
                        shieldAnimator.PlayShieldBreak();
                    }
                }

                canBeDamaged = false;
                Invoke("CanBeDamaged", 1f);
            }
        }
    }
    #endregion

    IEnumerator DeathSequence()
    {
        inDeathSequence = true;

        canMove = false;
        character.enabled = false;
        canFire = false;

        if (gunActivated)
        {
            StartCoroutine(DeactivateGun());
        }
        else
        {
            if (stungunAnimator != null)
            {
                stungunAnimator.SetBool("Equipped", false);
            }
            else
            {
                stunGun.SetActive(false);
            }
        }

        if (invisibilityActivated)
            DeactivateInvisibility();

        if (speedBoostActivated)
            DeactivateSpeed();

        //Stop all of the ghosts' movements
        foreach (Ghost g in ghosts)
        {
            g.StopGhost();
        }

        WaitForSeconds deathTimer = new WaitForSeconds(deathSequenceLength / 2);

        deathAnimator.SetTrigger("Death");

        if (faceController)
            faceController.Die();

        yield return deathTimer;

        yield return deathTimer;

        yield return deathTimer;

        if (Score.bossEnding)
        {
            BossSpawner bossSpawner = FindObjectOfType<BossSpawner>();
            if (bossSpawner) bossSpawner.ResetGhosts();
            Boss boss = FindObjectOfType<Boss>();
            if (boss) boss.ResetBoss();
        }
        else
        {
            //Reset the Ghosts
            foreach (Ghost g in ghosts)
            {
                g.ResetGhost();
            }
        }

        //Reset the fruit (just in case)
        FruitController fruitController = FindObjectOfType<FruitController>();
        if (fruitController != null)
        {
            fruitController.DeactivateFruit();
        }

        transform.position = playerSpawnPoint.position;

        playerLives--;
        Score.totalLivesConsumed++;
        AchievementManager.addDeath();
        if (playerLives <= 0)
        {
            print("Ending Scene");
            SaveLives();
            //end game scene
            //corruptedView.SetFloat("_Strength", 0);
            //Score.GameEnd();
            Score score = FindObjectOfType<Score>();
            StartCoroutine(score.SceneEnd(true));
        }
        else
        {
            deathAnimator.SetTrigger("FadeOut");

            if (faceController)
                faceController.Respawn();

            yield return deathTimer;

            if (stungunAnimator != null)
            {
                stungunAnimator.SetBool("Equipped", true);
            } else
            {
                stunGun.SetActive(true);
            }

            character.enabled = true;
            canMove = true;
            canFire = true;
            inDeathSequence = false;

            LivesText.text = "" + playerLives;
        }
    }

    public void AddShields()
    {
        shieldsRemaining++;
        Score.totalShieldsRecieved++;
        print("shields: " + shieldsRemaining);
        if (shieldAnimator != null && shieldsRemaining == 1)
            shieldAnimator.PlayShieldUp();
    }

    #region Extra Life
    public void AddLives()
    {
        if (extraLifeSource != null)
            extraLifeSource.PlayOneShot(extraLifeSound);

        if (extraLifeFlash != null)
            StartCoroutine(ExtraLifeFlash());

        playerLives++;
        LivesText.text = "" + playerLives;
    }
    IEnumerator ExtraLifeFlash()
    {
        extraLifeFlash.gameObject.SetActive(true);

        float alpha = 0;
        Color color = extraLifeFlash.color;
        color.a = alpha;
        extraLifeFlash.color = color;

        float change = 1 / (flashSpeed / 2);
        while (alpha < .75f)
        {
            alpha += change * Time.deltaTime;
            print(alpha);
            color = extraLifeFlash.color;
            color.a = alpha;
            extraLifeFlash.color = color;
            yield return null;
        }

        alpha = 0.75f;
        color = extraLifeFlash.color;
        color.a = alpha;
        extraLifeFlash.color = color;

        while (alpha > 0)
        {
            alpha -= change * Time.deltaTime;
            color = extraLifeFlash.color;
            color.a = alpha;
            extraLifeFlash.color = color;
            yield return null;
        }

        alpha = 0;
        color = extraLifeFlash.color;
        color.a = alpha;
        extraLifeFlash.color = color;

        extraLifeFlash.gameObject.SetActive(false);
    }
    public void DisplayPLayerShields()
    {
        shieldText.text = "" + shieldsRemaining;
    }
    #endregion

    #region Invisibility Power-Up
    Coroutine invisibilityPowerUpCoroutine;
    public void ActivateInvisibility()
    {
        foreach (Ghost ghost in ghosts)
        {
            ghost.ActivatedInvisibilityPowerUp();
        }

        if (Score.bossEnding)
        {
            Boss boss = FindObjectOfType<Boss>();
            boss.InvisibililtyPowerupActivated();
        }


        if (invisibilityPowerUpCoroutine != null)
            StopCoroutine(invisibilityPowerUpCoroutine);

        invisibilityPowerUpCoroutine = StartCoroutine(InvisibilityPowerUp());

        if (invisibilitySource != null)
            invisibilitySource.PlayOneShot(invisibilityActivate);
    }

    IEnumerator InvisibilityPowerUp()
    {
        invisibilityActivated = true;

        yield return new WaitForSeconds(invisibilityLength);

        DeactivateInvisibility();
    }
    public void DeactivateInvisibility()
    {
        if (invisibilityPowerUpCoroutine != null)
            StopCoroutine(invisibilityPowerUpCoroutine);

        invisibilityActivated = false;

        if (invisibilitySource != null)
            invisibilitySource.PlayOneShot(invisibilityDeactivate);
    }
    #endregion

    #region Speed Power-Up
    Coroutine speedPowerUp;
    public void ActivateSpeed()
    {
        if (speedPowerUpSource != null)
            speedPowerUpSource.PlayOneShot(speedActivate);

        if (speedBoostActivated)
            DeactivateSpeed();

        if (speedPowerUp != null)
            StopCoroutine(speedPowerUp);

        speedPowerUp = StartCoroutine(SpeedBoost());
    }

    IEnumerator SpeedBoost()
    {
        speedBoostActivated = true;

        baseSpeed += speedIncrease;

        yield return new WaitForSeconds(speedPowerUpLength);

        if (speedBoostActivated)
        {
            if (speedPowerUpSource != null)
                speedPowerUpSource.PlayOneShot(speedDeactivate);

            DeactivateSpeed();
        }
    }
    public void DeactivateSpeed()
    {
        speedBoostActivated = false;
        baseSpeed -= speedIncrease;
    }
    #endregion

    /// <summary>
    /// Disable the character controller temporarily to set the position to given location. (Used in combination with the teleport class) 
    /// </summary>
    public void SetPosition(Vector3 pos)
    {
        character.enabled = false;
        transform.position = pos;
    }

    #region UI Functions
    ///Summary
    ///Pause Game Function
    ///Summary
    private void PauseGame()
    {
        if (!MainMenuManager.isGamePaused)
        {
            mainMenuManager.PauseGame();
        }
        else
        {
            mainMenuManager.ResumeGame();
        }
    }

    /// <summary>
    /// gets the players settings from playerprefs and applies them to the player
    /// will be also used for the pause menu
    /// </summary>
    public void ApplyGameSettings()
    {
        if (PlayerPrefs.HasKey("FOV"))
            playerCam.GetComponent<Camera>().fieldOfView = PlayerPrefs.GetFloat("FOV");
        
        if (PlayerPrefs.HasKey("Sensitivity"))
            sensitivity = PlayerPrefs.GetFloat("Sensitivity") / 5f;

        doTutorials = !PlayerPrefs.HasKey("TutorialPrompts") || PlayerPrefs.GetInt("TutorialPrompts") == 1;
    }
    #endregion

    public void SaveLives()
    {
        PlayerPrefs.SetInt("Lives", playerLives);
    }

    public void EnableCorruptionEnding()
    {
        baseSpeed += sprintMultiplier * 2;
        corruptionEffect.StartCorruption();
        //corruptedView.SetFloat("_Strength", 0.1f);
    }

    IEnumerator InsanityEnd()
    {
        canMove = false;
        float stepTime = 0.2f;

        AudioListener.volume = 1;

        int iterations = 10;
        float volumeStep = 1 / iterations;

        for (int i = 0; i < iterations; i++)
        {
            corruptionEffect.ProgressCorruptionEnd();
            yield return new WaitForSeconds(stepTime);
            AudioListener.volume -= volumeStep + 0.1f;
        }
        AudioListener.volume = 0;

        //Score.GameEnd();
        Score score = FindObjectOfType<Score>();
        StartCoroutine(score.SceneEnd(false));
    }
}
