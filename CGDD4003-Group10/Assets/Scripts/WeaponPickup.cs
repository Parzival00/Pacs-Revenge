using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] bool canBeCorrupted;
    [SerializeField] int minDistForCorruption = 3;
    [SerializeField] SpriteRenderer inGameSprite;
    [SerializeField] SpriteRenderer miniMapGameSprite;
    public bool CanBeCorrupted { get => canBeCorrupted; }
    public bool isCorrupted;

    AudioSource playerMusic;
    Vector3 playerPosition;
    float originalVol;
    float rolloffStartDistance;

    void Start()
    {
        playerMusic = GameObject.Find("Music").GetComponent<AudioSource>();
        playerPosition = GameObject.Find("Player").transform.position;
        originalVol = playerMusic.volume;
        rolloffStartDistance = this.gameObject.GetComponent<AudioSource>().maxDistance * 1.25f;

        Map map = FindObjectOfType<Map>();

        Vector2Int weaponLoc = map.GetGridLocation(transform.position);

        int distToCorruption = int.MaxValue;

        //Check North
        Vector2Int currentLoc = weaponLoc + new Vector2Int(0,1);
        while (currentLoc.y < map.MapHeight)
        {
            if (map.SampleGrid(currentLoc) == Map.GridType.Wall)
            {
                break;
            }
            else if (map.SampleGrid(currentLoc) == Map.GridType.CorruptedWall)
            {
                distToCorruption = Mathf.Min(distToCorruption, Mathf.Abs(currentLoc.x - weaponLoc.x) + Mathf.Abs(currentLoc.y - weaponLoc.y));
                break;
            }
            currentLoc.y += 1;
        }
        //Check South
        currentLoc = weaponLoc + new Vector2Int(0, -1);
        while (currentLoc.y >= 0)
        {
            if (map.SampleGrid(currentLoc) == Map.GridType.Wall)
            {
                break;
            }
            else if (map.SampleGrid(currentLoc) == Map.GridType.CorruptedWall)
            {
                distToCorruption = Mathf.Min(distToCorruption, Mathf.Abs(currentLoc.x - weaponLoc.x) + Mathf.Abs(currentLoc.y - weaponLoc.y));
                break;
            }
            currentLoc.y -= 1;
        }
        //Check West
        currentLoc = weaponLoc + new Vector2Int(1, 0);
        while (currentLoc.x < map.MapWidth)
        {
            if (map.SampleGrid(currentLoc) == Map.GridType.Wall)
            {
                break;
            }
            else if (map.SampleGrid(currentLoc) == Map.GridType.CorruptedWall)
            {
                distToCorruption = Mathf.Min(distToCorruption, Mathf.Abs(currentLoc.x - weaponLoc.x) + Mathf.Abs(currentLoc.y - weaponLoc.y));
                break;
            }
            currentLoc.x += 1;
        }
        //Check East
        currentLoc = weaponLoc + new Vector2Int(-1, 0);
        while (currentLoc.x >= 0)
        {
            if (map.SampleGrid(currentLoc) == Map.GridType.Wall)
            {
                break;
            }
            else if (map.SampleGrid(currentLoc) == Map.GridType.CorruptedWall)
            {
                distToCorruption = Mathf.Min(distToCorruption, Mathf.Abs(currentLoc.x - weaponLoc.x) + Mathf.Abs(currentLoc.y - weaponLoc.y));
                break;
            }
            currentLoc.x -= 1;
        }

        if (distToCorruption <= minDistForCorruption)
        {
            canBeCorrupted = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(this.transform.position, playerPosition) <= rolloffStartDistance && !PlayerController.gunActivated && !Score.bossEnding)
        {
            playerMusic.volume = originalVol * Mathf.Log(Vector3.Distance(this.transform.position, playerPosition), rolloffStartDistance * 5);
        }
    }

    public void SetDisplaySprite(Sprite sprite)
    {
        inGameSprite.sprite = sprite;
        miniMapGameSprite.sprite = sprite;
    }
}
