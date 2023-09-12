using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevisedRadar : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float rotationSpeed;
    [SerializeField] float trackingRadius;
    void Update()
    {
        this.transform.position = new Vector3(player.position.x,3.306f,player.position.z);
        this.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, fwd, out hit, 10))
        {
            GameObject objectHit = hit.collider.gameObject;
            if (objectHit.tag == "MinimapObject")
            {
                
                objectHit.GetComponent<SpriteRenderer>().color = new Color(255, 255,255, 1);
            }
        }
            
    }
}
