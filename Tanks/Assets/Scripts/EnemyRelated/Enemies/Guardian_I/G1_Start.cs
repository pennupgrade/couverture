// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using UnityEngine;

public class G1_Start : EnemyStartState
{
    public G1_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        enemy.audioManager.Play("Engine");
        return (alert) ? new G1_Alert(enemy) : new G1_Idle(enemy);
    }
}
