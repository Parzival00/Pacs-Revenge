using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TargetAreaCollider : MonoBehaviour
{
    [SerializeField] Ghost ghost;
    [SerializeField] Ghost.TargetAreaType targetAreaType;
    //[SerializeField] SpriteRenderer spriteRenderer;

    public Ghost.HitInformation OnShot()
    {
        ghost.GotHit(targetAreaType);
        return ghost.GotHit(targetAreaType);
    }
}
