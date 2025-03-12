using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileScript : MonoBehaviour
{
    private float g = -9.81f;

    public int damage;
    public Vector3 startLocation;
    public Vector3 endLocation;
    public float secondsInAir; //must be positive

    public GameObject missileBody;
    public GameObject targetZone;

    private bool hitTarget;
    private Vector3 acceleration;
    private Vector3 velocity;
    private float rotationTwirlInDegrees = 15f;

    // Start is called before the first frame update
    void Start()
    {
        startLocation = gameObject.transform.position;
        endLocation = targetZone.transform.position;
        secondsInAir = Mathf.Abs(secondsInAir); //avoid negative time

        hitTarget = false;
        acceleration = new Vector3(0, g, 0);

        //calculate initial velocity

        float initialVelocityX = (endLocation.x - startLocation.x) / secondsInAir;
        float initialVelocityZ = (endLocation.z - startLocation.z) / secondsInAir;

        float displacementY = endLocation.y - startLocation.y;
        float initialVelocityY = (float) ((displacementY - 0.5 * g * Mathf.Pow(secondsInAir,2)) / secondsInAir);
        velocity = new Vector3(initialVelocityX, initialVelocityY, initialVelocityZ);
        Debug.Log(velocity);
        missileBody.transform.rotation = Quaternion.LookRotation(new Vector3(0, 1, 0));
       // missileBody.transform.localEulerAngles = new Vector3(-90, 0, 0); //initially facing upward
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(velocity);
        if (!hitTarget)
        {
            missileBody.transform.position += velocity * Time.deltaTime;
            velocity += acceleration * Time.deltaTime;


            //float eulerAngleX = -Mathf.Atan(velocity.y / velocity.z) * Mathf.Rad2Deg;
            //float eulerAngleY = Mathf.Atan(velocity.z / velocity.x) * Mathf.Rad2Deg;
            //float eulerAngleZ = Mathf.Atan(velocity.y / velocity.x) * Mathf.Rad2Deg;
            //missileBody.transform.localEulerAngles =
            //    new Vector3(eulerAngleX, eulerAngleY, eulerAngleZ);
            missileBody.transform.rotation = Quaternion.LookRotation(velocity);
            Debug.Log(missileBody.transform.localEulerAngles);
        } else
        {
            velocity = new Vector3();
            missileBody.transform.position =
                new Vector3(endLocation.x, endLocation.y + 0.01f, endLocation.z);
           // missileBody.transform.localEulerAngles = new Vector3(90, 0, 0); //facing downward
        }
    }

    public void setHitTarget(bool value)
    {
        hitTarget = value;
    }

}
