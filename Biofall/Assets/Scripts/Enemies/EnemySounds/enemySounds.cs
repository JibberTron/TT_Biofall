using UnityEngine;

public class enemySounds : MonoBehaviour
{
    [Header("-----AI Sounds-----")]
    [SerializeField] AudioSource sfx_Source;
    [SerializeField] AudioClip enemyDeath;
    [SerializeField] AudioClip enemyWalk;
    [SerializeField] AudioClip enemyHit;
    [SerializeField] AudioClip enemyAttack;

    public AudioClip EnemyDeath => enemyDeath;
    public AudioClip EnemyWalk => enemyWalk;
    public AudioClip EnemyHit => enemyHit;
    public AudioClip EnemyAttack => enemyAttack;

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
