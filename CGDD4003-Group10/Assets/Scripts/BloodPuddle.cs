using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodPuddle : MonoBehaviour
{
    [SerializeField] Sprite bloodStage1;
    [SerializeField] Sprite bloodStage2;
    [SerializeField] float timeTillStage2 = 0.2f;

    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = bloodStage1;
        Invoke("SetBloodStage2", timeTillStage2);
    }

    void SetBloodStage2()
    {
        spriteRenderer.sprite = bloodStage2;
    }
}
