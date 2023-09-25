using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inky : Ghost
{
    //Temporary Movement to player
    protected override void Chase()
    {
        System.Random distanceForward = new System.Random();
        Vector2Int rand = new Vector2Int(distanceForward.Next(5),distanceForward.Next(5));

        Vector2Int playerGridPosition = map.CheckEdgePositions(transform.position);

        targetGridPosition = playerGridPosition + rand;

        Move();

        lastTargetGridPosition = targetGridPosition;

        //Play Sound
        source.loop = true;
        source.clip = chaseSound;

        if (!source.isPlaying || source.clip != chaseSound)
        {
            source.Play();
        }
    }

    ///Summary
    ///Take Blinky distance to player || do rand in between those position to set inky
    ///Summary
}
