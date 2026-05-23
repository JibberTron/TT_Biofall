using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    [SerializeField] AudioSource creditsMusic;


    public void CreditSequence()
    {
        StartCoroutine(EndCredits());
    }

    IEnumerator EndCredits()
    {
        while (creditsMusic.volume > 0)
        {
            creditsMusic.volume -= Time.deltaTime * 5.0f;

            yield return null;
        }

        SceneManager.LoadScene("Main Menu");
    }

}
