using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRendering : MonoBehaviour
{
    [SerializeField] int downScaling = 4;
    [SerializeField] RenderTexture gunRenderTexture;
    [SerializeField] Material gunRenderMaterial;


    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        int resX = Mathf.CeilToInt(Screen.width / (float)downScaling);
        int resY = Mathf.CeilToInt(Screen.height / (float)downScaling);

        gunRenderTexture.width = resX;
        gunRenderTexture.height = resY;

        gunRenderMaterial.SetTexture("_Gun_Render_Texture", gunRenderTexture);
    }
}
