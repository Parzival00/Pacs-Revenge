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
}
