using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCollider : MonoBehaviour
{
    [SerializeField] int headID;
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
}
