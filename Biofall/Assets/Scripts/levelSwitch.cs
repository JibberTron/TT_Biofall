using UnityEngine;
using UnityEngine.SceneManagement;

public class levelSwitch : MonoBehaviour
{
    [SerializeField] string nextLevel;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        SceneManager.LoadScene(nextLevel);
    }
}
