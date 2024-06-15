using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Animator animator;
    [Header("Projectile Settings")]
    [SerializeField] float speed = 200;
    [SerializeField] int timeToLive = 4;

    bool move;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(DestroyDelay(timeToLive));

        move = true;
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
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wall")
        {
            move = false;
            animator.SetTrigger("Destroy");
        }
    }

    IEnumerator DestroyDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        move = false;
        animator.SetTrigger("Destroy");
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
