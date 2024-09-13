using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingEnemy : Enemy
{
    public bool followWaypoints;
    public Vector3[] waypoints;
    public int increment(int ind) {
        ind++;
        if (ind >= waypoints.Length) {
            ind = 0;
        }
        return ind;
    }
}
