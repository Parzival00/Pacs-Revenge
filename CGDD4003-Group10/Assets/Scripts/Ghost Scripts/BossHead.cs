using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHead : MonoBehaviour
{
    Boss boss;
    [Header("Head Settings")]
    [SerializeField] float maxHealth = 300;
    float health;
    [SerializeField] float shieldDeactivateLength = 10;
    [Header("Point Settings")]
    [SerializeField] int killedPointWorth;
    [SerializeField] int hitPointWorth;

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
            health -= amount * (newGhostsKilled - ghostsKilled - 1) * patienceMultiplier;
            if (health <= 0)
            {
                health = 0;
                dead = true;
                points = Mathf.RoundToInt((float)killedPointWorth * (newGhostsKilled - ghostsKilled - 1) * patienceMultiplier);
            }
            else
            {
                points = Mathf.RoundToInt((float)hitPointWorth * (newGhostsKilled - ghostsKilled - 1) * patienceMultiplier);
                ghostsKilled = newGhostsKilled;
            }
        }
        return points;
    }
}
