using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineAnimator : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Texture[] textures;
    [SerializeField] int frameRate = 2;
    int animationStep = 0;
    float fpsCounter = 0;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        fpsCounter += Time.deltaTime;
        if(fpsCounter >= 1f / frameRate)
        {
            animationStep = (animationStep + 1) % textures.Length;
            lineRenderer.material.SetTexture("_MainTex", textures[animationStep]);
            fpsCounter -= 1f / frameRate;
        }
    }
}
