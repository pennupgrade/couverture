using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabBucket : MonoBehaviour
{
    private SphereCollider m_Collider;
    private SphereCollider m_DealDamageCollider;

    [SerializeField]
    float SensingRange = 3; // I think this variable isn't used - Anthony
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
        m_DealDamageCollider.radius = AttackRange; // is this line and above necessary if you assign their values on 29 and 31 - Anthony

        if (attacking)
        {
            if(attackWaitTimeCounter >= attackWaitTime) 
            {
                // Should we have a delay before the swipe so the player can get ready, or some visual indicator - Anthony
                Attack();
                attackWaitTimeCounter = 0;
            }
            else
            {
                attackWaitTimeCounter += Time.fixedDeltaTime;
            }
            // what happens to attackWaitTimeCounter if you aren't attacking, do you want it to go down - Anthony
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
            // if you go in and out of the trigger, the crab will immediately attack
            // I fee like attackWaitTimeCounter should start at 0 - Anthony
        }
    }

    private void Attack()
    {
        Debug.Log("One Attack");
        attackEffects.Play();
        // Considering the collider is a sphere, you may be better off doing Vector3.Distance or smth
        // but honestly your choice, I think this is fine - Anthony
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
