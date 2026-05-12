using UnityEngine;

public class enemyAnims : MonoBehaviour
{
    enemyReferences enemyRef;

    Animator anims;

    void Awake()
    {
        enemyRef = GetComponent<enemyReferences>();
    }
    void  Start()
    {
        anims = enemyRef.Animator;
    }
    public void Investigate(bool _isInvestigating)
    {
        Debug.Log("INVESTIGATE");
        enemyRef.Agent.speed = 0;
        anims.SetBool("IsLooking", _isInvestigating);
    }
    public void SetSpeed(float _mag, float _speed)
    {
        Debug.Log("WALK");
        enemyRef.Agent.speed = _speed;
        enemyRef.Animator.SetFloat("Speed", _mag);
    }
    public void Grab()
    {
        enemyRef.Agent.isStopped = true;
        enemyRef.Animator.SetTrigger("Attack");
    }
}
