// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClearActivator : MonoBehaviour
{
    public GameObject[] toChange;
    public GameObject[] enemies;
    [SerializeField] private int enemiesRemaining;

    // Start is called before the first frame update
    void Start()
    {
        enemiesRemaining = enemies.Length;
        foreach (GameObject g in enemies) {
            g.SetActive(true);
            g.GetComponent<Enemy>().onDeath += enemyDestroyed;
        }
    }
    public void enemyDestroyed() {
        enemiesRemaining -= 1;
        if (enemiesRemaining <= 0) {
            foreach (GameObject g in toChange) {
                if (g.TryGetComponent<Activatable>(out Activatable aObj)) {
                    aObj.activate();
                } else {
                    g.SetActive(!g.activeSelf);
                }
            }
        }
    }
}
