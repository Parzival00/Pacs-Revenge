using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogController : MonoBehaviour
{
    [SerializeField] Material[] corpseMats;
    [SerializeField] Material[] ghostMats;
    [SerializeField] Material customPostProcess;

    [SerializeField] float defaultFogDistance = 19.5f;
    [SerializeField] float bossFogDistance = 35f;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < corpseMats.Length; i++)
        {
            corpseMats[i].SetFloat("_MaxDistance", Score.bossEnding ? bossFogDistance : defaultFogDistance);
        }
        for (int i = 0; i < ghostMats.Length; i++)
        {
            ghostMats[i].SetFloat("_MaxDistance", Score.bossEnding ? bossFogDistance : defaultFogDistance);
        }
        customPostProcess.SetFloat("_FogDistance", Score.bossEnding ? bossFogDistance : defaultFogDistance);
    }
}
