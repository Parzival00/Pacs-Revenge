using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerIndicatorController : MonoBehaviour
{
    [System.Serializable]
    public struct IndicatorSettings
    {
        public float distanceThreshold;
        public bool indicatorActive;
    }

    [SerializeField] Material dangerIndicatorMat;
    [SerializeField] IndicatorSettings[] indicatorSettings;
    [SerializeField] float alertRate;
    [SerializeField] float dangerIndicatorAngleOffset = 45;

    float distanceThreshold;
    bool indicatorActive;

    float alertTimer;

    Ghost[] ghosts;
    Ghost closestGhost;
    float clostestGhostDistSqr;

    float distanceThresholdSqr;

    public IndicatorSettings GetIndicatorSettings(int index)
    {
        if (index < indicatorSettings.Length)
            return indicatorSettings[index];
        else
            return new IndicatorSettings()
            {
                distanceThreshold = 5,
                indicatorActive = true
            };
    }

    // Start is called before the first frame update
    void Start()
    {
        ghosts = FindObjectsOfType<Ghost>();
        closestGhost = ghosts[0];

        dangerIndicatorMat.SetFloat("_IndicatorActive", 0);

        alertTimer = 1 / alertRate;

        IndicatorSettings indicatorSetting;

        switch (Score.difficulty)
        {
            case 0:
                indicatorSetting = GetIndicatorSettings(0);
                distanceThreshold = indicatorSetting.distanceThreshold;
                indicatorActive = indicatorSetting.indicatorActive;
                break;
            case 1:
                indicatorSetting = GetIndicatorSettings(1);
                distanceThreshold = indicatorSetting.distanceThreshold;
                indicatorActive = indicatorSetting.indicatorActive;
                break;
            case 2:
                indicatorSetting = GetIndicatorSettings(2);
                distanceThreshold = indicatorSetting.distanceThreshold;
                indicatorActive = indicatorSetting.indicatorActive;
                break;
        }

        distanceThresholdSqr = distanceThreshold * distanceThreshold;
    }

    public float Remap(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        return newMin + (value - oldMin) * (newMax - newMin) / (oldMax - oldMin);
    }

    // Update is called once per frame
    void Update()
    {
        if (indicatorActive)
        {
            if (alertTimer <= 0)
            {
                clostestGhostDistSqr = float.MaxValue;
                for (int i = 0; i < ghosts.Length; i++)
                {
                    float ghostDistSqr = (new Vector3(transform.position.x, 0, transform.position.z)
                        - new Vector3(ghosts[i].transform.position.x, 0, ghosts[i].transform.position.z)).sqrMagnitude;

                    if (ghostDistSqr <= clostestGhostDistSqr && ghosts[i].CurrentMode != Ghost.Mode.Respawn)
                    {
                        closestGhost = ghosts[i];
                        clostestGhostDistSqr = ghostDistSqr;
                    }
                }

                if (clostestGhostDistSqr < distanceThresholdSqr)
                {
                    Vector2 dirToClosestGhost = (new Vector2(closestGhost.transform.position.x, closestGhost.transform.position.z) -
                        new Vector2(transform.position.x, transform.position.z)).normalized;
                    float angleToClosestPellet = 360 + Vector2.SignedAngle(new Vector2(transform.forward.x, transform.forward.z), dirToClosestGhost);

                    float indicatorAngle = 690 - angleToClosestPellet + dangerIndicatorAngleOffset;

                    dangerIndicatorMat.SetFloat("_IndicatorAngle", (Mathf.Deg2Rad * (indicatorAngle + 80)) / (Mathf.PI * 2));
                    dangerIndicatorMat.SetFloat("_IndicatorActive", Mathf.Lerp(0, 1, Remap(Mathf.Clamp01(0.5f - clostestGhostDistSqr / distanceThresholdSqr), 0f, 0.5f, 0f, 1f)));
                }
                else
                {
                    dangerIndicatorMat.SetFloat("_IndicatorActive", 0);
                }

                alertTimer = 1 / alertRate;
            }
            else
            {
                alertTimer -= Time.deltaTime;
            }
        }
    }
}
