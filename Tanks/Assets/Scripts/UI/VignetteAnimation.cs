// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignetteAnimation : MonoBehaviour
{
    public Volume volume;

    private Vignette vignette;

    public float baseIntensity;

    public float animationIntensity = 0.3f;

    public float enableTime = 0.5f;

    public AnimationCurve animCurve;

    // Start is called before the first frame update
    void Start()
    {
        volume.profile.TryGet(out vignette);

        baseIntensity = vignette.intensity.value + 0.293f;

        //volume.weight = 0f;
        vignetteEnabled = false;
    }

    bool vignetteEnabled = false;

    public void EnableVignette()
    {
        vignetteEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (vignetteEnabled)
        {

            if (volume.weight < 1f)
            {
                //volume.weight += 1f / enableTime * Time.deltaTime;
            }

            vignette.intensity.value = Mathf.Sin(animCurve.Evaluate(Time.time % 1)) * animationIntensity + baseIntensity;
        }

    }


}
