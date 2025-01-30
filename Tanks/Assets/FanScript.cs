using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanScript : MonoBehaviour
{
    public enum PowerFunction
    {
        Linear,
        Exponential,
        Hyperbolic
    };

    [SerializeField] // Using [SerializeField] can expose the below float to the editor - Anthony
    private float basePushPower, exponentialPushPower, linearPushPower, hyperPushPower;

    private float maxEffectiveDistance;
    public GameObject fanCollider;
    public PowerFunction functionType;

    // Start is called before the first frame update
    void Start()
    {
        maxEffectiveDistance = fanCollider.transform.lossyScale.y;
    }
    public void pushPlayer(Collider player)
    {
        float distance = getDistanceFromPlayer(player);

        float falloff = 1.0f;

        switch (functionType)
        {
            case PowerFunction.Linear:
                falloff = Mathf.Lerp(0, linearPushPower, 1.0f - (distance / maxEffectiveDistance));
                break;
            case PowerFunction.Exponential:
                falloff = Mathf.Pow(exponentialPushPower, maxEffectiveDistance - distance);
                break;
            case PowerFunction.Hyperbolic:
                falloff = hyperPushPower * maxEffectiveDistance / distance;
                break;
            default:
                break;
        }

        player.transform.position +=
            //this factor is always at least 1
            falloff * basePushPower * Time.deltaTime
            * transform.forward;
    }

    float getDistanceFromPlayer(Collider player)
    {
        return Vector2.Distance(
            new Vector2(transform.position.x, transform.position.z), 
            new Vector2(player.transform.position.x, player.transform.position.z));
    }
}
