using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LeverDoor : MonoBehaviour
{
    [Header("Door Controls")]
    [SerializeField] GameObject currentDoor;
    [SerializeField] GameObject leverToDoor;

    [SerializeField] AudioSource doorUp;
    [SerializeField] AudioSource doorDown;

    Animator leverAnimation;
    Animator doorAnimation;

    [Header("Door/Lever Timers")]
    [SerializeField] float doorSpeed = 5f;
    [SerializeField] float leverResetTimer = 5f;
    [SerializeField] float doorResetTimer = 5f;

    int leverState = 1;
    int currentDoorState = 1;

    float timer,intervalTimer;

    Vector3 doorOpenPosition, doorClosedPosition;

    void Start()
    {
        /*Vector3 doorOpenPosition = new Vector3(currentDoor.transform.position.x, -5f, currentDoor.transform.position.z);
        Vector3 doorCLosedPosition = currentDoor.transform.position;*/
        
        //Get Animators
        doorAnimation = GetComponent<Animator>();
        Debug.Log("door animation obtained");
        foreach (Transform l in this.transform) 
        {
            if (l.gameObject.name == "ControllableDoorLever") 
            {
                leverAnimation = l.GetComponent<Animator>();
                Debug.Log("lever animation obtained");
            }
        }

        timer = 0;
        intervalTimer = Random.Range(8f,12f);
    }


    void Update()
    {
        //CheckDoorState();
        if (timer > intervalTimer) 
        {
            StartCoroutine(CloseDoor());
        }
        timer += Time.deltaTime;
    }
    /*public void ActivateDoor()
    {
        if (currentDoorState == 0) //If closed
        {
            currentDoorState = 1;
            currentDoor.transform.position = Vector3.MoveTowards(currentDoor.transform.position, doorClosedPosition, doorSpeed * Time.deltaTime);
            Invoke("ResetDoor", doorResetTimer);
        }
    }*/
    /*public int CheckLeverState()
    {
        if (leverState == 0)
        {
            return 0;
        }
        else if (leverState == 1)
        {
            return 1;
        }
        return leverState;
        //yield return null;
    }*/
    /*public void CheckDoorState()
    {
        if (currentDoorState == 1)
        {
            Invoke("ResetDoor", doorResetTimer);
        }
    }*/
    public void SetLeverState()
    {
        /*if (leverState == 0)
        {
            leverState = 1;
        }*/
        if (leverState == 1) //Set Lever Off, then reset in certain amount of time
        {
            leverState = 0;
            leverAnimation.SetBool("On",true);
            leverAnimation.SetBool("Cooldown",true);
            Invoke("ResetLever", leverResetTimer);
        }
        //yield return new WaitForSeconds(5f);
        //yield return null;
    }
    public void ResetLever()
    {
        if (leverState == 0) //Set Lever back on
        {
            leverState = 1;
            leverAnimation.SetBool("On", false);
            leverAnimation.SetBool("Cooldown", false);
        }
        
    }
    public void ResetDoor()
    {
        if (currentDoorState == 1) //If Open
        {
            currentDoorState = 0;
            currentDoor.transform.position = Vector3.MoveTowards(currentDoor.transform.position, doorOpenPosition, doorSpeed * Time.deltaTime);
        }
    }
    public IEnumerator CloseDoor() 
    {
        if (currentDoorState == 1) 
        {
            currentDoorState = 0;
            doorAnimation.SetBool("Closed",true);
            timer = 0;
            doorUp.Play();
        }
        yield return null;
    }
    public IEnumerator OpenDoor() 
    {
        if (currentDoorState == 0) 
        {
            currentDoorState = 1;
            doorAnimation.SetBool("Closed",false);
            timer = 0;
            doorDown.Play();
        }
        yield return null;
    }
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "Player") 
        {
            SetLeverState();
            StartCoroutine(OpenDoor());
        }
    }
    /*public void OnCollisionEnter(Collision other)
    {
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "Player")
        {
            SetLeverState();
            StartCoroutine(OpenDoor());
        }
    }*/
}
