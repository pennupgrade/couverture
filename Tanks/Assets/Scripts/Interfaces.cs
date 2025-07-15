// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IDestroyable {
    void takeDamage(int dmg);
    void incapacitate(float time);
}

interface IAlertableEnemy {
    void alert(bool alertState);
}