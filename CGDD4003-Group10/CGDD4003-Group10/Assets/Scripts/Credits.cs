using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioClip musicClip;

    [SerializeField] float startPause = 2;
    [SerializeField] float scrollLength = 30f;
    [SerializeField] Vector2 endPosition;

    RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        StartCoroutine(CreditScroll());
    }

    IEnumerator CreditScroll()
    {
        yield return new WaitForSeconds(startPause);

        musicSource.PlayOneShot(musicClip);

        Vector2 changeRate = (endPosition - rectTransform.anchoredPosition) / scrollLength;

        float timer = 0;
        while(timer < scrollLength)
        {
            rectTransform.anchoredPosition += changeRate * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = endPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
