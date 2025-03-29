using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sentry2_Idle : EnemyStartState
{
    public Sentry2_Idle(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        return (alert) ? new Sentry2_Attack(enemy) : new Sentry2_Active(enemy);
    }
}
