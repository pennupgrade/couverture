// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

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
