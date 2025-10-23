using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct DifficultySettings
    {
        public int difficultyLevel;
        public float normalSpawnInterval;
        public float enrageSpawnInterval;
    }

    struct GhostChoice
    {
        public int ghostID;
        public float weight;
        public GhostChoice(int id, float weight)
        {
            ghostID = id;
            this.weight = weight;
        }
    }

    GhostChoice[] ghostChoices;

    [SerializeField] DifficultySettings[] difficultySettings;
    [SerializeField] Boss boss;
    [SerializeField] Map map;
    [SerializeField] GameObject lightningEffect;
    [SerializeField] GameObject[] ghosts;
    [SerializeField] GameObject endGhost;
    [SerializeField] float spawnRadius = 10f;
    [SerializeField] float bossTimerEndInterval = 0.1f;

    public bool spawnGhosts = true;

    DifficultySettings currentDifficultySettings;

    float spawnTimer;

    List<Ghost> spawnedGhosts;

    // Start is called before the first frame update
    void Start()
    {
        if (Score.difficulty < difficultySettings.Length)
        {
            currentDifficultySettings = difficultySettings[Score.difficulty];
        }
        else
        {
            currentDifficultySettings = difficultySettings[0];
        }

        ghostChoices = new GhostChoice[4];
        ghostChoices[0] = new GhostChoice(0, 1);
        ghostChoices[1] = new GhostChoice(1, 1);
        ghostChoices[2] = new GhostChoice(2, 1);
        ghostChoices[3] = new GhostChoice(3, 0);

        spawnTimer = currentDifficultySettings.normalSpawnInterval;

        spawnedGhosts = new List<Ghost>();
    }

    public void ChangeGhostWeight(int id, float newWeight)
    {
        ghostChoices[id].weight = newWeight;

        if(id == 3 && newWeight <= 0)
        {
            KillAllGhosts();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnGhosts)
        {
            spawnTimer -= Time.deltaTime;

            if(spawnTimer <= 0)
            {
                if(spawnedGhosts.Count < 50)
                    StartCoroutine(SpawnGhost());

                spawnTimer = boss.damage == 7 ? currentDifficultySettings.enrageSpawnInterval : currentDifficultySettings.normalSpawnInterval;

                if (Score.bossTimerEnded)
                {
                    spawnTimer = bossTimerEndInterval;
                }
            }
        }
    }

    IEnumerator SpawnGhost()
    {
        int currentGhost = 0;

        //Weighted random choice
        float totalWeight = 0;
        for (int i = 0; i < ghostChoices.Length; i++)
        {
            totalWeight += ghostChoices[i].weight;
        }
        float rand = Random.Range(0.1f, totalWeight);
        for (int i = 0; i < ghostChoices.Length; i++)
        {
            if (rand <= ghostChoices[i].weight)
            {
                currentGhost = ghostChoices[i].ghostID;
                break;
            }
            rand -= ghostChoices[i].weight;
        }

        Vector3 spawnPos = Vector3.zero;

        //spawnPos = map.GetWorldFromGrid(map.openMapLocations[Random.Range(0, map.openMapLocations.Length)]);

        bool goodSelection = false;
        while(!goodSelection)
        {
            Vector2 offset = Random.insideUnitCircle * spawnRadius;
            Vector3 testPos = new Vector3(boss.transform.position.x, transform.position.y, boss.transform.position.z) + new Vector3(offset.x, 0, offset.y);

            goodSelection = map.SampleGrid(testPos) == Map.GridType.Air;

            if (goodSelection) spawnPos = testPos;
        }

        Instantiate(lightningEffect, spawnPos, Quaternion.identity);

        yield return new WaitForSeconds(0.38f);

        if (spawnGhosts)
        {
            if (!Score.bossTimerEnded)
            {
                GameObject ghostObj = Instantiate(ghosts[currentGhost], spawnPos, Quaternion.identity);
                spawnedGhosts.Add(ghostObj.GetComponent<Ghost>());
            }
            else
            {
                GameObject ghostObj = Instantiate(endGhost, spawnPos, Quaternion.identity);
                spawnedGhosts.Add(ghostObj.GetComponent<Ghost>());
            }
        }
    }

    public void ResetGhosts()
    {
        foreach(Ghost ghost in spawnedGhosts)
        {
            if(ghost != null)
                Destroy(ghost.gameObject);
        }

        spawnedGhosts = new List<Ghost>();

        spawnGhosts = true;
    }

    public void KillAllGhosts()
    {
        foreach (Ghost ghost in spawnedGhosts)
        {
            if (ghost != null)
            {
                Ghost.HitInformation hitInfo = ghost.GotHit(Ghost.TargetAreaType.Head, 5);
                Score.AddToScore(Color.white, hitInfo.pointWorth);
            }
        }

        spawnGhosts = false;
    }
}
