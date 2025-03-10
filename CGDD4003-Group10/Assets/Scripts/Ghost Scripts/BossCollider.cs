using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCollider : MonoBehaviour
{
    [SerializeField] int headID;
    [SerializeField] SpriteRenderer outline;

    public int HeadID { get => headID; }
    public Boss boss { get; private set; }

    private void Start()
    {
        boss = GetComponentInParent<Boss>();
    }

    public void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "Stun")
        {
            Destroy(c.gameObject);
        }
    }

    public TargetAreaCollider.TargetInfo OnTarget()
    {
        TargetAreaCollider.TargetInfo targetInfo;
        targetInfo.outline = outline;
        targetInfo.areaType = Ghost.TargetAreaType.Head;
        targetInfo.areaDifficulty = Ghost.TargetAreaDifficulty.Hard;
        return targetInfo;
    }
}
