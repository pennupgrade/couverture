using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspensionTest : MonoBehaviour
{
    private GameObject FrontWheel;
    private GameObject BackWheel;
    private GameObject Body;

    Vector3 bodyNormal;
    Vector3 bodyVector;
    Vector3 bodyPivot;
    Vector3[] hitNormals;

    void Start()
    {
        FrontWheel = transform.Find("Front").gameObject;
        BackWheel = transform.Find("Back").gameObject;
        Body = transform.Find("Body").gameObject;
        hitNormals = new Vector3[2];
    }

    // Raycast testing
    void RaycastWheel(GameObject wheel, int id)
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

            hitNormals[id] = hit.normal;

            // The wheel needs to be rotated by the normal?
            wheel.transform.localRotation = Quaternion.Euler(hit.normal);

            // vertically translate the wheel from the ground
            Vector3 wheelPos = wheel.transform.position;
            wheelPos.y = hitPoint.y;
            wheel.transform.position = wheelPos;
        }
    }

    void RaycastWheels()
    {
        RaycastWheel(FrontWheel, 0);
        RaycastWheel(BackWheel, 1);
    }

    // Calculates bodyVector, bodyPivot, and bodyNormal
    void CalculateBodyProperties()
    {
        Vector3 FrontWheelPos = FrontWheel.transform.position;
        Vector3 BackWheelPos = BackWheel.transform.position;

        // vector representing "forward" vector of the tank
        bodyVector = Vector3.Normalize(FrontWheelPos - BackWheelPos);

        // Position the body in the middle of the two wheels
        float wheelDistances = Vector3.Distance(FrontWheelPos, BackWheelPos);
        bodyPivot = 0.5f * wheelDistances * bodyVector + BackWheelPos;

        // Some vector math to get the body's normal (does Unity have a function for this?)
        bodyNormal = Vector3.Cross(bodyVector, transform.up);
        bodyNormal = Vector3.Normalize(Vector3.Cross(bodyNormal, bodyVector));
    }

    // Function rotates the body so thats forward vector points towards vector from 2 wheels
    void RotateBodyByWheels()
    {
        // Function finds bodyVector, bodyPivot, and bodyNormal
        CalculateBodyProperties();

        // Find the angle between two points (wheels), and then rotates body
        Vector3 hitNormal = Vector3.Normalize(hitNormals[0] + hitNormals[1]);
        //Vector3 forward = Vector3.Cross(Body.transform.right, hitNormal).normalized;
        Vector3 forward = Quaternion.Euler(0, 90, 0) * bodyVector;
        Vector3 desiredForward = Vector3.ProjectOnPlane(forward, hitNormal).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(desiredForward, hitNormal);

        Body.transform.rotation = Quaternion.Slerp(Body.transform.rotation, targetRotation, Time.deltaTime * 20.0f);

        // Use this to translate based on normal
        Vector3 bodyPosition = bodyPivot;
        bodyPosition += bodyNormal * -0.075f;
        Body.transform.position = bodyPosition;
    }

    void MoveTank()
    {
        // Function finds bodyVector, bodyPivot, and bodyNormal
        CalculateBodyProperties();

        float speed = 2.0f;
        float rotSpeed = 100.0f;

        Vector3 delta = new Vector3();

        if (Input.GetKey(KeyCode.W))
        {
            delta.x += 1.0f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            delta.y -= 1.0f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            delta.x -= 1.0f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            delta.y += 1.0f;
        }

        // This is a really fucking bad idea
        Vector3 direction = Vector3.ProjectOnPlane(bodyVector, transform.up);
        transform.position += speed * direction * delta.x * Time.deltaTime;
        transform.RotateAround(bodyPivot, transform.up, rotSpeed * delta.y * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        MoveTank();
        RaycastWheels();
        RotateBodyByWheels();
    }
}
