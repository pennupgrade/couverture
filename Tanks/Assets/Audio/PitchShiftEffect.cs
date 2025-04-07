using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PitchShiftEffect : SoundEffect
{
    public float shiftDelta;

    public float time;

    private float originalPitch;

    public override void Invoke(Sound s)
    {
        base.Invoke(s);

        originalPitch = s.source.pitch;
        s.source.pitch = s.source.pitch + shiftDelta;
        StartCoroutine(ChangePitch(s));
    }

    private IEnumerator ChangePitch(Sound s)
    {
        yield return new WaitForSeconds(time);
        //s.source.pitch = originalPitch;
        yield return null;
    }
}
