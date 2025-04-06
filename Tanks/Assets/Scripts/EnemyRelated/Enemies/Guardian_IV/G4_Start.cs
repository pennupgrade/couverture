using UnityEngine;

public class G4_Start : EnemyStartState
{
    public G4_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        enemy.audioManager.Play("Engine");
        ((EnemyOmniMove)enemy).accel = true;
        return (alert) ? new G4_Alert(enemy) : new G4_Idle(enemy);
    }
}
