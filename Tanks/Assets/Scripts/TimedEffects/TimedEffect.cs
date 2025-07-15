// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TimedEffect 
{
    public float cooldown;

    public Action<Tank> set;
    public Action<Tank> reset;

    private Coroutine coroutine;

    public bool enabled;


    public TimedEffect(float cooldown, Action<Tank> set, Action<Tank> reset) {
        this.cooldown = cooldown;
        this.set = set;
        this.reset = reset;
        enabled = true;
    }

    private IEnumerator RunCoroutine(Tank tank) {
        set(tank);

        yield return new WaitForSeconds(cooldown);

        reset(tank);
    }

    public void Start(Tank tank) {
        coroutine = tank.StartCoroutine(RunCoroutine(tank));
    }

    ~TimedEffect() {
        if (enabled) {
            Debug.Log("ERROR: Deleted TimedEffect without resetting value. This is not allowed");
        }
    }

    public void Kill(Tank tank) {
        if(coroutine == null) {
            return;
        }

        reset(tank);

        UnityEngine.MonoBehaviour s = new();
        s.StopCoroutine(coroutine);
        coroutine = null;
        enabled = false;
    }   
}


