using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingDoor : MonoBehaviour
{
    public enum DoorState
    {
        Open, Closed
    }

    Map map;
    Animator animator;

    DoorState doorState;
    [SerializeField] Vector2 intervalTimes = new Vector2(8f, 12f);
    [SerializeField] LayerMask overlapCheckMask;
    [SerializeField] int overlapCubeLength = 3;
    [SerializeField] bool debug;
    [SerializeField] AudioSource movingDoorDown;
    [SerializeField] AudioSource movingDoorUp;

    Vector2Int gridLoc;

    float timer;
    float intervalTime;

    // Start is called before the first frame update
    void Start()
    {
        map = FindObjectOfType<Map>();
        animator = GetComponent<Animator>();

        gridLoc = map.GetGridLocation(transform.position);

        timer = 0;
        intervalTime = Random.Range(intervalTimes.x, intervalTimes.y);

        if(Random.Range(0, 2) >= 1)
        {
            StartCoroutine(OpenDoor());
        }
        else
        {
            StartCoroutine(CloseDoor());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(timer > intervalTime)
        {
            if(doorState == DoorState.Open)
            {
                StartCoroutine(CloseDoor());
            }
            else
            {
                StartCoroutine(OpenDoor());
            }
        }

        timer += Time.deltaTime;
    }

    bool doorOpened = false;
    public void DoorOpenEnd()
    {
        doorOpened = true;
    }
    IEnumerator OpenDoor()
    {
        timer = 0;

        doorOpened = false;
        animator.SetBool("Closed", false);
        movingDoorDown.Play();
        doorState = DoorState.Open;

        yield return null;

        yield return new WaitUntil(() => doorOpened);

        timer = 0;
        intervalTime = Random.Range(intervalTimes.x, intervalTimes.y);

        map.SetGridAtPosition(gridLoc, Map.GridType.Air);
    }

    bool doorClosed = false;
    public void DoorCloseEnd()
    {
        doorClosed = true;
    }
    IEnumerator CloseDoor()
    {
        doorClosed = false;

        timer = 0;

        yield return null;

        WaitForSeconds checkWait = new WaitForSeconds(0.1f);
        Vector3 center = transform.position;
        Vector3 size = Vector3.one * map.Size * overlapCubeLength;
        while (Physics.CheckBox(center, size / 2f, transform.rotation, overlapCheckMask))
        {
            timer = 0;
            yield return checkWait;
        }
        map.SetGridAtPosition(gridLoc, Map.GridType.Wall);
        animator.SetBool("Closed", true);
        movingDoorUp.Play();
        doorState = DoorState.Closed;

        yield return new WaitUntil(() => doorClosed);

        timer = 0;
        intervalTime = Random.Range(intervalTimes.x, intervalTimes.y);
    }

    private void OnDrawGizmosSelected()
    {
        if (debug)
        {
            Vector3 center = transform.position;
            Vector3 size = Vector3.one * 2 * overlapCubeLength;
            Gizmos.DrawWireCube(center, size);
        }
    }
}
