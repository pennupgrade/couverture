// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalResetter : MonoBehaviour
{
    [SerializeField] private Vector3 ResetNormal;

    void OnTriggerEnter(Collider c)
    {
        Tank t = c.gameObject.GetComponent<Tank>();
        if (t is not null)
        {

        }
    }
}
