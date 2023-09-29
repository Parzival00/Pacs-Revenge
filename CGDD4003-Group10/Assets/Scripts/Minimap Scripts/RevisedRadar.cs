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
        //sets the location of the radar object to the players location rotates constantly
        this.transform.position = new Vector3(player.position.x,3.306f,player.position.z);
        this.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        //draw a raycast that returns all objects hit and stores them in an array
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, fwd, 10);
        if (hits.Length > 0)
        {
            foreach(RaycastHit hit in hits)
            {
                //if the object is a minimap object, then make in transparent
                GameObject objectHit = hit.collider.gameObject;
                if (objectHit.tag == "MinimapObject")
                {
                    Color color = objectHit.GetComponent<SpriteRenderer>().color;
                    objectHit.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 1);
                }
            }
        }           
    }
}