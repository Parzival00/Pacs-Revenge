using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] float speed;
    [SerializeField] Transform origin;
    [SerializeField] Rigidbody rigidbody;
    protected int timeToLive;
    protected bool fired;

    public void Awake()
    {
        fired = false;
        this.origin = this.transform;
    }
    // Start is called before the first frame update
    void Start()
    {
        timeToLive = 600;
        this.transform.position = origin.position;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        move();
        timeToLive--;
        if (timeToLive <= 0)
        {
            Destroy(this.gameObject);
        }
    }
    /// <summary>
    /// Applies force and moves the shot
    /// </summary>
    public void move()
    {
        if (!fired)
        {
            this.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0f, 0f, 1f) * speed);
            fired = true;
        }
    }
}
