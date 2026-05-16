using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuGameOver;
    [SerializeField] GameObject menuWin;

    public Image infectionBar;
    public Image playerHPBar;
    public bool isPaused;
    public GameObject player;
    // public PlayerController playerScript;

    private InfectionSystem infection;

    float timeScaleOrig;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;

        // player = GameObject.FindWithTag("Player");
        // playerScript = player.GetComponent<PlayerController>();

        infection = player.GetComponent<InfectionSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            UpdateInfectionHUD();

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
    }

    public void GameOver()
    {
        StatePause();
        menuActive = menuGameOver;
        menuActive.SetActive(true);
    }

    private void UpdateInfectionHUD()
    {
        infectionBar.fillAmount = infection.currentInfection / infection.maxInfection;
    }
}