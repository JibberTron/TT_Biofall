using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject controlPanel;
    [SerializeField] GameObject quit;
    [SerializeField] GameObject play;

    public void PlayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void StateControlPanelOn()
    {
        mainMenu.SetActive(false);
        controlPanel.SetActive(true);
    }

    public void StateControlPanelOff()
    {
        mainMenu.SetActive(true);
        controlPanel.SetActive(false);
    }

    public void Back()
    {
        StateControlPanelOff();
    }
}
