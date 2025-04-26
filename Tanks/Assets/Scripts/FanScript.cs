using UnityEngine;

public class FanScript : MonoBehaviour
{
    public float basePushPower; //linear factor
    public float exponentialPushPower; //base in the exponential decay

    private float maxEffectiveDistance;

    public GameObject fanCollider;
    public ParticleSystem effect;

    public bool active;
    public bool canEffectEnemy;

    public GameObject fanBladeModel;

    // Start is called before the first frame update
    private void Start() {
        basePushPower = 0.4f;
        exponentialPushPower = 2f;
        maxEffectiveDistance = fanCollider.transform.lossyScale.y;
        effect.startLifetime = fanCollider.transform.lossyScale.y / 6 * 1.3f;
    }

    private void Update()
    {
        if (!active)
            return;
        fanBladeModel.transform.localEulerAngles = Vector3.forward * (fanBladeModel.transform.localEulerAngles.z + 480f * Time.deltaTime);
    }

    public void pushPlayer(Collider player) {
        var distance = getDistanceFromPlayer(player);
        var displacement = //this factor is always at least 1
            basePushPower * Time.deltaTime
                          * transform.forward;
        displacement *= Mathf.Pow(exponentialPushPower, (maxEffectiveDistance - distance) / maxEffectiveDistance);

        var threshold = maxEffectiveDistance / 2.5f;


        if (distance <= threshold) {
            displacement *= 1 + 5 * (threshold - distance);
        }

        player.transform.position += displacement;
    }

    private float getDistanceFromPlayer(Collider player) {
        var x1 = transform.position.x;
        var z1 = transform.position.z;

        var x2 = player.transform.position.x;
        var z2 = player.transform.position.z;

        return Vector2.Distance(new Vector2(x1, z1), new Vector2(x2, z2));
    }
}