using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnPlayerEnterExit : MonoBehaviour
{
    public UnityEvent onPlayerEnter;
    public UnityEvent onPlayerExit;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onPlayerEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onPlayerExit.Invoke();
        }
    }

}
