using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TargetAreaCollider : MonoBehaviour
{
    public struct TargetInfo
    {
        public SpriteRenderer outline;
        public Ghost.TargetAreaType areaType;
        public Ghost.TargetAreaDifficulty areaDifficulty;
    }

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
        //ghost.GotHit(targetAreaType);
        return ghost.GotHit(targetAreaType);
    }

    public TargetInfo OnTarget()
    {
        TargetInfo targetInfo;
        targetInfo.outline = outline;
        targetInfo.areaType = targetAreaType;
        targetInfo.areaDifficulty = ghost.GetDifficulty(targetAreaType);
        return targetInfo;
    }
}
