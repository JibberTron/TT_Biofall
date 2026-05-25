using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class levelSwitch : MonoBehaviour
{
    [SerializeField] string nextLevel;
    [SerializeField]Animator transition;
    [SerializeField] Image fade;
    
    void Start()
    {
        if(transition == null)
        {
            Debug.Log("Animator == null");
        }
        if(fade == null)
        {
            Debug.Log("Image == null");
        }
        fade.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        StartCoroutine(LoadScene());    
    }
    IEnumerator LoadScene()
    {
        fade.enabled = true;
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(nextLevel);
    }
}
