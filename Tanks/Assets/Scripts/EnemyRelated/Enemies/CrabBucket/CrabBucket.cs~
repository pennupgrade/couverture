using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabBucket : MonoBehaviour
{
    private Collider m_Collider;
    bool attacking = false;
    float attackWaitTime = 2;

    // Start is called before the first frame update
    void Start()
    {
        m_Collider = gameObject.GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (attacking)
        {
            if(attackWaitTime >= 2)
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
            Debug.Log("Player Enter");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            attacking = false;
            attackWaitTime = 2;
            Debug.Log("Player Exit");
        }
    }

    private void Attack()
    {
        Debug.Log("One Attack");
    }
}
