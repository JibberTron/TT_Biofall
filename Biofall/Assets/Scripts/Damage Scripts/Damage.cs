using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour
{
    enum DamageType { Bullet, Stationary, DOT }

    [SerializeField] DamageType type;
    [SerializeField] Rigidbody rb;
    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;
    [SerializeField] int bulletSpeed;
    [SerializeField] int bulletDestroyTime;
    [SerializeField] ParticleSystem hitEffect;

    bool isDamaging;

    void Start()
    {
        if (type == DamageType.Bullet)
        {
            rb.linearVelocity = transform.forward * bulletSpeed;
            Destroy(gameObject, bulletDestroyTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        iDamage dmg = other.GetComponent<iDamage>();
        if (dmg == null)
            dmg = other.GetComponentInParent<iDamage>();

        if (dmg != null && type != DamageType.DOT)
            dmg.TakeDamage(damageAmount);

        if (type == DamageType.Bullet)
        {
            if (hitEffect != null)
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.isTrigger) return;

        iDamage dmg = other.GetComponent<iDamage>();
        if (dmg == null)
            dmg = other.GetComponentInParent<iDamage>();

        if (dmg != null && type == DamageType.DOT && !isDamaging)
            StartCoroutine(DamageOther(dmg));
    }

    IEnumerator DamageOther(iDamage d)
    {
        isDamaging = true;
        d.TakeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }
}