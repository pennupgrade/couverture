using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabBucketAttack : MonoBehaviour
{
    private bool playerInRange = false;

    public bool isPlayerInRange()
    {
        return playerInRange;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            playerInRange = true;
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
