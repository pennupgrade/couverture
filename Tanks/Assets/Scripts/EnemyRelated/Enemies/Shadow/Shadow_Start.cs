using UnityEngine;

public class Shadow_Start : EnemyStartState
{
    public Shadow_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        enemy.audioManager.Play("Engine");
        ((EnemyOmniMove)enemy).accel = true;
        ((Shadow)enemy).startInvisFlicker();
        return (alert) ? new Shadow_Alert(enemy) : new Shadow_Idle(enemy);
    }
}
