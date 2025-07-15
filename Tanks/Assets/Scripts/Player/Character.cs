// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character
{
    public abstract bool Ability(Tank tank);

    public abstract void AbilityUpdate(Tank t);

    public abstract float getCoolDown();

    public abstract bool isActive();
}
