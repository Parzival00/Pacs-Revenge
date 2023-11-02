using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GlobalAudioController : MonoBehaviour
{
    [SerializeField] protected AudioMixer mix;
    // Start is called before the first frame update
    void Start()
    {
        ApplyAudioSettings();
    }

    public void ApplyAudioSettings()
    {
        print("Applied Audio Settings");

        if (PlayerPrefs.HasKey("MastVolume"))
            mix.SetFloat( "MasterVol", PlayerPrefs.GetFloat("MastVolume"));

        if (PlayerPrefs.HasKey("MusVolume"))
            mix.SetFloat("MusicVol", PlayerPrefs.GetFloat("MusVolume"));

        if (PlayerPrefs.HasKey("WVolume"))
            mix.SetFloat("WeaponVol", PlayerPrefs.GetFloat("WVolume"));

        if (PlayerPrefs.HasKey("EVolume"))
            mix.SetFloat("EnemyVol", PlayerPrefs.GetFloat("EVolume"));

        if (PlayerPrefs.HasKey("PlVolume"))
            mix.SetFloat("PlayerVol", PlayerPrefs.GetFloat("PlVolume"));

        if (PlayerPrefs.HasKey("PiVolume"))
            mix.SetFloat("PickupVol", PlayerPrefs.GetFloat("PiVolume"));

        if (PlayerPrefs.HasKey("MiscVolume"))
            mix.SetFloat("MiscVol", PlayerPrefs.GetFloat("MiscVolume"));
    }
}
