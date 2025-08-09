using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScalar : MonoBehaviour
{
    [SerializeField] Vector2 offset;
    [SerializeField] float scaleOffset = 10;

    Vector3 defaultScale;
    Vector3 defaultPosition;
    // Start is called before the first frame update
    void Start()
    {
        defaultScale = transform.localScale;
        defaultPosition = transform.localPosition;

        ScaleWeapon();
    }

    public void ScaleWeapon()
    {
        float fov = PlayerPrefs.GetFloat("FOV", 70);
        transform.localScale = defaultScale + Vector3.one * 0.035f * ((fov - 70) / scaleOffset);
        transform.localPosition = defaultPosition + (Vector3)offset * ((fov - 70) / (120 - 70));
        print($"Scaling weapon: {name}");
    }

    private void OnEnable()
    {
        MainMenuManager.OnOptionsChanged += ScaleWeapon;
        if(defaultScale.x > 0)
            ScaleWeapon();
    }

    public void OnDestroy()
    {
        MainMenuManager.OnOptionsChanged -= ScaleWeapon;
    }
    public void OnDisable()
    {
        MainMenuManager.OnOptionsChanged -= ScaleWeapon;
    }
}
