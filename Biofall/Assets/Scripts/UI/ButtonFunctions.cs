using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
<<<<<<< HEAD
    [SerializeField] string openingLevel;

   public void Resume()
=======
    [SerializeField] string openingScene;
    public void Resume()
>>>>>>> a9577b15126cbc13fd32995f0a3b3113b7e52273
    {
        Gamemanager.instance.StateUnpause();
    }

    public void Restart()
    {
<<<<<<< HEAD
        SceneManager.LoadScene(openingLevel);
=======
        SceneManager.LoadScene(openingScene);
>>>>>>> a9577b15126cbc13fd32995f0a3b3113b7e52273
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
