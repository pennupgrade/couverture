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
        maxEffectiveDistance = transform.Find("FanCollider").transform.lossyScale.y;
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
        float distancePower = 1;
        if (distance < 2)
        {
            distancePower = 5f;
        }
        player.transform.position +=
            0.8f * maxEffectiveDistance / distance
            * basePushPower * Time.deltaTime
            * new Vector3(xComponent, 0, zComponent);
    }

    float getDistanceFromPlayer(Collider player)
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
