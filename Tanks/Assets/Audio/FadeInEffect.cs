using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FadeInEffect : SoundEffect
{
    public float time;

    public float baseVolume;

    private float originalVolume;

    private float currTime;

    public override void Invoke(Sound s)
    {
        originalVolume = s.volume;
        currTime = 0f;
        s.source.volume = baseVolume;
        StartCoroutine(ChangeVolume(s));
    }

    private IEnumerator ChangeVolume(Sound s)
    {
        while (true)
        {
            print("a");

            currTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            s.source.volume = Mathf.Lerp(baseVolume, originalVolume, currTime / time);

            if (currTime >= time)
            {
                yield break;
            }
        }
    }
}
