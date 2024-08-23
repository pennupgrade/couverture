using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject livesManagerPrefab;

    void Awake()
    {
        // Check if LivesManager already exists
        if (LivesManager.Instance == null)
        {
            // Instantiate LivesManager from prefab
            Instantiate(livesManagerPrefab);
        }
    }
}
