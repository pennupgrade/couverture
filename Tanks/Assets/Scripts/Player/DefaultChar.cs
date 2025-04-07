using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultChar : Character
{
    public override bool Ability(Tank tank)
    {
        return true;
    }

    public override void AbilityUpdate(Tank t)
    {

    }

    public override float getCoolDown()
    {
        return 0;
    }

    public override bool isActive()
    {
        return false;
    }
}