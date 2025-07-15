// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalDecay : MonoBehaviour
{
    public DecalProjector decalProjector;

    public AnimationCurve opacityCurve;

    public float lifetime;

    // Start is called before the first frame update
    void Start()
    {
        startLife = Time.time;
    }

    float startLife;

    // Update is called once per frame
    void Update()
    {
        decalProjector.fadeFactor = opacityCurve.Evaluate((Time.time - startLife) / lifetime);

        if (decalProjector.fadeFactor <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
