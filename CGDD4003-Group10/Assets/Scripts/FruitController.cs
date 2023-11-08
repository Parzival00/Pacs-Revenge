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
        public int levelAppearance;
        public int pointsWorth;
    }

    public struct FruitInfo
    {
        public int pointsWorth;
        public PowerUpType powerUp;
    }

    public enum PowerUpType
    {
        Shield,
        Invisibility,
        Speed,
        EnhancedRadar,
        ExtraLife,
        None
    }

    [System.Serializable]
    public struct PowerUp
    {
        public PowerUpType type;
        [Range(0,100)] public int[] weights;
    }

    [Header("Fruit Spawn Alert Settings")]
    [SerializeField] string alertMessage = "Fruit Offering Detected";
    [SerializeField] string corruptionEndingMessage = "Final Offering Detected";
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

    [Header("Power Up Spawn Settings")]
    [SerializeField] PowerUp[] powerUps;

    private bool fruitActivated;
    private Fruit currentFruit;

    private bool firstFruitActivated;
    private bool secondFruitActivated;

    Coroutine fruitTimerCoroutine;
    Coroutine fruitSpawnAlertCoroutine;

    HUDMessenger hudMessenger;

    // Start is called before the first frame update
    void Start()
    {

        if (availableFruits != null)
        {
            currentFruit = availableFruits[0];
            for (int i = 0; i < availableFruits.Length; i++)
            {
                if(availableFruits[i].levelAppearance == Score.currentLevel)
                {
                    currentFruit = availableFruits[i];
                    break;
                }
            }

            minimapFruitSpriteRenderer.sprite = currentFruit.sprite;
            fruitSpriteRenderer.sprite = currentFruit.sprite;
        }
        hudMessenger = FindObjectOfType<HUDMessenger>();

        firstFruitActivated = false;
        secondFruitActivated = false;

        if (Score.corruptionEnding)
        {
            ActivateFruit();
        }
        else
        {
            DeactivateFruit();
        }
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
        }
    }

    /// <summary>
    /// Deactivates the fruit and returns the point worth of the current fruit
    /// </summary>
    public FruitInfo CollectFruit()
    {

        FruitInfo fruitInfo;
        fruitInfo.pointsWorth = 0;
        fruitInfo.powerUp = PowerUpType.None;

        if (fruitActivated)
        {
            fruitInfo.pointsWorth = currentFruit.pointsWorth;
            fruitInfo.powerUp = SelectPowerUp(Score.currentLevel);
        }

        DeactivateFruit();

        return fruitInfo;
    }

    /// <summary>
    /// Timer for how long the fruit can get collected
    /// </summary>
    IEnumerator FruitTimer()
    {
        WaitForSeconds timer = new WaitForSeconds(fruitTimer);
        yield return timer;

        if (fruitActivated)
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
        if (!fruitActivated && ((Score.pelletsCollected == firstFruitSpawnThreshold && !firstFruitActivated) || (Score.pelletsCollected == secondFruitSpawnThreshold && !secondFruitActivated)))
        {
            if (Score.pelletsCollected == firstFruitSpawnThreshold)
                firstFruitActivated = true;

            if (Score.pelletsCollected == secondFruitSpawnThreshold)
                secondFruitActivated = true;

            ActivateFruit();
        }
    }

    PowerUpType SelectPowerUp(int currentLevel)
    {
        //print("Current Level: " + currentLevel);
        int total = 0;
        for (int i = 0; i < powerUps.Length; i++)
        {
            if (currentLevel - 1 < powerUps[i].weights.Length)
            {
                total += powerUps[i].weights[currentLevel - 1];
            }
        }

        int value = Random.Range(0, total + 1);
        //print("Total: " + total);
        //print("Value: " + value);
        int min = 0;
        for (int i = 0; i < powerUps.Length; i++)
        {
            if (currentLevel < powerUps[i].weights.Length)
            {
                if (powerUps[i].weights[currentLevel - 1] > 0 && value > min && value <= min + powerUps[i].weights[currentLevel - 1])
                {
                    return powerUps[i].type;
                }
                min = min + powerUps[i].weights[currentLevel - 1];
            }
        }

        return PowerUpType.Shield;
    }

    IEnumerator FruitSpawnAlert()
    {
        float messageDelayTimer = Time.time + messageDelayTimerAmount;
        float lightningStrikeTimer = Time.time + intervalBtwLightningStrikes;
        float messageTimer = 0;

        bool alertMessegeSent = false;

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

            if(messageDelayTimer <= Time.time && !alertMessegeSent)
            {
                if (Score.corruptionEnding)
                {
                    hudMessenger?.Display(corruptionEndingMessage, alertVisibleTimerAmount);
                }
                else
                {
                    hudMessenger?.Display(alertMessage, alertVisibleTimerAmount);
                }
                alertMessegeSent = true;
                messageTimer = Time.time + alertVisibleTimerAmount;
            }

            yield return null;
        }

        while(messageTimer > Time.time)
            yield return null;

        lightningBeam?.Stop();
    }
}
