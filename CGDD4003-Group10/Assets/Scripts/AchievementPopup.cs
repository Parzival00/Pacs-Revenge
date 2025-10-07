using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementPopup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PopupRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator PopupRoutine()
    {
        Vector3 topPosition = transform.position + new Vector3(0, 200, 0);

        Vector3 downPosition = transform.position;

        transform.position = topPosition;

        float t = 0;
        while (t < 1f)
        {
            transform.position = Vector3.Lerp(topPosition, downPosition, t);
            yield return null;
            t += Time.deltaTime;
        }

        transform.position = downPosition;

        yield return new WaitForSeconds(2.5f);

        t = 0;
        while (t < 1f)
        {
            transform.position = Vector3.Lerp(downPosition, topPosition, t);
            yield return null;
            t += Time.deltaTime;
        }

        transform.position = topPosition;

        Destroy(gameObject);
    }
}
