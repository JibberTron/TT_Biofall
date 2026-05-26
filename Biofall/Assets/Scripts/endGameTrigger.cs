using UnityEngine;

public class endGameTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Gamemanager.instance.Win();
    }
}
