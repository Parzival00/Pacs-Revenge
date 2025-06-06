using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] Transform destination;
    [SerializeField] Vector3 destinationOffset;
    [SerializeField] AudioSource teleportSound;

    Map map;

    private void Start()
    {
        if (map == null)
        {
            map = GameObject.FindObjectOfType<Map>();
        }
    }
    private void Update()
    {
        if (!Score.bossEnding && this.tag.Equals("CorruptedTeleport"))
        {
            //this.transform.position = new Vector3(27f, 0f, 5f);
        }
        if (this.tag.Equals("RightEdge"))
        {
            //this.transform.position = new Vector3(-2f, 0f, -14f);
        }
        if (this.tag.Equals("LeftEdge"))
        {
           // this.transform.position = new Vector3(-2f, 0f, 12f);
        }
    }

    //Teleports the enemy and player to destination (used to make the map feel infinite)
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            Ghost ghost = other.gameObject.GetComponent<Ghost>();

            if (ghost != null)
            {
                //gets random destination if ghost uses the corrupted portal
                if (this.tag.Equals("CorruptedTeleport"))
                {
                    //ghost.SetPosition(GetValidSpace());
                    return;
                }
                else
                {
                    ghost.TeleportGhost(new Vector3(destination.position.x, other.transform.position.y, destination.position.z), destinationOffset);
                }
                teleportSound.spatialBlend = 1.0f;
                teleportSound.Play();
            }
        }
        else if (other.tag == "Player")
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();

            if (player != null)
            {
                //gets random destination if player uses the corrupted portal
                if (this.tag.Equals("CorruptedTeleport"))
                    player.SetPosition(GetValidSpace());
                else
                    player.SetPosition(new Vector3(destination.position.x, other.transform.position.y, destination.position.z) + destinationOffset);
                teleportSound.spatialBlend = 0f;
                teleportSound.Play();
            }
                
        }
    }

    private Vector3 GetValidSpace()
    {
        Vector3 randomDestination = new Vector3(Random.Range(-25, 25), 0.52f, Random.Range(-20, 20));
        Vector2 gridLocation = map.GetGridLocation(randomDestination);

        if (map.map[(int)gridLocation.x, (int)gridLocation.y].Equals(Map.GridType.Barrier) || map.map[(int)gridLocation.x, (int)gridLocation.y].Equals(Map.GridType.Wall))
        {
            return GetValidSpace();
        }
        else return randomDestination;
    }

}
