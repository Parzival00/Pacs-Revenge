using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl : MonoBehaviour
{
    [Header("Door Controls")]
    [SerializeField] GameObject currentDoor;
    [SerializeField] GameObject leverToDoor;

    [Header("Door/Lever Timers")]
    [SerializeField] float doorSpeed = 5f;
    [SerializeField] float leverResetTimer = 5f;
    [SerializeField] float doorResetTimer = 5f;

    int leverState = 1;
    int currentDoorState = 1;

    Vector3 doorOpenPosition, doorClosedPosition;

    void Start()
    {
        Vector3 doorOpenPosition = new Vector3(currentDoor.transform.position.x, -5f, currentDoor.transform.position.z);
        Vector3 doorCLosedPosition = currentDoor.transform.position;
    }


    void Update()
    {
        CheckDoorState();
    }
    public void ActivateDoor()
    {
        if (currentDoorState == 0) //If closed
        {
            currentDoorState = 1;
            currentDoor.transform.position = Vector3.MoveTowards(currentDoor.transform.position, doorClosedPosition, doorSpeed * Time.deltaTime);
            Invoke("ResetDoor", doorResetTimer);
        }
    }
    public int CheckLeverState()
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
    }
    public void CheckDoorState() 
    {
        if (currentDoorState == 1) 
        {
            Invoke("ResetDoor", doorResetTimer);
        }
    }
    public void SetLeverState()
    {
        /*if (leverState == 0)
        {
            leverState = 1;
        }*/
        if (leverState == 1) //Set Lever Off, then reset in certain amount of time
        {
            leverState = 0;
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
}
