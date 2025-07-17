// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using UnityEngine;

public class G5_Start : EnemyStartState
{
    public G5_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        enemy.audioManager.Play("Engine");
        ((EnemyOmniMove)enemy).accel = true;
        return (alert) ? new G5_Alert(enemy) : new G5_Idle(enemy);
    }
}
