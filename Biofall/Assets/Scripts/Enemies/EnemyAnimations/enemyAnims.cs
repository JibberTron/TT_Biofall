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
        anims.SetBool("IsLooking", _isInvestigating);
    }
    public void SetMovement(float _mag)
    {
        enemyRef.Animator.SetFloat("Speed", _mag);
    }
    public void Attack(bool _should)
    {
        enemyRef.Animator.SetBool("Attack", _should);
    }
    public void Death(bool _should)
    {
        enemyRef.Animator.SetBool("Dead", _should);
    }
    public void StandUp(bool _should)
    {
        enemyRef.Animator.SetBool("StandUp", _should);
    }
}
