using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialController : MonoBehaviour
{
    [SerializeField] GameObject holdToShootPrompt;
    [SerializeField] GameObject releasePrompt;
    [SerializeField] GameObject overheatPrompt;
    [SerializeField] GameObject ghostNearbyPrompt;

    public void ToggleShootPrompt(bool toggle)
    {
        Time.timeScale = toggle ? 0 : 1;
        holdToShootPrompt.SetActive(toggle);
    }
    public void ToggleReleasePrompt(bool toggle)
    {
        Time.timeScale = toggle ? 0 : 1;
        releasePrompt.SetActive(toggle);
    }
    public void ToggleOverheatPrompt(bool toggle)
    {
        Time.timeScale = toggle ? 0 : 1;
        overheatPrompt.SetActive(toggle);
    }
    public void ToggleGhostNearbyPrompt(bool toggle)
    {
        ghostNearbyPrompt.SetActive(toggle);
    }

    IEnumerator GhostNearby()
    {
        yield return null;
    }
}
