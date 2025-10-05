using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [Header("Pickup Spawn Settings")]
    [SerializeField] float spawnInterval;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] WeaponInfo[] weaponInfos;
    GameObject weaponPickup;
    protected bool startedSpawning = false;

    WeaponInfo currentWeaponInfo;

    // Start is called before the first frame update
    void Start()
    {
        weaponPickup = Resources.Load<GameObject>("Prefabs/GunPickup");

        currentWeaponInfo = weaponInfos[PlayerPrefs.GetInt("Weapon")];
    }

    // Update is called once per frame
    void Update()
    {
        spawn();
    }
    void spawn()
    {
        if (!startedSpawning)
        {
            spawnWeapons = StartCoroutine(spawnWeapon());
        }
    }
    Coroutine spawnWeapons;
    IEnumerator spawnWeapon()
    {
        startedSpawning = true;
        WaitForSeconds spawnTimer = new WaitForSeconds(spawnInterval);
        yield return spawnTimer;

        Transform thisSpawnLoc = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject w = Instantiate(weaponPickup, thisSpawnLoc, true);
        w.transform.position = thisSpawnLoc.position;
        w.GetComponentInChildren<FadeIconOut>().enhanced = true;

        WeaponPickup wp= w.GetComponent<WeaponPickup>();
        if (wp != null)
        {
            wp.SetDisplaySprite(currentWeaponInfo.gunIcon);
        }
    }

    public void Reset()
    {
        print("Spawner Reset");
        startedSpawning = false;
    }
}
