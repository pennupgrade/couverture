// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using UnityEngine;

public class Javelin_Start : EnemyStartState
{
    public Javelin_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        enemy.audioManager.Play("Engine");
        ((EnemyOmniMove)enemy).accel = true;
        return (alert) ? new Javelin_Attack(enemy) : new Javelin_Idle(enemy);
    }
}
