using UnityEngine;

public class Barrel : MonoBehaviour
{
    Animator barrelAnimation;
    BoxCollider barrelCollider;
    AudioSource boom;
    [SerializeField] float range;
    [SerializeField] float playerRange;
    [SerializeField] float explosionDelay = 0.5f;
    [SerializeField] float explosionPower = 5f;
    [SerializeField] float stunLength = 5f;
    bool exploding = false;

    public enum state
    {
        stable,
        unstable
    }

    void Start()
    {
        barrelAnimation = GetComponent<Animator>();
        barrelCollider = GetComponent<BoxCollider>();
        boom = GetComponent<AudioSource>();
        exploding = false;
    }

    public void StartExplosion()
    {
        if (!exploding)
        {
            boom.Play();
            Invoke("Explosion", explosionDelay);
            exploding = true;
        }
    }

    public void Explosion()
    {
        barrelAnimation.SetTrigger("Explode");
        //Debug.Log("Barrel Should Explode");
        //barrelCollider.enabled = false;

        //Gather things in radius by collider
        Collider[] objectInRange = Physics.OverlapSphere(transform.position, range);

        //Check if Ghost / Player / Other Barrel Depending on type of Barrel
        if (transform.name.Contains("ShockBarrel"))
        {
            foreach (Collider ob in objectInRange)
            {
                Rigidbody moveBody = ob.gameObject.GetComponent<Rigidbody>();

                //Explosion effect on enemy/other barrels
                if (ob.gameObject.tag == "Enemy")
                {
                    /*if (ob.gameObject.GetComponent<Rigidbody>() != null)
                    {
                        moveBody.AddExplosionForce(explosionPower, transform.position, range, 2.0f);
                    }*/
                    //Debug.Log(ob.name);
                    ob.gameObject.GetComponent<Ghost>().FreezeGhost(stunLength);
                    /*switch (ob.gameObject.name) 
                    {
                        case "Blinky":
                            ob.gameObject.GetComponent<Blinky>().FreezeGhost();
                            //Debug.Log(ob.name + " has most likely been frosted (D_D) ...");
                            break;
                        case "Inky":
                            ob.gameObject.GetComponent<Inky>().FreezeGhost();
                            break;
                        case "Pinky":
                            ob.gameObject.GetComponent<Pinky>().FreezeGhost();
                            break;
                        case "Clyde":
                            ob.gameObject.GetComponent<Clyde>().FreezeGhost();
                            break;
                    }*/
                }
                else if (ob.gameObject.name.Contains("ShockBarrel"))
                {
                    /*if (ob.gameObject.GetComponent<Rigidbody>() != null)
                    {
                        moveBody.AddExplosionForce(explosionPower, transform.position, range, 2.0f);
                    }*/
                    ob.GetComponent<Barrel>().StartExplosion();
                }
            }
        }
        else if (transform.name.Contains("ExplosiveBarrel"))
        {
            foreach (Collider ob in objectInRange)
            {
                Rigidbody moveBody = ob.gameObject.GetComponent<Rigidbody>();

                if (ob.gameObject.tag == "Enemy")
                {
                    Ghost ghost = ob.gameObject.GetComponent<Ghost>();
                    ghost.GotHit(ob.gameObject.GetComponent<Ghost>().GetInstakillTargetAreaType(), 99);
                    //ghost.SpawnBlood(10);
                    /*switch (ob.gameObject.name)
                    {
                        case "Blinky":
                            ob.gameObject.GetComponent<Blinky>().GotHit(ob.gameObject.GetComponent<Blinky>().barrelDamage());
                            Ghost.TargetArea b = ob.GetComponent<Blinky>().GetTargetArea(ob.gameObject.GetComponent<Blinky>().barrelDamage());
                            //Debug.Log(g.healthValue + " is really this!?!?!?!");
                            break;
                        case "Inky":
                            ob.gameObject.GetComponent<Inky>().GotHit(ob.gameObject.GetComponent<Inky>().barrelDamage());
                            Ghost.TargetArea i = ob.GetComponent<Inky>().GetTargetArea(ob.gameObject.GetComponent<Inky>().barrelDamage());
                            break;
                        case "Pinky":
                            ob.gameObject.GetComponent<Pinky>().GotHit(ob.gameObject.GetComponent<Pinky>().barrelDamage());
                            Ghost.TargetArea p = ob.GetComponent<Pinky>().GetTargetArea(ob.gameObject.GetComponent<Pinky>().barrelDamage());
                            break;
                        case "Clyde":
                            ob.gameObject.GetComponent<Clyde>().GotHit(ob.gameObject.GetComponent<Clyde>().barrelDamage());
                            Ghost.TargetArea c = ob.GetComponent<Clyde>().GetTargetArea(ob.gameObject.GetComponent<Clyde>().barrelDamage());
                            break;
                    }*/
                    if (ob.gameObject.GetComponent<Rigidbody>() != null)
                    {
                        moveBody.AddExplosionForce(explosionPower * 20, transform.position, range, 2.0f);
                    }
                }
                else if (ob.gameObject.name.Contains("ExplosiveBarrel"))
                {
                    if (ob.gameObject.GetComponent<Rigidbody>() != null)
                    {
                        moveBody.AddExplosionForce(explosionPower * 20, transform.position, range, 2.0f);
                    }
                    ob.GetComponent<Barrel>().StartExplosion();
                }
                /*else if (ob.gameObject.name.Contains("Player"))
                {
                    ob.GetComponent<PlayerController>().TakeDamage();
                }*/
            }

            Collider[] playersInRange = Physics.OverlapSphere(transform.position, playerRange);
            foreach (Collider ob in playersInRange)
            {
                if (ob.gameObject.name.Contains("Player"))
                {
                    ob.GetComponent<PlayerController>().TakeDamage();
                }
            }
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.name + " has hit the barrel");
        if (/*transform.name.Contains("ShockBarrel") && */(other.gameObject.tag == "Stun" || other.gameObject.name == "StunShot"))
        {
            //Debug.Log("this is the " + transform.name);

            //Debug.Log("---- this is the " + other.gameObject.name);

            Destroy(other.gameObject);
            StartExplosion();
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
