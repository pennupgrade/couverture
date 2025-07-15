// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableScript : MonoBehaviour, IDestroyable
{
    private DamageFlash damageFlash;
    public GameObject Body;
    public float breakTime;
    public int hits;
    private bool isDead;

    // Start is called before the first frame update
    void Start()
    {
        damageFlash = new DamageFlash(this.gameObject);
        damageFlash._flashTime = breakTime;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void incapacitate(float t) { }

    void destroy()
    {
        Destroy(gameObject, breakTime);
    }

    public void takeDamage(int dmg)
    {
        hits -= dmg / 100;

        if (hits <= 0 && !isDead)
        {
            isDead = true;
            destroy();

            return;
        }
        else
        {
            damageFlash.CallDamageFlash(this);
        }
    }
}
