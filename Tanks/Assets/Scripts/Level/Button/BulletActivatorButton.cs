// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletActivatorButton : ActivatorButton
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (!onCooldown && other.transform.tag == "Projectile")
        {
            buttonPressed();
        }
    }
}
