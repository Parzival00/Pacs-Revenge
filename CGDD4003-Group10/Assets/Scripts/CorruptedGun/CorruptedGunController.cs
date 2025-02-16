using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptedGunController : MonoBehaviour
{
    struct DifficultySetting
    {
        public AnimationCurve timerProbability;
        public float baseCorruptionTimer;
        public float levelTimerMultiplier;
    }
    [SerializeField] DifficultySetting[] difficultySettings;
    [SerializeField] WeaponInfo[] weaponInfos;
    [SerializeField] int overlapCubeSize = 4;
    [SerializeField] LayerMask overlapCheckMask;
    DifficultySetting currentSettings;

    WeaponPickup[] weaponPickups;

    float timer;
    float corruptionTime;

    // Start is called before the first frame update
    void Start()
    {
        currentSettings = difficultySettings[Mathf.Min(difficultySettings.Length - 1, Score.difficulty)];
        timer = 0;
        corruptionTime = GetRandomCorruptionTime();

        weaponPickups = FindObjectsOfType<WeaponPickup>();
    }

    public float GetRandomCorruptionTime()
    {
        return currentSettings.baseCorruptionTimer 
            * currentSettings.levelTimerMultiplier * Score.currentLevel 
            * currentSettings.timerProbability.Evaluate(Random.Range(0f, 1f));
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer >= corruptionTime)
        {
            timer = 0;
            if (!spawningCorruptGun) StartCoroutine(SpawnCorruptGun());
        }
    }

    bool spawningCorruptGun;
    IEnumerator SpawnCorruptGun()
    {
        spawningCorruptGun = true;
        WaitForSeconds wait = new WaitForSeconds(0.1f);

        while(spawningCorruptGun)
        {
            WeaponPickup weaponPickup = weaponPickups[Random.Range(0, weaponPickups.Length)];
            if (weaponPickup.CanBeCorrupted)
            {
                Vector3 center = weaponPickup.transform.position;
                Vector3 size = Vector3.one * 2 * overlapCubeSize;
                if (!Physics.CheckBox(center, size / 2f, transform.rotation, overlapCheckMask))
                {
                    weaponPickup.isCorrupted = true;
                    spawningCorruptGun = false;
                    timer = 0;
                }
            }
            yield return wait;
        }
    }
}
