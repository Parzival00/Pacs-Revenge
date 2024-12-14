using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserWall : MonoBehaviour
{
    Map map;

    Animator animator;

    [SerializeField] Vector2 activationTimes = new Vector2(8, 12);
    [SerializeField] Vector2 cooldownTimes = new Vector2(15, 20);
    [SerializeField] float ghostReactionDelay = 3f;
    [SerializeField] GameObject[] lasers;
    [Header("Audio")]
    [SerializeField] AudioSource laserSound;
    [SerializeField] AudioSource laserShockSound;
    [SerializeField] AudioSource activator1DownSound;
    [SerializeField] AudioSource activator2DownSound;
    [SerializeField] AudioSource activator1UpSound;
    [SerializeField] AudioSource activator2UpSound;

    Vector2Int gridLoc;

    public bool activated { get; private set; }
    float activationTime;
    public bool onCooldown { get; private set; }
    float cooldownTime;

    float timer = 0;

    Collider collider;

    LaserWallActivator currentActivator;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider>();
        animator = GetComponent<Animator>();

        map = FindObjectOfType<Map>();
        gridLoc = map.GetGridLocation(transform.position);

        activated = false;
        onCooldown = false;
        timer = 0;

        foreach (GameObject obj in lasers)
        {
            obj.SetActive(false);
        }
        collider.enabled = false;
    }

    private void Update()
    {
        if(activated && timer > activationTime)
        {
            DeactivateLaser();
        }
        else if (onCooldown && timer > cooldownTime)
        {
            onCooldown = false;
            animator.SetBool("PressedDown", false);

            activator1UpSound.Play();
            activator2UpSound.Play();
        }

        timer += Time.deltaTime;
    }

    public void ActivateLaser(LaserWallActivator activator)
    {
        currentActivator = activator;

        activated = true;
        collider.enabled = true;

        animator.SetBool("PressedDown", true);

        activator1DownSound.Play();
        activator2DownSound.Play();
        laserSound.Play();

        foreach (GameObject obj in lasers)
        {
            obj.SetActive(true);
        }

        timer = 0;
        activationTime = Random.Range(activationTimes.x, activationTimes.y);

        Invoke("SetLaserAsWall", ghostReactionDelay);
    }
    void SetLaserAsWall()
    {
        map.SetGridAtPosition(gridLoc, Map.GridType.Wall);
    }

    public void DeactivateLaser()
    {
        activated = false;
        collider.enabled = false;

        laserSound.Stop();

        foreach (GameObject obj in lasers)
        {
            obj.SetActive(false);
        }

        onCooldown = true;

        map.SetGridAtPosition(gridLoc, Map.GridType.Air);

        timer = 0;
        cooldownTime = Random.Range(cooldownTimes.x, cooldownTimes.y);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Enemy"))
        {
            Ghost ghost = other.transform.GetComponent<Ghost>();
            if (ghost != null)
            {
                laserShockSound.Play();
                ghost.GotHit(Ghost.TargetAreaType.Head);
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            PlayerController player = other.transform.GetComponent<PlayerController>();
            if (player != null)
            {
                laserShockSound.Play();
                player.TakeDamage();

            }
        }
    }
}
