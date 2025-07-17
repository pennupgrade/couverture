// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using UnityEngine;

public class Phantom_Start : EnemyStartState
{
    public Phantom_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        enemy.audioManager.Play("Engine");
        enemy.StartCoroutine(((Phantom)enemy).activateCamo());
        return (alert) ? new Phantom_Alert(enemy) : new Phantom_Idle(enemy);
    }
}
