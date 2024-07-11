using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDoor : MonoBehaviour
{
    [SerializeField] Animator animator;

    int spawnQueue;

    private void Start()
    {
        spawnQueue = 0;
    }

    public void OpenSpawnDoor()
    {
        spawnQueue++;
        animator.SetTrigger("Open");
        animator.ResetTrigger("Close");
    }

    public void CloseSpawnDoor()
    {
        spawnQueue--;
        if (spawnQueue <= 0)
        {
            animator.SetTrigger("Close");
            animator.ResetTrigger("Open");
            spawnQueue = 0;
        }
    }
}
