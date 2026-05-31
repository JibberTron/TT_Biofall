using UnityEngine;

public class enemySounds : MonoBehaviour
{
    [Header("-----AI Sounds-----")]
    [SerializeField] AudioSource sfx_Source;
    [SerializeField] AudioClip enemyDeath;
    [SerializeField] AudioClip enemyWalk;
    [SerializeField] AudioClip enemyHit;
    [SerializeField] AudioClip enemyAttack;
    [SerializeField] AudioClip enemyChase;

    public AudioClip EnemyDeath => enemyDeath;
    public AudioClip EnemyWalk => enemyWalk;
    public AudioClip EnemyHit => enemyHit;
    public AudioClip EnemyAttack => enemyAttack;
    public AudioClip EnemyChase => enemyChase;

    void Awake()
    {
        AwakeChecks();
    }
    void AwakeChecks()
    {
        if(sfx_Source == null)
        {
            Debug.Log("Audio Source == null");
            return;
        }
        if (enemyDeath == null)
        {
            Debug.Log("Death audio == null");
            return;
        }
        if (enemyWalk == null)
        {
            Debug.Log("Walk audio == null");
            return;
        }
        if (enemyHit == null)
        {
            Debug.Log("Hit audio == null");
            return;
        }
        if (enemyAttack == null)
        {
            Debug.Log("Attack audio == null");
            return;
        }
        if (enemyChase == null)
        {
            Debug.Log("Chase audio == null");
            return;
        }
    }
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
