using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float baseShake = 0.3f;

    public float delay = 0.1f;
    
    public void Shake(float intensity)
    {
        StartCoroutine(ShakeCam(intensity));
    }

    private IEnumerator ShakeCam(float intensity)
    {
        yield return new WaitForSeconds(delay);

        Vector3 randShake = new Vector3(0f, 0f, baseShake) * (intensity / 1000f);
        transform.position += randShake;
        yield return new WaitForSeconds(0.04f);
        transform.position -= randShake;
        yield return new WaitForSeconds(0.03f);
        transform.position -= randShake;
        yield return new WaitForSeconds(0.02f);
        transform.position += randShake / 1.5f;
    }

}
