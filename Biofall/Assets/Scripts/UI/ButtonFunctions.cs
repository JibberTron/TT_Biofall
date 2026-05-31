using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{



    [SerializeField] string openingLevel;

    public void Resume()
    {
        Gamemanager.instance.StateUnpause();
    }

    public void Restart()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        Gamemanager.instance.StateUnpause();
    }

    public void Quit()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
            Application.OpenURL("about:blank");
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
        Gamemanager.instance.StatePause();
        Gamemanager.instance.ControlsLegend();
    }
}
