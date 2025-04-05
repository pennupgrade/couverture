using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHandler : MonoBehaviour
{
    public Tank tank;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tank.AbilityUpdate();
        if (Input.GetKeyDown(KeyCode.Q))
        {
            bool success = tank.Ability();
            if (tank.CharacterHasAbility() && success)
            {
                Debug.Log("Cooling Down");
                UIManager.instance.Gameplay_Panel.
                    GetComponentInChildren<GameplayHUDManager>()
                    .StartFillAbilityBar(tank.character.getCoolDown());
            }
        }
    }
}
