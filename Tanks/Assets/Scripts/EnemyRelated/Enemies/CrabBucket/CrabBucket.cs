using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabBucket : MonoBehaviour
{
    private Collider m_Collider;
    bool attacking = false;
    float attackWaitTime = 3;
    private CrabBucketAttack attackScript;
    private ParticleSystem attackEffects;

    // Start is called before the first frame update
    void Start()
    {
        m_Collider = gameObject.GetComponent<SphereCollider>();
        attackScript = gameObject.GetComponentInChildren<CrabBucketAttack>();
        attackEffects = gameObject.GetComponentInChildren<ParticleSystem>();
        Debug.Log(attackEffects.isPaused);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (attacking)
        {
            if(attackWaitTime >= 3)
            {
                Attack();
                attackWaitTime = 0;
            }
            else
            {
                attackWaitTime += Time.fixedDeltaTime;
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
            attackWaitTime = 3;
        }
    }

    private void Attack()
    {
        Debug.Log("One Attack");
        attackEffects.Play();
        if (attackScript.isPlayerInRange())
        {
            Debug.Log("Deals Damage");
        }
        else
        {
            Debug.Log("Missed");
        }
    }
}
