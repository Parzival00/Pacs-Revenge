using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHead : MonoBehaviour
{
    [System.Serializable]
    public struct DifficultySettings
    {
        public int difficultyLevel;
        [Header("Head Settings")]
        public float maxHealth;
        public float shieldDeactivateLength;
        [Header("Point Settings")]
        public int killedPointWorth;
        public int hitPointWorth;
    }

    Boss boss;
    BossVFX bossVFX;

    [SerializeField] DifficultySettings[] difficultySettings;
    [SerializeField] int id;
    [SerializeField] bool debug = true;

    float health;

    DifficultySettings currentDifficultySettings;

    public float MaxHealth { get => currentDifficultySettings.maxHealth; }
    public float Health { get => health; }

    public bool dead { get; private set; }

    int ghostsKilled = 0;

    int newGhostsKilled = 0;

    bool shieldActive = true;
    public bool ShieldActive { get => shieldActive; }

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

        health = currentDifficultySettings.maxHealth;

        boss = GetComponentInParent<Boss>();
        bossVFX = GetComponentInParent<BossVFX>();

        bossVFX.SetPulseWeight(id, 0);
    }

    public void DeactivateShield()
    {
        if (deactivateCoroutine != null)
            StopCoroutine(deactivateCoroutine);

        deactivateCoroutine = StartCoroutine(ShieldDeactivate());
    }

    Coroutine deactivateCoroutine;
    IEnumerator ShieldDeactivate()
    {
        bossVFX.SetPulseWeight(id, 1);
        shieldActive = false;
        float deactivateTimer = 0;
        while(deactivateTimer < currentDifficultySettings.shieldDeactivateLength)
        {
            deactivateTimer += Time.deltaTime;
            yield return null;
        }
        shieldActive = !dead;
        ghostsKilled = newGhostsKilled;
        bossVFX.SetPulseWeight(id, 0);
    }

    public int TakeDamage(float amount, float patienceMultiplier, int newGhostsKilled)
    {
        int points = 0;
        if(shieldActive == false && health > 0)
        {
            health -= amount * Mathf.Max(1, 1 + (newGhostsKilled - ghostsKilled - 1) * (1 - patienceMultiplier));
            if (debug) print(name + " - Health: " + health + ", Max Health: " + currentDifficultySettings.maxHealth);
            if (health <= 0)
            {
                health = 0;
                dead = true;
                points = Mathf.RoundToInt((float)currentDifficultySettings.killedPointWorth * Mathf.Max(1, 1 + (newGhostsKilled - ghostsKilled - 1) * (1 - patienceMultiplier)));

                boss.HeadKilled(id);
            }
            else
            {
                points = Mathf.RoundToInt((float)currentDifficultySettings.hitPointWorth * Mathf.Max(1, 1 + (newGhostsKilled - ghostsKilled - 1) * (1 - patienceMultiplier)));
                this.newGhostsKilled = newGhostsKilled;
                if (newGhostsKilled - ghostsKilled <= 1)
                    ghostsKilled = newGhostsKilled;
            }
        }
        return points;
    }
}
