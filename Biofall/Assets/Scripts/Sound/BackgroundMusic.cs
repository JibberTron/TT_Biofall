using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] float volume = 0.3f;
    [SerializeField] float fadeSpeed = 1f;

    bool isFading;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.volume = volume;
        audioSource.loop = true;

        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    public void SetVolume(float value)
    {
        volume = Mathf.Clamp01(value);
        audioSource.volume = volume;
    }

    public void FadeOut()
    {
        StartCoroutine(Fade(0f));
    }

    public void FadeIn()
    {
        StartCoroutine(Fade(volume));
    }

    System.Collections.IEnumerator Fade(float targetVolume)
    {
        while (!Mathf.Approximately(audioSource.volume, targetVolume))
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, fadeSpeed * Time.deltaTime);
            yield return null;
        }
    }
}