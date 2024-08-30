using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defend_Start : EnemyStartState
{
    public Defend_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        return new Defend_Active(enemy);
    }
}
