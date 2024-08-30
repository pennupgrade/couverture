using UnityEngine;

public class Phantom_Start : EnemyStartState
{
    public Phantom_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        enemy.StartCoroutine(((Phantom)enemy).activateCamo());
        return (alert) ? new Phantom_Alert(enemy) : new Phantom_Idle(enemy);
    }
}
