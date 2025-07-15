// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public int nextIndex;

    public bool isMoving = true;

    public void EnableMoving()
    {
        isMoving = true;
    }

    public void DisableMoving()
    {
        isMoving = false; 
    }
}
