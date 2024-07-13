using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Animator animator;
    [Header("Projectile Settings")]
    [SerializeField] bool canHitFloor = false;
    [SerializeField] float speed = 200;
    [SerializeField] float timeToLive = 4;
    [SerializeField] float impactRegisterDelay = 0;
    [SerializeField] Collider impactCollider;
    [SerializeField] bool canHitPlayer = false;
    [SerializeField] bool orientToPlayerOnStart = false;
    [SerializeField] float attackLeading = 2;
    [SerializeField] bool followPlayer = false;
    [SerializeField] [Range(0, 100)] float followSmoothing = 0.1f;

    PlayerController player;

    bool move;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(DestroyDelay(timeToLive));

        move = true;

        if (followPlayer || orientToPlayerOnStart) 
        { 
            player = FindObjectOfType<PlayerController>();
            Vector3 dirToPlayer = (player.transform.position + player.velocity * attackLeading - transform.position).normalized;
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
        if(followPlayer == true)
        {
            Vector3 dirToPlayer = (player.transform.position + player.velocity * attackLeading - transform.position).normalized;
            Quaternion rot = Quaternion.FromToRotation(transform.forward, dirToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot * transform.rotation, followSmoothing * Time.deltaTime);
        }

        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wall" || (canHitPlayer && other.tag == "Player") || (canHitFloor && other.tag == "Floor"))
        {
            move = false;
            animator.SetTrigger("Destroy");
            StartCoroutine(ImpactRegister(impactRegisterDelay));
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
            StartCoroutine(ImpactRegister(impactRegisterDelay));
        }
    }
}
