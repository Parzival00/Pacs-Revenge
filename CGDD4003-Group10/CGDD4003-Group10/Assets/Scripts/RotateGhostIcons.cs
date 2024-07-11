using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGhostIcons : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform ghostIcon;
    
    void Update()
    {
        //makes the ghosts minimap icon rotate with the player, so they are all upright on the minimap
        ghostIcon.rotation = Quaternion.Euler(90, player.transform.eulerAngles.y,0);
    }
}
