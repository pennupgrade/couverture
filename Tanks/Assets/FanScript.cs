using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanScript : MonoBehaviour
{
    private float basePushPower;         //linear factor
    private float exponentialPushPower;  //base in the exponential decay



    private float maxEffectiveDistance;
    public GameObject fanCollider;
    public ParticleSystem effect;

    // Start is called before the first frame update
    void Start()
    {
        basePushPower = 0.65f;
        exponentialPushPower = 4f;
        maxEffectiveDistance = fanCollider.transform.lossyScale.y;
        effect.startLifetime = fanCollider.transform.lossyScale.y / 6 * 1.3f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pushPlayer(Collider player)
    {
        float distance = getDistanceFromPlayer(player);
        Vector3 displacement = //this factor is always at least 1
            basePushPower * Time.deltaTime
            * transform.forward;

        float threshold = maxEffectiveDistance / 2.5f;
        displacement *= (distance >= threshold)
            ? Mathf.Pow(exponentialPushPower, (maxEffectiveDistance - distance) / maxEffectiveDistance)
            : Mathf.Pow(maxEffectiveDistance,1.5f);

        player.transform.position += displacement;
        Debug.Log(distance);
    }
    
    float getDistanceFromPlayer(Collider player)
    {
        float x1 = transform.position.x;
        float z1 = transform.position.z;

        float x2 = player.transform.position.x;
        float z2 = player.transform.position.z;

        return Vector2.Distance(new Vector2(x1, z1), new Vector2(x2, z2));
    }
}
