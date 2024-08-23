using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerBootstrapper : MonoBehaviour
{
    [SerializeField] private GameObject GameManagerPrefab;
    void Awake()
    {
        if (GameManager.Instance == null)
        {
            Instantiate(GameManagerPrefab);
        }
    }
}
