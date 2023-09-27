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
    [SerializeField] float alertVisibleTimer = 3;

    [Header("Sprite Renderers")]
    [SerializeField] SpriteRenderer minimapFruitSpriteRenderer;
    [SerializeField] SpriteRenderer fruitSpriteRenderer;

    [Header("Collection Collider")]
    [SerializeField] Collider fruitCollectionCollider;

    [Header("Fruit Settings")]
    [SerializeField] Fruit[] availableFruits;
    [SerializeField] int firstFruitSpawnThreshold = 70;
    [SerializeField] int secondFruitSpawnThreshold = 170;
    [SerializeField] float fruitTimer = 15;

    private bool fruitActivated;
    private Fruit currentFruit;

    Coroutine fruitTimerCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        if (availableFruits != null)
        {
            currentFruit = availableFruits[0];

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
                StartCoroutine(FruitSpawnAlert());
        }
    }

    /// <summary>
    /// Deactivates the fruit by turning off the collider and sprites and disabling the timer
    /// </summary>
    void DeactivateFruit()
    {
        fruitSpriteRenderer.enabled = false;
        minimapFruitSpriteRenderer.enabled = false;

        fruitCollectionCollider.enabled = false;

        fruitActivated = false;

        if (fruitTimerCoroutine != null)
            StopCoroutine(fruitTimerCoroutine);
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
        alertMessage.SetActive(true);

        WaitForSeconds timer = new WaitForSeconds(alertVisibleTimer);

        yield return timer;

        alertMessage.SetActive(false);
    }
}
