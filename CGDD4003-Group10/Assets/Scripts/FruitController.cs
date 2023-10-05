using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitController : MonoBehaviour
{
    [System.Serializable]
    public struct Fruit
    {
        public string name;
        public Sprite sprite;
        public int pointsWorth;
    }

    [Header("Fruit Spawn Alert Settings")]
    [SerializeField] GameObject alertMessage;
    [SerializeField] ParticleSystem lightningBeam;
    [SerializeField] AudioSource lightningSoundSource;
    [SerializeField] float alertVisibleTimerAmount = 3;
    [SerializeField] float messageDelayTimerAmount = 2;
    [SerializeField] int numberOfLightningStrikes = 5;
    [SerializeField] float intervalBtwLightningStrikes = 0.5f;

    [Header("Sprite Renderers")]
    [SerializeField] SpriteRenderer minimapFruitSpriteRenderer;
    [SerializeField] SpriteRenderer fruitSpriteRenderer;

    [Header("Collection Collider")]
    [SerializeField] Collider fruitCollectionCollider;

    [Header("Fruit Settings")]
    [SerializeField] Fruit[] availableFruits;
    [SerializeField] int firstFruitSpawnThreshold = 70;
    [SerializeField] int secondFruitSpawnThreshold = 170;
    [SerializeField] float fruitTimer = 40;

    private bool fruitActivated;
    private Fruit currentFruit;

    Coroutine fruitTimerCoroutine;
    Coroutine fruitSpawnAlertCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        if (availableFruits != null)
        {
            currentFruit = availableFruits[Random.Range(0, availableFruits.Length)];

            minimapFruitSpriteRenderer.sprite = currentFruit.sprite;
            fruitSpriteRenderer.sprite = currentFruit.sprite;
        }

        if(alertMessage != null)
            alertMessage.SetActive(false);

        DeactivateFruit();
    }

    /// <summary>
    /// Activates the fruit by turning on the collider and sprites and starting the timer
    /// </summary>
    public void ActivateFruit()
    {
        if (!fruitActivated)
        {
            fruitSpriteRenderer.enabled = true;
            minimapFruitSpriteRenderer.enabled = true;

            fruitCollectionCollider.enabled = true;

            fruitActivated = true;

            fruitTimerCoroutine = StartCoroutine(FruitTimer());

            if (alertMessage != null)
                fruitSpawnAlertCoroutine = StartCoroutine(FruitSpawnAlert());
        }
    }

    /// <summary>
    /// Deactivates the fruit by turning off the collider and sprites and disabling the timer
    /// </summary>
    public void DeactivateFruit()
    {
        fruitSpriteRenderer.enabled = false;
        minimapFruitSpriteRenderer.enabled = false;

        fruitCollectionCollider.enabled = false;

        fruitActivated = false;

        if (fruitTimerCoroutine != null)
            StopCoroutine(fruitTimerCoroutine);

        if(fruitSpawnAlertCoroutine != null)
        {
            StopCoroutine(fruitSpawnAlertCoroutine);

            lightningBeam?.Stop();

            alertMessage.SetActive(false);
        }
    }

    /// <summary>
    /// Deactivates the fruit and returns the point worth of the current fruit
    /// </summary>
    public int CollectFruit()
    {
        int pointsToReturn = 0;

        if (fruitActivated)
        {
            pointsToReturn = currentFruit.pointsWorth;
        }

        DeactivateFruit();

        return pointsToReturn;
    }

    /// <summary>
    /// Timer for how long the fruit can get collected
    /// </summary>
    IEnumerator FruitTimer()
    {
        WaitForSeconds timer = new WaitForSeconds(fruitTimer);
        yield return timer;

        if(fruitActivated)
        {
            DeactivateFruit();
        }

    }

    public Fruit GetCurrentFruit()
    {
        return currentFruit;
    }

    // Update is called once per frame
    void Update()
    {
        if (!fruitActivated && (Score.pelletsCollected == firstFruitSpawnThreshold || Score.pelletsCollected == secondFruitSpawnThreshold))
        {
            ActivateFruit();
        }
    }

    IEnumerator FruitSpawnAlert()
    {
        float messageDelayTimer = Time.time + messageDelayTimerAmount;
        float lightningStrikeTimer = Time.time + intervalBtwLightningStrikes;
        float messageTimer = 0;

        int i = 0;
        while(i < numberOfLightningStrikes)
        {
            if(lightningStrikeTimer <= Time.time)
            {
                i++;

                lightningBeam?.Stop();
                lightningBeam?.Play();

                if (lightningSoundSource.clip != null)
                    lightningSoundSource.Play();

                lightningStrikeTimer = Time.time + intervalBtwLightningStrikes;
            }

            if(messageDelayTimer <= Time.time && !alertMessage.activeSelf)
            {
                messageTimer = Time.time + alertVisibleTimerAmount;
                alertMessage.SetActive(true);
            }

            yield return null;
        }

        while(messageTimer > Time.time)
            yield return null;

        lightningBeam?.Stop();

        alertMessage.SetActive(false);
    }
}
