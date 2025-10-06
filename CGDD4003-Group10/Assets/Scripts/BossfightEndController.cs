using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings.WSA;

public class BossfightEndController : MonoBehaviour
{
    [SerializeField] Image whiteFlash;
    [SerializeField] float whiteFlashLength = 0.5f;
    [SerializeField] Image blackFade;
    [SerializeField] float fadeBlackLength = 1.5f;
    [SerializeField] float timeTillFadeBlack = 5f;
    [SerializeField] float timeTillWhiteFlash = 5f;

    [Header("Gameobject References")]
    [SerializeField] GameObject cleanBorderWalls;
    [SerializeField] GameObject borderWalls;
    [SerializeField] GameObject cleanPillars;
    [SerializeField] GameObject pillars;
    [SerializeField] GameObject cleanFloor;
    [SerializeField] GameObject floor;
    [SerializeField] GameObject pedastal;
    [SerializeField] GameObject portals;
    [SerializeField] GameObject decorations;

    public void StartEndSequence()
    {
        StartCoroutine(EndSequence());
    }

    IEnumerator EndSequence()
    {
        blackFade.gameObject.SetActive(false);

        yield return new WaitForSeconds(timeTillWhiteFlash);

        float timer = 0;
        Color color = whiteFlash.color;
        color.a = 0;
        whiteFlash.color = color;
        whiteFlash.gameObject.SetActive(true);
        while(timer <= whiteFlashLength)
        {
            color.a = timer / (whiteFlashLength);
            whiteFlash.color = color;
            timer += Time.deltaTime;
            yield return null;
        }
        color.a = 1;
        whiteFlash.color = color;

        borderWalls.SetActive(false);
        cleanBorderWalls.SetActive(true);
        pillars.SetActive(false);
        cleanPillars.SetActive(true);
        cleanFloor.SetActive(true);
        floor.SetActive(false);
        pedastal.SetActive(false);
        portals.SetActive(false);
        decorations.SetActive(false);

        GameObject[] corpses = GameObject.FindGameObjectsWithTag("Corpse");
        foreach(GameObject corpse in corpses)
        {
            Destroy(corpse);
        }

        yield return new WaitForSeconds(whiteFlashLength / 2f);

        while (timer <= whiteFlashLength)
        {
            color.a = 1 - timer / (whiteFlashLength);
            whiteFlash.color = color;
            timer += Time.deltaTime;
            yield return null;
        }
        color.a = 0;
        whiteFlash.color = color;
        whiteFlash.gameObject.SetActive(false);

        yield return new WaitForSeconds(timeTillFadeBlack);

        /*timer = 0;
        color = blackFade.color;
        color.a = 0;
        blackFade.color = color;
        blackFade.gameObject.SetActive(true);
        while (timer <= fadeBlackLength)
        {
            color.a = timer / fadeBlackLength;
            blackFade.color = color;
            timer += Time.deltaTime;
            yield return null;
        }
        color.a = 1;
        blackFade.color = color;*/

        Score score = FindObjectOfType<Score>();
        StartCoroutine(score.SceneEnd(false));

        //Score.BossEnd();
    }
}
