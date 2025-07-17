// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBob : MonoBehaviour
{
    [SerializeField] private float BaseIntensity;
    [SerializeField] private float LightDelta;
    [SerializeField] private float BobSpeed;

    private Light light;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
