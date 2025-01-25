using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabBucket : MonoBehaviour
{
    private Collider m_Collider;
    bool attacking = false;
    float attackWaitTime = 1.5f;
    float attackWaitTimeCounter = 0;
    private CrabBucketAttack attackScript;
    private ParticleSystem attackEffects;
    private AudioSource sound;

    // Start is called before the first frame update
    void Start()
    {
        m_Collider = gameObject.GetComponent<SphereCollider>();
        attackScript = gameObject.GetComponentInChildren<CrabBucketAttack>();
        attackEffects = gameObject.GetComponentInChildren<ParticleSystem>();
        sound = gameObject.GetComponent<AudioSource>();
        Debug.Log(attackEffects.isPaused);
        attackWaitTimeCounter = attackWaitTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
            sound.Play();
        }
        else
        {
            Debug.Log("Missed");
        }
    }
}
