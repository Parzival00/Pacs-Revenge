using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHead : MonoBehaviour
{
    Boss boss;

    [Header("Head Settings")]
    [SerializeField] int id;
    [SerializeField] float maxHealth = 300;
    float health;
    [SerializeField] float shieldDeactivateLength = 10;
    [Header("Point Settings")]
    [SerializeField] int killedPointWorth;
    [SerializeField] int hitPointWorth;
    [SerializeField] bool debug = true;

    public float MaxHealth { get => maxHealth; }
    public float Health { get => health; }

    public bool dead { get; private set; }

    int ghostsKilled = 0;

    bool shieldActive;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;

        boss = GetComponentInParent<Boss>();
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
        shieldActive = false;
        float deactivateTimer = 0;
        while(deactivateTimer < shieldDeactivateLength)
        {
            deactivateTimer += Time.deltaTime;
            yield return null;
        }
        shieldActive = !dead;
    }

    public int TakeDamage(float amount, float patienceMultiplier, int newGhostsKilled)
    {
        int points = 0;
        if(shieldActive == false && health > 0)
        {
            health -= amount * Mathf.Max(1, (newGhostsKilled - ghostsKilled - 1)) * patienceMultiplier;
            if(debug) print(name + " - Health: " + health + ", Max Health: " + maxHealth);
            if (health <= 0)
            {
                health = 0;
                dead = true;
                points = Mathf.RoundToInt((float)killedPointWorth * Mathf.Max(1, (newGhostsKilled - ghostsKilled - 1)) * patienceMultiplier);

                boss.HeadKilled(id);
            }
            else
            {
                points = Mathf.RoundToInt((float)hitPointWorth * Mathf.Max(1, (newGhostsKilled - ghostsKilled - 1)) * patienceMultiplier);
                ghostsKilled = newGhostsKilled;
            }
        }
        return points;
    }
}
