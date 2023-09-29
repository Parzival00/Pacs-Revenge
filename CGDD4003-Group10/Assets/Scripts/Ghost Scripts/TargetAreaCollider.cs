using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TargetAreaCollider : MonoBehaviour
{
    [SerializeField] Ghost ghost;
    [SerializeField] Ghost.TargetAreaType targetAreaType;
    [SerializeField] SpriteRenderer outline;

    private void Start()
    {
        if (ghost == null)
            ghost = transform.root.GetComponent<Ghost>();
    }

    public Ghost.HitInformation OnShot()
    {
        ghost.GotHit(targetAreaType);
        return ghost.GotHit(targetAreaType);
    }

    public SpriteRenderer OnTarget()
    {
        return outline;
    }
}
