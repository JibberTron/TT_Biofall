using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Collections;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuGameOver;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject HUD;

    public Image infectionBar;
    public Image playerHPBar;
    public bool isPaused;
    public GameObject player;
    public PlayerController playerScript;
    public GameObject hallucinationFlashScreen;

    private bool isFlashing;
    private InfectionHallucination hallucination;
    public InfectionSystem infection;

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
    }

    // Update is called once per frame
    void Update()
    {
        // UpdateInfectionHUD();

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
    }

    public void StatePause()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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
        HUD.SetActive(true);
    }

    public void GameOver()
    {
        StatePause();
        menuActive = menuGameOver;
        menuActive.SetActive(true);
    }

    // private void UpdateInfectionHUD()
    // {
        // Gamemanager.instance.infectionBar.fillAmount = infection.currentInfection / infection.maxInfection;
    // }

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
}