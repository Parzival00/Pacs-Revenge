using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapScript : MonoBehaviour
{
    public Transform player;
    public bool rotateWithPlayer;

    private void LateUpdate()
    {
        //follow player
        Vector3 newPos = player.position;
        newPos.y = transform.position.y;
        transform.position = newPos;

        //rotate with player
        if (rotateWithPlayer)
        {
            transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
        }
    }
}
