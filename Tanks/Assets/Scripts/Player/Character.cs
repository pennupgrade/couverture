using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character
{
    public abstract bool Ability(Tank tank);

    public abstract void AbilityUpdate(Tank t);
}
