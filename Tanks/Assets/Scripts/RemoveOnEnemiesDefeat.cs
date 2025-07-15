// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveOnEnemiesDefeat : MonoBehaviour
{

    public List<GameObject> enemies;

    void Update()
    {
        enemies.RemoveAll((x) => x == null);
        if (enemies.Count == 0) {
            Destroy(gameObject);
        }
    }
}
