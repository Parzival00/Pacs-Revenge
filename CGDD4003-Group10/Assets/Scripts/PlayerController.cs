using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.InputSystem;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerController : MonoBehaviour
{
    public Transform playerCam;
    public Transform playerT;

    MainMenuManager mainMenuManager;

    public bool canMove { get; private set; }
    public bool trapped { get; private set; }

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
    [SerializeField] AudioClip stunShotSFX;
    [SerializeField] AudioClip stunShotEmpty;
    [SerializeField] AudioSource weaponSound;


    [Header("Weapon Settings")]
    [SerializeField] WeaponInfo[] weaponInfos;
    [SerializeField] Weapon[] weapons;
    [SerializeField] string railgunAlertMessage = "Advanced Targeting System Activated";
    [SerializeField] float railgunAlertLength = 2;
    [SerializeField] Animator gunAnimator;
    [SerializeField] float gunTimeAmount = 5f;
    [SerializeField] Camera fpsCam;
    [SerializeField] float camShakeFrequency = 1f;
    [SerializeField] float camShakeDuration = 0.4f;
    [SerializeField] CameraShake camShake;
    [SerializeField] float weaponRange;
    [SerializeField] LayerMask targetingMask;
    CorruptedGunController corruptedGunController;

    Weapon currentWeapon;

    public CameraShake CamShake { get => camShake; }

    [Header("Stun-Gun Settings")]
    [SerializeField] StungunVFX stungunVFX;
    [SerializeField] Animator stungunAnimator;
    [SerializeField] float timeBtwStunShots;
    [SerializeField] int pelletsPerStunAmmo = 10;
    [SerializeField] Transform bulletOrigin;
    [SerializeField] GameObject stunGun;
    int stunAmmoCount;
    [SerializeField] int maxAmmoCount = 1;
    //The amount of pellets per ammo is set in the score script
    private float stunFireTimer;
    private int pelletCountSinceLastShot;

    private int weaponIndex;

    bool lostShield;

    private float gunTimer;

    public float GunTimer01 { get => (gunTimer / gunTimeAmount); }

    public bool StunGunCanFire { get; private set; }
    public int StunAmmoCount { get => stunAmmoCount; }
    public int StunAmmoPerPellets { get => pelletsPerStunAmmo; }
    public bool StunGunAmmoEmpty { get => stunAmmoCount <= 0; }

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
    [SerializeField] TransitionEffect transitionEffect;
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
    private bool holdingFire = false;

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

        weaponIndex = PlayerPrefs.GetInt("Weapon");
        currentWeapon = weapons[weaponIndex];

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
        currentWeapon.gameObject.SetActive(false);
        hud.SetActive(false);

        if (animator == null)
            animator = GetComponent<Animator>();

        if (deathAnimator == null)
            deathAnimator = GameObject.FindGameObjectWithTag("DeathAnimator").GetComponent<Animator>();

        if (stungunAnimator != null)
        {
            stunGun.SetActive(true);
            stungunAnimator.SetTrigger("Equip");
            //stungunAnimator.SetBool("Equipped", true);
        }
        else
        {
            stunGun.SetActive(true);
        }

        if (shieldAnimator == null)
            shieldAnimator = FindObjectOfType<ShieldEffectAnimator>();

        ghosts = new Ghost[4];
        ghosts = FindObjectsOfType<Ghost>();

        corruptedGunController = GetComponent<CorruptedGunController>();

        ApplyGameSettings();

        canMove = true;
        canFire = true;
        canBeDamaged = true;

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
                if(!trapped)
                    MovementControl();
            }

            if (canFire)
            {
                if (gunActivated)
                {
                    OutlineTargetEnemy();
                    currentWeapon.OnPassiveEvent();

                    if (holdingFire) currentWeapon.OnMouseEvent();
                    else currentWeapon.OnNoMouseEvent();
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
            DisplayPlayerShields();
    }


    #region Move and Look
    public void OnMove(InputAction.CallbackContext context)
    {
        targetDirection = context.ReadValue<Vector2>().normalized;
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>() * 0.1f;
    }

    Vector2 mousePosition;
    /// <summary>
    /// This method takes the mouse position and rotates the player and camera accordingly. Using the x position of the mouse for horizontal and the y position for vertical.
    /// </summary>
    void MouseControl()
    {

        cameraPitch -= mousePosition.y * sensitivity * Mathf.Min(0.01f, Time.deltaTime) * 30f * Time.timeScale;
        cameraPitch = Mathf.Clamp(cameraPitch, -35.0f, 50.0f);

        playerCam.localEulerAngles = Vector3.right * cameraPitch;
        playerT.Rotate(Vector3.up * mousePosition.x * sensitivity * Mathf.Min(0.01f, Time.deltaTime) * 30f * Time.timeScale);

    }

    Vector2 targetDirection;
    /// <summary>
    /// Uses the input axis system to move the player's character controller
    /// Has a sprint ability activated by the L-shift key
    /// </summary>
    void MovementControl()
    {
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
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            holdingFire = true;

            if (!MainMenuManager.isGamePaused && canFire && gunActivated) currentWeapon.OnMouseDownEvent();
        }
        else if (context.phase == InputActionPhase.Performed)
        {
            holdingFire = false;

            if (!MainMenuManager.isGamePaused && canFire && gunActivated) currentWeapon.OnMouseUpEvent();
        }
    }
    #endregion

    #region Stun-Gun Functionality
    /// <summary>
    /// Handles the firing of the stungun, spawning the projectile and initializing it's motion.
    /// </summary>
    void StungunFire()
    {

        StunGunCanFire = stunFireTimer <= 0 && stunAmmoCount > 0;

        if (stunFireTimer <= 0 && Input.GetMouseButtonDown(0))
        {
            if (stunAmmoCount > 0)
            {
                Instantiate(stunProjectile, bulletOrigin.position, bulletOrigin.rotation);

                Score.totalStunsFired++;

                if (stunAmmoCount == maxAmmoCount)
                    pelletCountSinceLastShot = Score.pelletsCollected;

                stunAmmoCount--;

                //stungunVFX.Shoot();
                if (stungunAnimator != null)
                {
                    stungunAnimator.SetTrigger("Shoot");
                    Invoke("SetChargeAnimator", 32f / 60);
                }

                stunFireTimer = timeBtwStunShots;
                weaponSound.PlayOneShot(stunShotSFX);
                ammoText.text = "" + Mathf.RoundToInt(Mathf.Clamp(stunAmmoCount + ((Score.pelletsCollected - pelletCountSinceLastShot) % pelletsPerStunAmmo) / (float)pelletsPerStunAmmo, 0, maxAmmoCount) * 100) + "%";
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
    public void CheckToAddStunAmmo()
    {
        if ((Score.pelletsCollected - pelletCountSinceLastShot) > 0 && (Score.pelletsCollected - pelletCountSinceLastShot) % pelletsPerStunAmmo == 0)
        {
            if (stunAmmoCount < maxAmmoCount)
            {
                stunAmmoCount++;
                stungunAnimator.SetFloat("Charge", Mathf.Min(3, stunAmmoCount));
            }
        }
        ammoText.text = "" + Mathf.RoundToInt(Mathf.Clamp(stunAmmoCount + ((Score.pelletsCollected - pelletCountSinceLastShot) % pelletsPerStunAmmo) / (float)pelletsPerStunAmmo, 0, maxAmmoCount) * 100) + "%";
    }
    //An animation event to set the charge float to ensure consistency
    public void SetChargeAnimator()
    {
        if (stungunAnimator != null)
        {
            stungunAnimator.SetFloat("Charge", Mathf.Min(3, stunAmmoCount));
        }
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
            BossCollider bossCollider = hit.collider.GetComponent<BossCollider>();

            if (targetAreaCollider != null)
            {
                TargetAreaCollider.TargetInfo target = targetAreaCollider.OnTarget();
                targetOutlineController.SetTargetOutline(target);
                crosshair.color = targetingColor;

                //Set TempColorSave Variable for Score method
                tempColorSave = targetOutlineController.GetOutlineColor(target);
            } 
            else if(bossCollider != null && bossCollider.boss.CheckHeadDamagable(bossCollider.HeadID))
            {
                TargetAreaCollider.TargetInfo target = bossCollider.OnTarget();
                targetOutlineController.SetTargetOutline(target);
                crosshair.color = targetingColor;

                //Set TempColorSave Variable for Score method
                tempColorSave = targetOutlineController.GetOutlineColor(target);
            }
            else
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
            stungunAnimator.SetTrigger("Unequip");

            yield return new WaitForSeconds(0.3f);
            stunGun.SetActive(false);
        } else
        {
            stunGun.SetActive(false);
        }
        if (Score.bossEnding)
        {
            WeaponSpawner ws = FindObjectOfType<WeaponSpawner>();
            ws.Reset();
        }

        canFire = false;
        gunActivated = true;
        currentWeapon.gameObject.SetActive(true);
        yield return null;
        currentWeapon.ResetWeapon();
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

        if (gunAnimator != null)
        {
            gunAnimator.ResetTrigger("Unequip");
            gunAnimator.SetTrigger("Equip");
        }

        foreach (Ghost ghost in ghosts)
        {
            ghost.InitiateScatter();
        }
        gunTimerCoroutine = StartCoroutine(GunTimer());

        baseSpeed += gunSpeedMultiplier;
        AddShields();
        lostShield = false;

        canFire = true;

        yield return new WaitForSeconds(0.5f);

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
        canFire = false;
        hud.SetActive(false);

        musicPlayer.Stop();

        //musicPlayer.volume = musicPlayer.volume / powerMusicVolBoost;

        //Activates Stun-Gun again
        //stunGun.SetActive(true);

        if (gunTimerCoroutine != null)
        {
            StopCoroutine(gunTimerCoroutine);
        }

        if (gunAnimator != null)
        {
            gunAnimator.ResetTrigger("Equip");
            gunAnimator.SetTrigger("Unequip");

            yield return new WaitForSeconds(0.3f);
        }

        currentWeapon.gameObject.SetActive(false);

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
            stunGun.SetActive(true);
            stungunAnimator.SetTrigger("Equip");
            stungunAnimator.SetFloat("Charge", Mathf.Min(3, stunAmmoCount));
            yield return new WaitForSeconds(0.3f);
        } else
        {
            stunGun.SetActive(true);
        }

        gunActivated = false;
        canFire = true;
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
            WeaponPickup weaponPickup = other.GetComponent<WeaponPickup>();
            if(weaponPickup != null && weaponPickup.isCorrupted)
            {
                corruptedGunController.corruptedGun.ActivateEntrapment(this);
            }
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
                        ghost.FreezeGhost();

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

        //Disable corrupted gun in event of dying while trapped
        trapped = false;
        if(corruptedGunController != null)
            corruptedGunController.DeactivateCorruptedGun();

        if (gunActivated)
        {
            StartCoroutine(DeactivateGun());
        }
        else
        {
            if (stungunAnimator != null)
            {
                stungunAnimator.SetTrigger("Unequip");
                yield return new WaitForSeconds(0.3f);
                stunGun.SetActive(false);
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

        deathAnimator.gameObject.SetActive(true);
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
            deathAnimator.gameObject.SetActive(false);
            transitionEffect.DeathTransition();

            if (faceController)
                faceController.Respawn();

            yield return deathTimer;

            if (stungunAnimator != null)
            {
                stunGun.SetActive(true);
                stungunAnimator.SetFloat("Charge", Mathf.Min(3, stunAmmoCount));
                stungunAnimator.SetTrigger("Equip");
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
    public void DisplayPlayerShields()
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

    public PlayerController(Animator gunAnimator)
    {
        this.gunAnimator = gunAnimator;
    }

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

    public void SetTrapped(bool trapped)
    {
        this.trapped = trapped;
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Lever") 
        {
            other.GetComponent<DoorControl>().ActivateDoor();
        }
    }
}
