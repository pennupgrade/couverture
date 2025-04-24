using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlinkerScript : MonoBehaviour
{
    [SerializeField] private GameObject lght;
    [SerializeField] private Material newMaterial;

    public void Activate()
    {
        GetComponent<MeshRenderer>().material = newMaterial;
        lght.SetActive(true);
    }
}