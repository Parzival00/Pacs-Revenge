//Toby Mose
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Transform origin;
    public float speed;
    public int damage;
    protected int timeToLive;
    protected bool fired;
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
    public virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "weapon")
        {
            if (collision.gameObject.tag == "enemy")
            {
                collision.gameObject.SendMessage("reset");
            }
        }
        Destroy(this.gameObject);
    }
    public void Awake()
    {
        fired = false;
        this.origin = this.transform;
    }
    public void move()
    {
        if (!fired)
        {
            this.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0f, 0f, 1f) * speed);
            fired = true;
        }
    }
}