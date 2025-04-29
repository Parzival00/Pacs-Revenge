using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewAlignment : MonoBehaviour
{
    enum AlignmentAxis
    {
        forward, right, up
    }
    enum AlignmenMode
    {
        Target, CameraForward
    }
    [SerializeField] Transform target;
    [SerializeField] AlignmenMode mode;
    [SerializeField] AlignmentAxis axis;
    [SerializeField] AlignmentAxis upAxis;
    [SerializeField] bool negative = true;

    // Start is called before the first frame update
    void Start()
    {
        //if (target == null) target = GameObject.Find("Player").transform;
        if (target == null) target = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (mode == AlignmenMode.Target)
        {
            Vector3 forwardVec = transform.forward;
            switch (axis)
            {
                case AlignmentAxis.forward:
                    forwardVec = transform.forward;
                    break;
                case AlignmentAxis.right:
                    forwardVec = transform.right;
                    break;
                case AlignmentAxis.up:
                    forwardVec = transform.up;
                    break;
            }
            if (negative) forwardVec *= -1;

            Vector3 upVec = transform.up;
            switch (upAxis)
            {
                case AlignmentAxis.forward:
                    upVec = transform.forward;
                    break;
                case AlignmentAxis.right:
                    upVec = transform.right;
                    break;
                case AlignmentAxis.up:
                    upVec = transform.up;
                    break;
            }

            Vector3 perpVec = Vector3.Cross(forwardVec, upVec);
            Plane plane = new Plane(perpVec, transform.position);
            Vector3 closestPoint = plane.ClosestPointOnPlane(target.position);
            Vector3 dirToPoint = (closestPoint - transform.position).normalized;
            transform.rotation = Quaternion.FromToRotation(forwardVec, dirToPoint) * transform.rotation;
        }
        else if (mode == AlignmenMode.CameraForward)
        {
            Vector3 forwardVec = transform.forward;
            switch (axis)
            {
                case AlignmentAxis.forward:
                    forwardVec = transform.forward;
                    break;
                case AlignmentAxis.right:
                    forwardVec = transform.right;
                    break;
                case AlignmentAxis.up:
                    forwardVec = transform.up;
                    break;
            }
            if (negative) forwardVec *= -1;

            Vector3 thisPosition = transform.position;
            Vector3 cameraPosition = target.position;

            Vector3 lookAtPosition = new Vector3(cameraPosition.x, cameraPosition.y, thisPosition.z);
            Vector3 lookDir = (lookAtPosition - transform.position).normalized;

            //transform.LookAt(lookAtPosition);

            transform.rotation = Quaternion.FromToRotation(forwardVec, lookDir) * transform.rotation;
        }
    }
}
