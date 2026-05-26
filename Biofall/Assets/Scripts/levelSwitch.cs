using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class levelSwitch : MonoBehaviour
{
    [SerializeField] string nextLevel;
    [SerializeField]Animator transition;
    [SerializeField] Image fade;
    GameObject player;
    GameObject loc;
    
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
        if (player == null) return;
    }
    void Start()
    {
        loc = GameObject.FindGameObjectWithTag("Start");
        if(loc == null)
        {
            Debug.Log("Location is NULL");
            return;
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
        player.transform.position = loc.transform.position;
    }
}
