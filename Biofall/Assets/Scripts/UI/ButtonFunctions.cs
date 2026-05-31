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
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
    Debug.Log("Quit is disabled in WebGL builds.");
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
