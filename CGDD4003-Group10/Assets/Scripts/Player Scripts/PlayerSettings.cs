using System;
using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    private float fov = 70f;
    public float FOV => fov;

    private float sensitivity = 60;
    public float Sensitivity => sensitivity;

    private float mastVolume = 0;
    public float MastVolume => mastVolume;

    private float musVolume = 0;
    public float MusVolume => musVolume;

    private float wVolume = 0;
    public float WVolume => wVolume;

    private float eVolume = 0;
    public float EVolume => eVolume;

    private float plVolume = 0;
    public float PlVolume => plVolume;

    private float piVolume = 0;
    public float PiVolume => piVolume;

    private float uiVolume = 0;
    public float UIVolume => uiVolume;

    private float miscVolume = 0;
    public float MiscVolume => miscVolume;

    private int resolution = 2;
    public float Resolution => resolution;

    private int fullscreen = 1;
    public int Fullscreen => fullscreen;

    private int headBob = 1;
    public int HeadBob => headBob;

    private int weaponSelection = 0;
    public int WeaponSelection => weaponSelection;

    public enum SettingType { FOV, Sensitivity, Audio, Resolution, ScreenMode, ViewBobbing, WeaponSelection }

    public event Action<SettingType> OnSettingsChanged;

    public void LoadFromPrefs()
    {
        fov = PlayerPrefs.GetFloat("FOV", fov);
        sensitivity = PlayerPrefs.GetFloat("Sensitivity", sensitivity);
        mastVolume = PlayerPrefs.GetFloat("MastVolume", mastVolume);
        musVolume = PlayerPrefs.GetFloat("MusVolume", musVolume);
        wVolume = PlayerPrefs.GetFloat("WVolume", wVolume);
        eVolume = PlayerPrefs.GetFloat("EVolume", eVolume);
        plVolume = PlayerPrefs.GetFloat("PlVolume", plVolume);
        piVolume = PlayerPrefs.GetFloat("PiVolume", piVolume);
        uiVolume = PlayerPrefs.GetFloat("UIVolume", uiVolume);
        miscVolume = PlayerPrefs.GetFloat("MiscVolume", miscVolume);
        resolution = PlayerPrefs.GetInt("Resolution", resolution);
        fullscreen = PlayerPrefs.GetInt("Fullscreen", fullscreen);
        headBob = PlayerPrefs.GetInt("HeadBob", headBob);

        weaponSelection = PlayerPrefs.GetInt("Weapon", weaponSelection);
    }

    public void SetFOV(float newFOV)
    {
        if (Mathf.Approximately(fov, newFOV)) return;

        fov = newFOV;
        PlayerPrefs.SetFloat("FOV", newFOV);
        OnSettingsChanged?.Invoke(SettingType.FOV);
    }

    public void SetSensitivity(float newSens)
    {
        if (Mathf.Approximately(sensitivity, newSens)) return;

        sensitivity = newSens;
        PlayerPrefs.SetFloat("Sensitivity", newSens);
        OnSettingsChanged?.Invoke(SettingType.Sensitivity);
    }

    public void SetMasterVolume(float newMastVol)
    {
        if (Mathf.Approximately(mastVolume, newMastVol)) return;

        mastVolume = newMastVol;
        PlayerPrefs.SetFloat("MastVolume", mastVolume);
        OnSettingsChanged?.Invoke(SettingType.Audio);
    }

    public void SetMusicVolume(float newMusVol)
    {
        if (Mathf.Approximately(musVolume, newMusVol)) return;

        musVolume = newMusVol;
        PlayerPrefs.SetFloat("MusVolume", musVolume);
        OnSettingsChanged?.Invoke(SettingType.Audio);
    }

    public void SetWVolume(float newWVol)
    {
        if (Mathf.Approximately(wVolume, newWVol)) return;

        wVolume = newWVol;
        PlayerPrefs.SetFloat("WVolume", wVolume);
        OnSettingsChanged?.Invoke(SettingType.Audio);
    }

    public void SetEVolume(float newEVol)
    {
        if (Mathf.Approximately(eVolume, newEVol)) return;

        eVolume = newEVol;
        PlayerPrefs.SetFloat("EVolume", eVolume);
        OnSettingsChanged?.Invoke(SettingType.Audio);
    }

    public void SetPlVolume(float newPlVol)
    {
        if (Mathf.Approximately(plVolume, newPlVol)) return;

        plVolume = newPlVol;
        PlayerPrefs.SetFloat("PlVolume", plVolume);
        OnSettingsChanged?.Invoke(SettingType.Audio);
    }

    public void SetPiVolume(float newPiVol)
    {
        if (Mathf.Approximately(piVolume, newPiVol)) return;

        piVolume = newPiVol;
        PlayerPrefs.SetFloat("PiVolume", piVolume);
        OnSettingsChanged?.Invoke(SettingType.Audio);
    }

    public void SetUIVolume(float newUIVol)
    {
        if (Mathf.Approximately(uiVolume, newUIVol)) return;

        uiVolume = newUIVol;
        PlayerPrefs.SetFloat("UIVolume", uiVolume);
        OnSettingsChanged?.Invoke(SettingType.Audio);
    }

    public void SetMiscVolume(float newMiscVol)
    {
        if (Mathf.Approximately(miscVolume, newMiscVol)) return;

        miscVolume = newMiscVol;
        PlayerPrefs.SetFloat("MiscVolume", miscVolume);
        OnSettingsChanged?.Invoke(SettingType.Audio);
    }

    public void SetResolution(int newRes)
    {
        if (resolution == newRes) return;

        resolution = newRes;
        PlayerPrefs.SetFloat("Resolution", resolution);
        OnSettingsChanged?.Invoke(SettingType.Resolution);
    }

    public void SetFullscreen(int newFullscreen)
    {
        if (fullscreen == newFullscreen) return;

        fullscreen = newFullscreen;
        PlayerPrefs.SetFloat("Fullscreen", fullscreen);
        OnSettingsChanged?.Invoke(SettingType.ScreenMode);
    }

    public void SetHeadBob(int newHeadBob)
    {
        if (headBob == newHeadBob) return;

        headBob = newHeadBob;
        PlayerPrefs.SetFloat("HeadBob", headBob);
        OnSettingsChanged?.Invoke(SettingType.ViewBobbing);
    }

    public void SetWeaponSelection(int newWepSel)
    {
        if (weaponSelection == newWepSel) return;

        weaponSelection = newWepSel;
        PlayerPrefs.SetInt("Weapon", weaponSelection);
        OnSettingsChanged?.Invoke(SettingType.WeaponSelection);
    }
}
