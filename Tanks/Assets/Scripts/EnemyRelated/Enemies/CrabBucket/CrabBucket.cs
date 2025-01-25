using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabBucket : MonoBehaviour
{
    private SphereCollider m_Collider;
    private SphereCollider m_DealDamageCollider;

    [SerializeField]
    float SensingRange = 3;
    [SerializeField]
    float AttackRange = 2;
    bool attacking = false;
    [SerializeField]
    float attackWaitTime = 1.5f;
    float attackWaitTimeCounter = 0;
    [SerializeField]
    int damage = 100;

    private CrabBucketAttack attackScript;
    private ParticleSystem attackEffects;
    private AudioSource sound;

    // Start is called before the first frame update
    void Start()
    {
        m_Collider = gameObject.GetComponent<SphereCollider>();
        m_Collider.radius = SensingRange;
        m_DealDamageCollider = gameObject.GetComponentInChildren<SphereCollider>();
        m_DealDamageCollider.radius = AttackRange;
        attackScript = gameObject.GetComponentInChildren<CrabBucketAttack>();
        attackEffects = gameObject.GetComponentInChildren<ParticleSystem>();
        sound = gameObject.GetComponentInChildren<AudioSource>();
        attackWaitTimeCounter = attackWaitTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //For Serialize Field to Work During RunTime
        m_Collider.radius = SensingRange;
        m_DealDamageCollider.radius = AttackRange;

        if (attacking)
        {
            if(attackWaitTimeCounter >= attackWaitTime)
            {
                Attack();
                attackWaitTimeCounter = 0;
            }
            else
            {
                attackWaitTimeCounter += Time.fixedDeltaTime;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            attacking = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            attacking = false;
            attackWaitTimeCounter = attackWaitTime;
        }
    }

    private void Attack()
    {
        Debug.Log("One Attack");
        attackEffects.Play();
        if (attackScript.isPlayerInRange())
        {
            Debug.Log("Deals Damage");
            attackScript.getPlayer().gameObject.GetComponent<Tank>().takeDamage(damage);
            sound.Play();
        }
        else
        {
            Debug.Log("Missed");
        }
    }
}
