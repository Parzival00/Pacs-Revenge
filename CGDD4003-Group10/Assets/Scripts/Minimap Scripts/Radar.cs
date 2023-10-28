using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    [SerializeField] Camera minimapCam;
    [SerializeField] bool rotateWithPlayer = true;
    [SerializeField] Transform radarObject;
    [SerializeField] GameObject radarLine;
    [SerializeField] GameObject indicator;
    [SerializeField] Transform player;
    [SerializeField] float rotationSpeed;
    [SerializeField] float trackingRadius;
    [SerializeField] float radarLineAngleOffset = 95;
    [SerializeField] float radarIndicatorAngleOffset = 45;
    [SerializeField] Material radarMaterial;
    [SerializeField] Material radarIndicatorMat;

    float degreeRotations = 0;

    GameObject closestPellet;
    private void Start()
    {
        degreeRotations = transform.localEulerAngles.y + 360;

        if(radarLine && minimapCam)
            radarLine.transform.localScale = Vector3.one * minimapCam.orthographicSize * 2;

        if (indicator && minimapCam)
            indicator.transform.localScale = Vector3.one * minimapCam.orthographicSize * 2;
    }

    void Update()
    {

        //sets the location of the radar object to the players location rotates constantly
        radarObject.position = new Vector3(player.position.x,3.306f,player.position.z);
        radarObject.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        //have camera follow player
        Vector3 newPos = player.position;
        newPos.y = minimapCam.transform.position.y;
        minimapCam.transform.position = newPos;

        //have camera rotate with player
        if (rotateWithPlayer)
        {
            minimapCam.transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
        }

        degreeRotations += rotationSpeed * Time.deltaTime;

        Vector3 fwd = radarObject.forward;//radarObject.TransformDirection(Vector3.forward);

        radarMaterial.SetFloat("_LineAngle", Mathf.Deg2Rad * (degreeRotations - minimapCam.transform.eulerAngles.y + radarLineAngleOffset) / (Mathf.PI * 2));

        //draw a raycast that returns all objects hit and stores them in an array
        RaycastHit[] hits;
        hits = Physics.RaycastAll(radarObject.position, fwd, trackingRadius);
        if (hits.Length > 0)
        {
            foreach(RaycastHit hit in hits)
            {
                //if the object is a minimap object, then make in transparent
                GameObject objectHit = hit.collider.gameObject;

                if (objectHit.tag == "MinimapPellet" || objectHit.tag == "MinimapFruit" || objectHit.tag == "MinimapObject")
                {
                    SpriteRenderer hitSprite = objectHit.GetComponent<SpriteRenderer>();
                    Color color = hitSprite.color;
                    hitSprite.color = new Color(color.r, color.g, color.b, 1);

                    if (objectHit.tag == "MinimapPellet" && (closestPellet == null || closestPellet.transform.root.gameObject.activeSelf == false || hitSprite.enabled == false ||
                        hit.distance < Vector2.Distance(new Vector2(closestPellet.transform.position.x, closestPellet.transform.position.z), new Vector2(player.position.x, player.position.z))))
                    {
                        closestPellet = objectHit;
                    }
                }
            }
        }   
        
        if(Score.indicatorActive)
        {
            RadarIndicator();
        } 
        else
        {
            radarIndicatorMat.SetFloat("_IndicatorActive", 0);
        }
    }

    void RadarIndicator()
    {
        if (closestPellet == null || closestPellet.transform.root.gameObject.activeSelf == false)
            return;

        Vector2 dirToClosestPellet = (new Vector2(closestPellet.transform.position.x, closestPellet.transform.position.z) - new Vector2(player.position.x, player.position.z)).normalized;

        print("Closest Pellet: " + closestPellet.transform.position);
        float angleToClosestPellet = 360 + Vector2.SignedAngle(new Vector2(player.forward.x, player.forward.z), dirToClosestPellet);
        //print("Angle to closest pellet: " + (angleToClosestPellet + radarIndicatorAngleOffset - 360));
        radarIndicatorMat.SetFloat("_IndicatorAngle", (Mathf.Deg2Rad * (720 - angleToClosestPellet + radarIndicatorAngleOffset + 90)) / (Mathf.PI * 2));
        radarIndicatorMat.SetFloat("_IndicatorActive", 1);

    }
}
