using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIconOut : MonoBehaviour
{
    SpriteRenderer sprite;

    [SerializeField] float fadeModifier;
    public bool enhanced;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        if(!enhanced)
            sprite.color = new Color(255, 255, 255, sprite.color.a - fadeModifier*Time.deltaTime);
    }
}
