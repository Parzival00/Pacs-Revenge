using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallHitEffect : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float riseSpeed = 0.5f;
    [SerializeField] Sprite[] frames;
    [SerializeField] float[] stepTimes;
    [SerializeField] float[] alphas;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WallHit());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * riseSpeed * Time.deltaTime;
    }

    IEnumerator WallHit()
    {
        spriteRenderer.sprite = frames[0];
        Color color = spriteRenderer.color;
        color.a = alphas[0];
        spriteRenderer.color = color;

        for (int i = 1; i < frames.Length; i++)
        {
            yield return new WaitForSeconds(stepTimes[i - 1]);
            spriteRenderer.sprite = frames[i];
            color = spriteRenderer.color;
            color.a = alphas[i];
            spriteRenderer.color = color;
        }

        spriteRenderer.sprite = frames[frames.Length - 1];
        color = spriteRenderer.color;
        color.a = alphas[alphas.Length - 1];
        spriteRenderer.color = color;

        float alpha = color.a;
        float step = alpha / stepTimes[stepTimes.Length - 1];
        while (alpha > 0)
        {
            alpha -= step * Time.deltaTime;
            color.a = alpha;
            spriteRenderer.color = color;
            yield return null;
        }

        color.a = 0;
        spriteRenderer.color = color;

        yield return null;

        Destroy(gameObject);
    }
}
