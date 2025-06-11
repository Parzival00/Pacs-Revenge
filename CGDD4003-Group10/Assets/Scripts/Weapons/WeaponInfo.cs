using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewWeapon",menuName = "Weapon")]
public class WeaponInfo : ScriptableObject
{
    public Sprite gunIcon;
    public Sprite corruptedGunIcon;
    public string weaponName;
    public string weaponDescription;
    public float damageMultiplier;
    public float scoreMultiplier = 1;
    public float shootSpeed = 1;
    [Range(0, 10)] public int damageRating;
    [Range(0, 10)] public int speedRating;
    [Range(0, 10)] public int rangeRating;
}
