// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using UnityEngine;

public class AbilityHandler : MonoBehaviour
{
    public Tank tank;

    // Start is called before the first frame update
    private void Start() { }

    // Update is called once per frame
    private void Update() {
        tank.AbilityUpdate();
        if (Input.GetKeyDown(KeyCode.Q)) {
            var success = tank.Ability();
            if (success) {
                if (tank.character.GetType() == typeof(BubbleChar)) {
                    UIManager.Instance.Gameplay_Panel.GetComponentInChildren<GameplayHUDManager>()
                             .AbilityBarIsCasting(((BubbleChar)tank.character).getStayTime(), tank.character);
                }
                else if (tank.character.GetType() == typeof(RocketChar)) {
                    UIManager.Instance.Gameplay_Panel.GetComponentInChildren<GameplayHUDManager>()
                             .StartFillAbilityBar(tank.character.getCoolDown());
                }
            }
        }
    }
}