using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureTentacle : MonoBehaviour
{
    Animator tentacleAnimator;
    Collider collider;
    AudioSource audio;

    CorruptedGun corruptedGun;

    [SerializeField] int babyHealth = 25;
    [SerializeField] int normalHealth = 50;
    [SerializeField] int nightmareHealth = 75;
    [SerializeField] AudioClip deathSound;

    int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        switch (Score.difficulty)
        {
            case 0:
                currentHealth = babyHealth;
                break;
            case 1:
                currentHealth = normalHealth;
                break;
            case 2:
                currentHealth = nightmareHealth;
                break;
        }

        tentacleAnimator = GetComponent<Animator>();
        collider = GetComponent<Collider>();
        corruptedGun = GetComponentInParent<CorruptedGun>();
        audio = GetComponent<AudioSource>();

        collider.enabled = false;
    }

    public void ActivateTentacle()
    {
        if(tentacleAnimator == null)
        {
            Init();
        }
        tentacleAnimator.SetTrigger("Reset");
        collider.enabled = true;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            tentacleAnimator.ResetTrigger("Reset");
            tentacleAnimator.SetTrigger("Death");
            audio.PlayOneShot(deathSound);
            collider.enabled = false;

            corruptedGun.TentacleKilled();
        }
    }
}
