using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    public virtual void Invoke(Sound s)
    {
        StopAllCoroutines();
    }

}
