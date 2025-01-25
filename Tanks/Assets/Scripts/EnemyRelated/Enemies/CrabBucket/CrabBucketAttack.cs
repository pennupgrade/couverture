using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabBucketAttack : MonoBehaviour
{
    public bool playerInRange;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter");
        playerInRange = true;
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit");
        playerInRange = false;
    }
}
