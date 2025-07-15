// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutEffect : SoundEffect
{
    public float length;

    public float baseVolume;

    private float originalVolume;

    private float currTime;

    public override void Invoke(Sound s)
    {
        base.Invoke(s);

        originalVolume = s.volume;
        currTime = 0f;
        StartCoroutine(ChangeVolume(s));
    }

    private IEnumerator ChangeVolume(Sound s)
    {
        yield return new WaitForSecondsRealtime(s.clip.length - length);

        while (true)
        {
            print("a");

            currTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            s.source.volume = Mathf.Lerp(originalVolume, baseVolume, currTime / length);

            if (currTime >= length)
            {
                yield break;
            }
        }
    }
}
