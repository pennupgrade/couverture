using UnityEngine;

public class G2_Start : EnemyStartState
{
    public G2_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        enemy.StartCoroutine(((Guardian2)enemy).deployMines());
        return (alert) ? new G2_Alert(enemy) : new G2_Idle(enemy);
    }
}
