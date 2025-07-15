// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDamageFlash : MonoBehaviour
{
    // Start is called before the first frame update
    private DamageFlash df;
    void Start()
    {
        df = new DamageFlash(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void damageFlash() {
        df.CallDamageFlash(this);
    }
}
