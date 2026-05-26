using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class levelSwitch : MonoBehaviour
{
    [SerializeField] string nextLevel;
    [SerializeField]Animator transition;
    [SerializeField] Image fade;
    [SerializeField] GameObject nextStartingLocation;
    GameObject player;
    
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
        if (player == null) return;
    }
    void Start()
    {     
        if(nextStartingLocation == null)
        {
            Debug.Log("Next levels Location is null, auto setting");
            nextStartingLocation = GameObject.FindGameObjectWithTag("Start");
        }
        if (transition == null)
        {
            Debug.Log("Animator == null");
            return;
        }
        if(fade == null)
        {
            Debug.Log("Image == null");
            return;
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
        player.transform.position = nextStartingLocation.transform.position;
    }
}
