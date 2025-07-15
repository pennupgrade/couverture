// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using UnityEngine;

public class Aegis_Start : EnemyStartState
{
    public Aegis_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        ((EnemyOmniMove)enemy).accel = true;
        enemy.audioManager.Play("Engine");
        return (alert) ? new Aegis_Alert(enemy) : new Aegis_Idle(enemy);
    }
}
