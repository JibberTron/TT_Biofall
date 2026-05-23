using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class levelSwitch : MonoBehaviour
{
    [SerializeField] string nextLevel;
    Animator transition;

    void Start()
    {
        transition = Gamemanager.instance.GetComponentInParent<Animator>();
        Gamemanager.instance.transitionScreen.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        StartCoroutine(LoadScene());    
    }
    IEnumerator LoadScene()
    {
        Gamemanager.instance.transitionScreen.enabled = true;
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(nextLevel);
    }
}
