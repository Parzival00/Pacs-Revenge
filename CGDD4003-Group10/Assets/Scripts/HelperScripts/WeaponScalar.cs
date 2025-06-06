using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScalar : MonoBehaviour
{
    [SerializeField] Vector2 offset;

    Vector3 defaultScale;
    Vector3 defaultPosition;
    // Start is called before the first frame update
    void Start()
    {
        defaultScale = transform.localScale;
        defaultPosition = transform.localPosition;
        MainMenuManager.OnOptionsChanged += ScaleWeapon;

        ScaleWeapon();
    }

    public void ScaleWeapon()
    {
        float fov = PlayerPrefs.GetFloat("FOV", 70);
        transform.localScale = defaultScale + Vector3.one * 0.035f * ((fov - 70) / 10f);
        transform.localPosition = defaultPosition + (Vector3)offset * ((fov - 70) / (120 - 70));
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
