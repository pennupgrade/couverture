using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutEffect : SoundEffect
{
    public float time;

    public float baseVolume;

    private float originalVolume;

    private float currTime;

    public override void Invoke(Sound s)
    {
        originalVolume = s.volume;
        currTime = 0f;
        StartCoroutine(ChangeVolume(s));
    }

    private IEnumerator ChangeVolume(Sound s)
    {
        yield return new WaitForSecondsRealtime(s.clip.length - time);

        while (true)
        {
            print("a");

            currTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            s.source.volume = Mathf.Lerp(originalVolume, baseVolume, currTime / time);

            if (currTime >= time)
            {
                yield break;
            }
        }
    }
}
