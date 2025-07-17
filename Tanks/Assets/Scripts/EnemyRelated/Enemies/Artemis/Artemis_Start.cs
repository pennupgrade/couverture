// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using UnityEngine;

public class Artemis_Start : EnemyStartState
{
    public Artemis_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        enemy.audioManager.Play("Engine");
        return (alert) ? new Artemis_Alert(enemy) : new Artemis_Idle(enemy);
    }
}
