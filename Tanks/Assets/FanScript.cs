using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanScript : MonoBehaviour
{
    private float basePushPower;         //linear factor
    private float exponentialPushPower;  //base in the exponential decay


    private float maxEffectiveDistance;
    public GameObject fanCollider;

    // Start is called before the first frame update
    void Start()
    {
        basePushPower = 2.5f;
        exponentialPushPower = 2f;
        maxEffectiveDistance = fanCollider.transform.lossyScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pushPlayer(Collider player)
    {
        float distance = getDistanceFromPlayer(player);
       
        player.transform.position +=

            //this factor is always at least 1
            Mathf.Pow(exponentialPushPower, maxEffectiveDistance - distance)
            * basePushPower * Time.deltaTime
            * transform.forward;
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
