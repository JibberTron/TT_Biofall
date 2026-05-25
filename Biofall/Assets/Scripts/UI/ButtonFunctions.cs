using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    [SerializeField] string openingScene;
    public void Resume()
    {
        Gamemanager.instance.StateUnpause();
    }

    public void Restart()
    {
        SceneManager.LoadScene(openingScene);
        Gamemanager.instance.StateUnpause();
    }

    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void Play()
    {
        Debug.Log("PLAY BUTTON WORKS");

        Gamemanager.instance.StateUnpause();
    }

    public void Back()
    {
        Gamemanager.instance.StateControlPanelOff();
    }

    public void Controls()
    {
        Gamemanager.instance.ControlsLegend();
    }
}
