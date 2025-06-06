using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LeverDoor : MonoBehaviour
{
    [Header("Door Controls")]
    [SerializeField] GameObject currentDoor;
    [SerializeField] Animator[] leverAnimators;
    [SerializeField] GameObject leverToDoor;

    [SerializeField] AudioSource doorUp;

    [SerializeField] AudioSource doorDown;
    [SerializeField] LayerMask overlapCheckMask;
    [SerializeField] int overlapCubeLength = 3;

    Animator doorAnimation;

    Map map;

    Vector2Int gridLoc;

    [Header("Door/Lever Timers")]
    [SerializeField] float doorSpeed = 5f;
    [SerializeField] float leverResetTimer = 5f;
    [SerializeField] float doorResetTimer = 5f;
    [SerializeField] float doorCooldownTimer = 3f;

    int leverState = 1;
    int currentDoorState = 1;

    bool canCloseDoor;

    float doorCloseTimer,intervalTimer,cooldownTimer;

    Vector3 doorOpenPosition, doorClosedPosition;

    void Start()
    {
        /*Vector3 doorOpenPosition = new Vector3(currentDoor.transform.position.x, -5f, currentDoor.transform.position.z);
        Vector3 doorCLosedPosition = currentDoor.transform.position;*/

        map = FindObjectOfType<Map>();
        gridLoc = map.GetGridLocation(transform.position);

        //Get Animators
        doorAnimation = GetComponent<Animator>();
        Debug.Log("door animation obtained");
        /*foreach (Transform l in this.transform) 
        {
            if (l.gameObject.name == "ControllableDoorLever") 
            {
                leverAnimators = l.GetComponent<Animator>();
                Debug.Log("lever animation obtained");
            }
        }*/

        doorCloseTimer = 0;

        leverState = 1;
        currentDoorState = 1;

        canCloseDoor = true;
        //SetLeverState();
    }


    void Update()
    {
        //CheckDoorState();

        if (currentDoorState == 0)
        {

            if (doorCloseTimer > doorResetTimer && canCloseDoor)
            {
                StartCoroutine(OpenDoor());

                canCloseDoor = false;
            }
            doorCloseTimer += Time.deltaTime;
        }
        else
        {
            if(cooldownTimer > doorCooldownTimer && !canCloseDoor)
            {
                canCloseDoor = true;
                foreach(Animator leverAnimator in leverAnimators)
                {
                    leverAnimator.SetBool("Cooldown", false);
                }
            }
            cooldownTimer += Time.deltaTime;
        }
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
        if (leverState == 1 && canCloseDoor) //Set Lever Off, then reset in certain amount of time
        {
            leverState = 0;
            foreach (Animator leverAnimator in leverAnimators)
            {
                leverAnimator.SetBool("On", true);
                leverAnimator.SetBool("Cooldown", true);
            }
            //Invoke("ResetLever", leverResetTimer);
        }
        //yield return new WaitForSeconds(5f);
        //yield return null;
    }
    public void ResetLever()
    {
        if (leverState == 0) //Set Lever back on
        {
            leverState = 1;
            
            foreach (Animator leverAnimator in leverAnimators)
            {
                leverAnimator.SetBool("On", false);
                leverAnimator.SetBool("Cooldown", false);
            }
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
            doorClosed = false;

            WaitForSeconds checkWait = new WaitForSeconds(0.1f);
            Vector3 center = transform.position;
            Vector3 size = Vector3.one * map.Size * overlapCubeLength;
            while (Physics.CheckBox(center, size / 2f, transform.rotation, overlapCheckMask))
            {
                doorCloseTimer = 0;
                cooldownTimer = 0;
                yield return checkWait;
            }

            currentDoorState = 0;
            doorAnimation.SetBool("Closed",true);
            doorUp.Play();

            map.SetGridAtPosition(gridLoc, Map.GridType.Wall);

            yield return null;
            yield return new WaitUntil(() => doorClosed);

            //leverAnimation.SetBool("Cooldown", true);

            doorCloseTimer = 0;
            cooldownTimer = 0;

        }
    }
    bool doorClosed = false;
    public void DoorCloseEnd()
    {
        doorClosed = true;
    }
    public IEnumerator OpenDoor() 
    {
        if (currentDoorState == 0) 
        {
            doorOpened = false;

            currentDoorState = 1;
            doorAnimation.SetBool("Closed",false);
            doorCloseTimer = 0;
            cooldownTimer = 0;
            doorDown.Play();
            ResetLever();

            yield return null;
            yield return new WaitUntil(() => doorOpened);

            map.SetGridAtPosition(gridLoc, Map.GridType.Air);

            foreach (Animator leverAnimator in leverAnimators)
            {
                leverAnimator.SetBool("Cooldown", true);
            }
        }
    }
    bool doorOpened = false;
    public void DoorOpenEnd()
    {
        doorOpened = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "Player" && canCloseDoor) 
        {
            SetLeverState();
            StartCoroutine(CloseDoor());
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
