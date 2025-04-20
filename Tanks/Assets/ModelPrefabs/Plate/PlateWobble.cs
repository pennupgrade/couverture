using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateWobble : MonoBehaviour
{
    public Animator animator;

    public MovingPlatformBackAndForth movingPlatform;

    public float wobbleDelay;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WobbleWithDelay());

        animator.SetLayerWeight(1, lightWobbleWeight);
    }

    private IEnumerator WobbleWithDelay()
    {
        yield return new WaitForSeconds(wobbleDelay);
        animator.Play("PlateWobble");
    }

    public float lightWobbleWeight = 0.5f;


    float currentWeight = 0.0f;
    // Update is called once per frame
    void Update()
    {
        if (movingPlatform.passengerList.Count >= 1)
        {
            currentWeight += Time.deltaTime;
            currentWeight = Mathf.Clamp01(currentWeight);
        }
        else
        {
            currentWeight -= Time.deltaTime;

            currentWeight = Mathf.Clamp(currentWeight, lightWobbleWeight, 1f);
        }

        animator.SetLayerWeight(1, currentWeight);
    }
}
