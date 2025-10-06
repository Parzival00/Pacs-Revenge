using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPauser : MonoBehaviour
{
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
        MainMenuManager.OnPause += PauseAudio;
        MainMenuManager.OnResume += UnPauseAudio;

        if (MainMenuManager.isGamePaused)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
    }

    public void PauseAudio()
    {
        audioSource.Pause();
    }

    public void UnPauseAudio()
    {
        audioSource.UnPause();
    }

    private void OnDisable()
    {
        MainMenuManager.OnPause -= PauseAudio;
        MainMenuManager.OnResume -= UnPauseAudio;
    }
    private void OnDestroy()
    {
        MainMenuManager.OnPause -= PauseAudio;
        MainMenuManager.OnResume -= UnPauseAudio;
    }
}
