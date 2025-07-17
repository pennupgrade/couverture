// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using UnityEngine;

public class Storm_Start : EnemyStartState
{
    public Storm_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        enemy.audioManager.Play("Engine");
        ((EnemyOmniMove)enemy).accel = true;
        return (alert) ? new Storm_Alert(enemy) : new Storm_Idle(enemy);
    }
}
