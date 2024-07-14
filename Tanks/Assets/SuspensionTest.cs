using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspensionTest : MonoBehaviour
{
    private GameObject FrontWheel;
    private GameObject BackWheel;
    private GameObject Body;
    GameObject[] Wheels = new GameObject[2];

    enum Direction
    {
        FORWARD,
        BACKWARD
    };


    void Start()
    {
        FrontWheel = transform.Find("Front").gameObject;
        BackWheel = transform.Find("Back").gameObject;
        Body = transform.Find("Body").gameObject;
    }

    // Raycast testing
    void RaycastWheel(GameObject wheel)
    {
        RaycastHit hit;

        // Raycast args
        Vector3 origin = wheel.transform.position;
        Vector3 direction = -transform.up;
        float maxDist = 3.0f;

        // Perform raycast to find ground
        Physics.Raycast(origin, direction, out hit, maxDist);

        // We hit an object
        if (hit.collider != null)
        {
            Vector3 hitPoint = hit.point;
            hitPoint += transform.up * 0.2f; // 0.2f value is arbitrary, lifts up wheel

            // vertically translate the wheel from the ground
            Vector3 wheelPos = wheel.transform.position;
            wheelPos.y = hitPoint.y;
            wheel.transform.position = wheelPos;
        }
    }

    void RaycastWheels()
    {
        RaycastWheel(FrontWheel);
        RaycastWheel(BackWheel);
    }

    // Function rotates the body so thats forward vector points towards vector from 2 wheels
    void RotateBodyByWheels()
    {
        Vector3 FrontWheelPos = FrontWheel.transform.position;
        Vector3 BackWheelPos = BackWheel.transform.position;

        // Find the angle between two points (wheels), and then rotates body
        Vector3 bodyVector = Vector3.Normalize(FrontWheelPos - BackWheelPos);
        float angle = Vector3.Angle(bodyVector, transform.up);
        Body.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

        // Position the body in the middle of the two wheels
        float wheelDistances = Vector3.Distance(FrontWheelPos, BackWheelPos);
        Vector3 bodyPosition = 0.5f * wheelDistances * bodyVector + BackWheelPos;

        // Some vector math to get the body's normal (does Unity have a function for this?)
        // Use this to translate based on normal
        Vector3 bodyNormal = Vector3.Cross(bodyVector, transform.up);
        bodyNormal = Vector3.Cross(bodyNormal, bodyVector);
        bodyPosition += bodyNormal * -0.075f;

        Body.transform.position = bodyPosition;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastWheels();
        RotateBodyByWheels();
    }
}
