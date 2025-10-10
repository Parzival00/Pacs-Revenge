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
            mix.SetFloat("MasterVol", PlayerPrefs.GetFloat("MastVolume"));
        else
            mix.SetFloat("MasterVol", -0.04f);

        if (PlayerPrefs.HasKey("MusVolume"))
            mix.SetFloat("MusicVol", PlayerPrefs.GetFloat("MusVolume"));
        else
            mix.SetFloat("MusicVol", 0.0f);

        if (PlayerPrefs.HasKey("WVolume"))
            mix.SetFloat("WeaponVol", PlayerPrefs.GetFloat("WVolume"));
        else
            mix.SetFloat("WeaponVol", -1.29f);

        if (PlayerPrefs.HasKey("EVolume"))
            mix.SetFloat("EnemyVol", PlayerPrefs.GetFloat("EVolume"));
        else
            mix.SetFloat("EnemyVol", 0.0f);

        if (PlayerPrefs.HasKey("PlVolume"))
            mix.SetFloat("PlayerVol", PlayerPrefs.GetFloat("PlVolume"));
        else
            mix.SetFloat("PlayerVol", -0.20f);

        if (PlayerPrefs.HasKey("PiVolume"))
            mix.SetFloat("PickupVol", PlayerPrefs.GetFloat("PiVolume"));
        else
            mix.SetFloat("PickupVol", 0.11f);

        if (PlayerPrefs.HasKey("UIVolume"))
            mix.SetFloat("UIVol", PlayerPrefs.GetFloat("UIVolume"));
        else
            mix.SetFloat("UIVol", 0.0f);

        if (PlayerPrefs.HasKey("MiscVolume"))
            mix.SetFloat("MiscVol", PlayerPrefs.GetFloat("MiscVolume"));
        else
            mix.SetFloat("MiscVol", 0.0f);
    }
}
