using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyRangeGizmo : MonoBehaviour
{
    [DrawGizmo(GizmoType.Selected)]
    static void DrawEnemyRangeGizmo(Enemy enemy, GizmoType gizmoType) {
        float range = enemy.gunRangeOverride;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(enemy.transform.position, range);
    }
}
