// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileAttackScript : MonoBehaviour
{
    MissileScript missileScript;
    // Start is called before the first frame update
    void Start()
    {
        missileScript = transform.parent.GetComponent<MissileScript>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        //Debug.Log("damage zone collided with " + collider.tag);
        missileScript.handleAttack(collider);
    }
}
