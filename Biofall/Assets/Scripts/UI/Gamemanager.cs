using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine.SceneManagement;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuGameOver;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject HUD;
    [SerializeField] GameObject ammoText;
    [SerializeField] GameObject controlPanel;
    [SerializeField] GameObject menuFade;

    public Image infectionBar;
    public Image playerHPBar;
    public Image gunImage;
    public bool isPaused;
    public GameObject hallucinationFlashScreen;
    private bool isFlashing;

    private InfectionHallucination hallucination;
    private InfectionSystem infection;
    public GameObject player;
    public PlayerController playerScript;
    private Gun gun;
    private PebbleThrower thrownPebble;
    private HidingSystem hidingSystem;
    private CameraOrbit cameraOrbit;

    float timeScaleOrig;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;

        // Time.timeScale = 0f;
        // mainMenu.SetActive(true);

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();

        infection = player.GetComponent<InfectionSystem>();
        hallucination = player.GetComponent<InfectionHallucination>();
        gun = player.GetComponent<Gun>();
        thrownPebble = player.GetComponent<PebbleThrower>();
        hidingSystem = player.GetComponent<HidingSystem>();
        cameraOrbit = FindFirstObjectByType<CameraOrbit>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInfectionHUD();

        if(hallucination != null && hallucination.IsHallucinating() && isFlashing)
        {
            StartCoroutine(FlashHallucination());
        }

        if (Input.GetButtonDown("Cancel"))
        {

            if(menuActive == null)
            {
                StatePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
               StateUnpause();
            }
        }

        //temp to debug
        if(Input.GetKeyDown(KeyCode.P))
        {
            Win();
        }
    }


    public void StatePause()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        hallucination.enabled = false;
        infection.enabled = false;
        playerScript.enabled = false;
        gun.enabled = false;
        hidingSystem.enabled = false;
        thrownPebble.enabled = false;
        cameraOrbit.enabled = false;
        HUD.SetActive(false);
    }

    public void StateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        // turn what ever menu is active off
        menuActive.SetActive(false);
        menuActive = null;
        hallucination.enabled = true;
        infection.enabled = true;
        playerScript.enabled = true;
        gun.enabled = true;
        hidingSystem.enabled = true;
        thrownPebble.enabled = true;
        cameraOrbit.enabled = true;
        HUD.SetActive(true);
    }

    public void GameOver()
    {
        StatePause();
        menuActive = menuGameOver;
        menuActive.SetActive(true);
    }

    public void Win()
    {

        menuActive = menuWin;
        menuActive.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        hallucination.enabled = false;
        infection.enabled = false;
        playerScript.enabled = false;
        gun.enabled = false;
        hidingSystem.enabled = false;
        thrownPebble.enabled = false;
        cameraOrbit.enabled = false;
        HUD.SetActive(false);

        StartCoroutine(WinSequence()); 
    }

    IEnumerator FadeOut()
    {

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        hallucination.enabled = false;
        infection.enabled = false;
        playerScript.enabled = false;
        gun.enabled = false;
        hidingSystem.enabled = false;
        thrownPebble.enabled = false;
        cameraOrbit.enabled = false;
        HUD.SetActive(false);
        menuActive = menuFade;
        menuActive.SetActive(true);

        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("FinalCredits");
    }

    IEnumerator WinSequence()
    {
        yield return new WaitForSeconds(2f);

        StartCoroutine(FadeOut());
    }

    public void ControlsLegend()
    {
        menuPause.SetActive(false);
        controlPanel.SetActive(true);
    }

    public void StateControlPanelOff()
    {
        StateUnpause();
        controlPanel.SetActive(false);
    }

    private void UpdateInfectionHUD()
    {
        Gamemanager.instance.infectionBar.fillAmount = infection.currentInfection / infection.maxInfection;
    }

    IEnumerator FlashHallucination()
    {
        isFlashing = true;

        while(hallucination.IsHallucinating())
        {
            hallucinationFlashScreen.SetActive(true);

            yield return new WaitForSeconds(0.1f);

            hallucinationFlashScreen.SetActive(false);

            yield return new WaitForSeconds(0.1f);
        }

        hallucinationFlashScreen.SetActive(false);

        isFlashing = false;
    }

    public void GunAmmo()
    {
        int currentAmmo = gun.GetCurrentAmmo();

        int totalAmmo = gun.GetTotalAmmo();

        string ammo = currentAmmo.ToString() + " | " + totalAmmo.ToString();
        ammoText.GetComponent<TextMeshProUGUI>().text = ammo;
    }
}