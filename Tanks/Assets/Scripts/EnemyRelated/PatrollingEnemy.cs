// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingEnemy : Enemy
{
    public bool followWaypoints;
    public Transform[] waypoints;
    public int increment(int ind) {
        ind++;
        if (ind >= waypoints.Length) {
            ind = 0;
        }
        return ind;
    }
}

public class PatrollingEnemyOmni : EnemyOmniMove
{
    public bool followWaypoints;
    public Transform[] waypoints;
    public int increment(int ind) {
        ind++;
        if (ind >= waypoints.Length) {
            ind = 0;
        }
        return ind;
    }
}
