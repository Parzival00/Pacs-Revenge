using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [System.Serializable]
    public struct DifficultySettings
    {
        public int difficultyLevel;
        public float scale;
        public float speed;
        public float timeToLive;
        public float impactRegisterDelay;
        public float attackLeading;
        public bool followPlayer;
        [Range(0, 100)] public float followSmoothing;
    }

    Animator animator;
    [Header("Projectile Settings")]
    [SerializeField] protected DifficultySettings[] difficultySettings = new DifficultySettings[3];
    [SerializeField] bool canHitFloor = false;
    [SerializeField] Collider impactCollider;
    [SerializeField] bool canHitPlayer = false;
    [SerializeField] bool orientToPlayerOnStart = false;

    PlayerController player;

    protected DifficultySettings currentDifficultySettings;

    bool move;

    // Start is called before the first frame update
    void Start()
    {
        if (Score.difficulty < difficultySettings.Length)
        {
            currentDifficultySettings = difficultySettings[Score.difficulty];
        }
        else
        {
            currentDifficultySettings = difficultySettings[0];
        }

        transform.localScale = Vector3.one * currentDifficultySettings.scale;

        animator = GetComponent<Animator>();
        StartCoroutine(DestroyDelay(currentDifficultySettings.timeToLive));

        move = true;

        if (currentDifficultySettings.followPlayer || orientToPlayerOnStart) 
        { 
            player = FindObjectOfType<PlayerController>();
            Vector3 dirToPlayer = (player.transform.position + player.transform.up * 0.3f + player.velocity * currentDifficultySettings.attackLeading - transform.position).normalized;
            Quaternion rot = Quaternion.FromToRotation(transform.forward, dirToPlayer);
            transform.rotation = rot * transform.rotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(move)
            Move();
    }

    /// <summary>
    /// Applies force and moves the shot
    /// </summary>
    public void Move()
    {
        if(currentDifficultySettings.followPlayer == true)
        {
            Vector3 dirToPlayer = (player.transform.position + player.velocity * currentDifficultySettings.attackLeading - transform.position).normalized;
            Quaternion rot = Quaternion.FromToRotation(transform.forward, dirToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot * transform.rotation, currentDifficultySettings.followSmoothing * Time.deltaTime);
        }

        transform.Translate(transform.forward * currentDifficultySettings.speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wall" || (canHitPlayer && other.tag == "Player") || (canHitFloor && other.tag == "Floor"))
        {
            move = false;
            animator.SetTrigger("Destroy");
            StartCoroutine(ImpactRegister(currentDifficultySettings.impactRegisterDelay));
        }
    }

    IEnumerator ImpactRegister(float delay)
    {
        if (impactCollider) impactCollider.enabled = true;
        yield return null;
        yield return new WaitForSeconds(delay);
        if (impactCollider) impactCollider.enabled = false;

        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    IEnumerator DestroyDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (move == true)
        {
            move = false;
            animator.SetTrigger("Destroy");
            StartCoroutine(ImpactRegister(currentDifficultySettings.impactRegisterDelay));
        }
    }
}
