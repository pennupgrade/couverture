using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanScript : MonoBehaviour
{
    private float basePushPower;
    private float maxEffectiveDistance;
    // Start is called before the first frame update
    void Start()
    {
        basePushPower = 0.8f;
        maxEffectiveDistance = transform.Find("FanCollider").transform.lossyScale.y; // nitpicky but maybe just attach the FanCollider as a reference - Anthony
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pushPlayer(Collider player)
    {
        float fanAngle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
        float zComponent = Mathf.Cos(fanAngle);
        float xComponent = Mathf.Sin(fanAngle);
        float distance = getDistanceFromPlayer(player);
        float distancePower = 1; // get rid of this - Anthony
        if (distance < 2)
        {
            distancePower = 5f;
        }
        player.transform.position +=
            0.8f * maxEffectiveDistance / distance // Kevin D. wants more power - Anthony
            * basePushPower * Time.deltaTime
            * new Vector3(xComponent, 0, zComponent); // I think you can just do like += transform.forward * magnitude and it'd effectively be the same - Anthony
    }

    float getDistanceFromPlayer(Collider player) // you can use Vector2.Distance for shorter code - Anthony
    {
        float x1 = transform.position.x;
        float z1 = transform.position.z;

        float x2 = player.transform.position.x;
        float z2 = player.transform.position.z;

        float xDist = x2 - x1;
        float zDist = z2 - z1;

        return Mathf.Sqrt(Mathf.Pow(xDist, 2) + Mathf.Pow(zDist, 2));
    }
}
