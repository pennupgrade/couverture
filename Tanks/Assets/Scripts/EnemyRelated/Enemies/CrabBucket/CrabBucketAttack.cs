using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabBucketAttack : MonoBehaviour
{
    private bool playerInRange = false;
    private Collider player;

    public bool isPlayerInRange()
    {
        return playerInRange;
    }

    public Collider getPlayer()
    {
        return player;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            playerInRange = true;
            player = other;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInRange = false;
        }
    }
}
