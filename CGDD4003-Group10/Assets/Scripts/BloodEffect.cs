using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SpriteRenderer))]
public class BloodEffect : MonoBehaviour
{
    [SerializeField] Sprite bloodStage1;
    [SerializeField] Sprite bloodStage2;
    [SerializeField] Sprite bloodStage3;
    [SerializeField] float timeTillStage2 = 0.2f;
    [SerializeField] float lifeTime = 10;
    [SerializeField] Vector3 startVelocity = new Vector3(0, 0.5f, 0);

    Rigidbody rb;
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        rb.velocity = startVelocity;

        rb.useGravity = true;

        spriteRenderer.sprite = bloodStage1;
        Invoke("SetBloodStage2", timeTillStage2);
    }

    void SetBloodStage2()
    {
        spriteRenderer.sprite = bloodStage2;
    }

    private void OnCollisionEnter(Collision collision)
    {
        rb.useGravity = false;
        //spriteRenderer.sprite = bloodStage3;
        Destroy(gameObject, lifeTime);
    }
}
