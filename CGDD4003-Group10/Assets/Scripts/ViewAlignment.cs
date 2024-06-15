using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewAlignment : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] bool forward = false;
    [SerializeField] bool negative = true;

    // Start is called before the first frame update
    void Start()
    {
        if (target == null) target = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forwardVec = transform.forward;
        if (!this.forward) forwardVec = transform.right;
        if (negative) forwardVec *= -1;

        Vector3 perpVec = Vector3.Cross(forwardVec, transform.up);
        Plane plane = new Plane(perpVec, transform.position);
        Vector3 closestPoint = plane.ClosestPointOnPlane(target.position);
        Vector3 dirToPoint = (closestPoint - transform.position).normalized;
        transform.rotation = Quaternion.FromToRotation(forwardVec, dirToPoint) * transform.rotation;
    }
}
