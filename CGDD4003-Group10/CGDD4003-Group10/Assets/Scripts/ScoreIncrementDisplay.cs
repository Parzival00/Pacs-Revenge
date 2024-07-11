using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreIncrementDisplay : MonoBehaviour
{
    [SerializeField] TextMeshPro scoreText;
    [SerializeField] Color[] flashColors = new Color[2];
    [SerializeField] float flashRate = 6f;
    [SerializeField] float displayLength = 1.5f;
    [SerializeField] float riseSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Display());
    }

    public void SetText(string text)
    {
        scoreText.text = text;
    }

    IEnumerator Display()
    {
        float timer = 0;
        float flashTimer = 0;

        float timeBtwFlashes = 1 / (flashRate);

        int i = 0;
        scoreText.color = flashColors[i];

        while (timer < displayLength)
        {
            if(flashTimer > timeBtwFlashes)
            {
                i = (i + 1) % flashColors.Length;
                Color color = flashColors[i];
                if (timer > displayLength / 2)
                {
                    float a = 1 - (timer - (displayLength / 2)) / (displayLength / 2);
                    color.a = a;
                }
                scoreText.color = color;
                flashTimer = 0;
            }
            transform.position += transform.up * riseSpeed * Time.deltaTime;
            yield return null;
            timer += Time.deltaTime;
            flashTimer += Time.deltaTime;
        }

        Destroy(gameObject);
    }

}
