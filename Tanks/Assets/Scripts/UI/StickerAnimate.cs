using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class StickerAnimate : MonoBehaviour
{
    private float animateTime = 0.8f; 
    [SerializeField] private float rotateDegrees = 2f;
    private RectTransform rect;
    private float timer;
    private float offset;
    private float startRotation;
    
    private void Start()
    {
        rect = GetComponent<RectTransform>();
        startRotation = rect.localEulerAngles.z;
        offset = (Random.Range(0,2) * 2 - 1) * rotateDegrees + startRotation;
        rect.localEulerAngles = Vector3.forward * offset;
        timer = animateTime;
    }

    private void Update()
    {
        timer = Mathf.Max(timer - Time.unscaledDeltaTime, 0f);
        if (timer == 0f)
        {
            offset *= -1f;
            rect.localEulerAngles = Vector3.forward * offset;
            timer = animateTime;
        }
    }
}
