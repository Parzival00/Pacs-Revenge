using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Event References")]
    [SerializeField] PlayerCombat playerCombat;
    [SerializeField] StungunController stungunController;
    [SerializeField] PlayerStateManager playerState;

    [Header("References")]
    [SerializeField] GameObject hud;
    [SerializeField] TMP_Text livesText;
    [SerializeField] TMP_Text ammoText;
    [SerializeField] TMP_Text shieldText;
    [SerializeField] TransitionEffect transitionEffect;
    [SerializeField] TutorialController tutorial;
    bool doTutorials;
    [SerializeField] Image crosshair;
    [SerializeField] Image extraLifeFlash;

    HUDMessenger hudMessenger;

    // Start is called before the first frame update
    void Start()
    {
        hudMessenger = FindObjectOfType<HUDMessenger>();
    }

    //Event Handlers
    public void HandleStunAmmoChanged(int ammoCount)
    {
        ammoText.text = $"{ammoCount}%";
    }
    public void HandleLivesChanged(int livesCount)
    {
        livesText.text = $"{livesCount}";
    }
    public void HandleShieldChanged(int shieldCount)
    {
        shieldText.text = $"{shieldCount}";
    }
    public void HandleTargetChanged(Color crosshairColor)
    {
        crosshair.color = crosshairColor;
    }
    /*public void HandleGunMessenger(string message, float length)
    {
        hudMessenger.Display(message, length);
    }*/
    public void HandleGunState(bool gunActivated)
    {
        hud.SetActive(gunActivated);
    }

    //Subscribe and Unsubscribe to events
    private void OnEnable()
    {
        stungunController.OnStunAmmoChanged += HandleStunAmmoChanged;
        playerCombat.OnTargetChanged += HandleTargetChanged;
        crosshair.color = Color.white;
        playerState.OnGunStateChanged += HandleGunState;
        hud.SetActive(false);
    }
    private void OnDisable()
    {
        stungunController.OnStunAmmoChanged -= HandleStunAmmoChanged;
        playerCombat.OnTargetChanged -= HandleTargetChanged;
        playerState.OnGunStateChanged -= HandleGunState;
    }
}
