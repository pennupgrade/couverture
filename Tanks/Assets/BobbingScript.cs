using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingScript : MonoBehaviour
{
    public GameObject venusFlytrap;
    float timer;
    float xAmplitude;
    float yAmplitude;
    float period;
    public float timerOffset = 0;
    Vector3 originalPosition;
    // Start is called before the first frame update
    void Start()
    {
        timer = timerOffset;
        xAmplitude = 0.4f;
        yAmplitude = 0.25f;
        period = 0.5f;
        originalPosition = venusFlytrap.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        //timer +=Time.deltaTime * Mathf.Abs(Mathf.Sin(2 * Mathf.PI / period * timer));
        float xDisplacement = xAmplitude * Mathf.Sin(Mathf.PI / period * (timer - period / 4));
        float yDisplacement = yAmplitude * Mathf.Sin(2 * Mathf.PI / period * timer);
        venusFlytrap.transform.position = originalPosition + new Vector3(xDisplacement, yDisplacement, 0);
    }
}
