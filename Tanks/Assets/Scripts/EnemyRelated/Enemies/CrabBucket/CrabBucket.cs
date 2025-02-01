using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabBucket : MonoBehaviour
{
    bool attacking = false;
    [SerializeField]
    float attackWaitTime = 1.5f;
    float attackWaitTimeCounter = 0;
    [SerializeField]
    int damage = 100;

    private CrabBucketAttack attackScript;
    private ParticleSystem attackEffects;
    [SerializeField] private AudioSource sound;
    [SerializeField] private AudioSource blockedSound;

    [SerializeField] private GameObject sphere_indicator;
    [SerializeField] private LayerMask obstacleLayer;

    // Start is called before the first frame update
    void Start()
    {
        attackScript = gameObject.GetComponentInChildren<CrabBucketAttack>();
        attackEffects = gameObject.GetComponentInChildren<ParticleSystem>();
        attackWaitTimeCounter = attackWaitTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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

        sphere_indicator.transform.localScale = new Vector3(attackWaitTimeCounter * 4,
            1, attackWaitTimeCounter * 4);
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
            GameObject player = attackScript.getPlayer().gameObject;

            // Raycast
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer))
            {
                Debug.Log("Deals Damage");
                player.GetComponent<Tank>().takeDamage(damage);
                sound.Play();
            }
            else
            {
                Debug.Log("Attack Blocked by Obstacle");
                blockedSound.Play();
            }
        }
        else
        {
            Debug.Log("Missed");
        }
    }
}
