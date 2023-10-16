using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] float speed = 200;
    [SerializeField] Transform origin;
    //[SerializeField] Rigidbody rigidbody;
    protected int timeToLive;
    //protected bool fired;

    public void Awake()
    {
        //fired = false;
        //this.origin = this.transform;
    }
    // Start is called before the first frame update
    void Start()
    {
        timeToLive = 4;

        //this.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0f, 0f, 1f) * speed);

        //this.transform.position = origin.position;

        Destroy(this.gameObject, timeToLive);
    }
    // Update is called once per frame
    void LateUpdate()
    {
        Move();
        //move();
        /*timeToLive--;
        if (timeToLive <= 0)
        {
            Destroy(this.gameObject);
        }*/
    }
    /// <summary>
    /// Applies force and moves the shot
    /// </summary>
    public void Move()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
        /*if (!fired)
        {
            this.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0f, 0f, 1f) * speed);
            fired = true;
        }*/
    }
}
