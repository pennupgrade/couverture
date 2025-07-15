// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankDummy : MonoBehaviour
{
    // Start is called before the first frame update
    private bool stunOver;
    private float startTime;
    void Start()
    {
        stunOver = false;
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > startTime + 2f) {
            stunOver = true;
        }
    }
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer ==  LayerMask.NameToLayer("Obstacle") && Time.time - startTime > 0.25f) {
            stunOver = true;
        }
        
    }

    public bool StunOver() {
        return stunOver;
    }
}
