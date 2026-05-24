using UnityEngine;

public class enemySounds : MonoBehaviour
{
    [SerializeField] AudioSource sfx_Source;
    [SerializeField] public AudioClip enemyDeath;
    [SerializeField] public AudioClip enemyWalk;
    [SerializeField] public AudioClip enemyHit;
    [SerializeField] public AudioClip enemyAttack;

    public void PlayClip(AudioClip _clip)
    {
        sfx_Source.PlayOneShot(_clip);
    }
    public void PlayWalk()
    {
        sfx_Source.clip = enemyWalk;
        sfx_Source.Play();
    }
    public void PlayAttack()
    {
        sfx_Source.clip = enemyAttack;
        sfx_Source.Play();
    }
}
