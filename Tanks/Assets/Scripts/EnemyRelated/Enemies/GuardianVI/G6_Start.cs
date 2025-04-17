using UnityEngine;

public class G6_Start : EnemyStartState
{
    public G6_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        enemy.audioManager.Play("Engine");
        ((EnemyOmniMove)enemy).accel = true;
        return (alert) ? new G6_Alert(enemy) : new G6_Idle(enemy);
    }
}
