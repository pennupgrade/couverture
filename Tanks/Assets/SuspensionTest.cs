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

    float bodyDy = 0.0f;
    float epsilon = 0.1f;
    float groundMargin = 0.2f;
    Vector2 hitDists;

    void Start()
    {
        FrontWheel = transform.Find("Front").gameObject;
        BackWheel = transform.Find("Back").gameObject;
        Body = transform.Find("Body").gameObject;
        hitNormals = new Vector3[2];
    }

    // Raycast testing
    bool RaycastWheel(GameObject wheel, int id)
    {
        RaycastHit hit;

        // Raycast args
        Vector3 wheelPos = wheel.transform.position;
        Vector3 origin = wheel.transform.position;
        Vector3 direction = -transform.up;

        float maxDist = 3.0f;

        // Perform raycast to find ground
        Physics.Raycast(origin, direction, out hit, maxDist);

        // We hit an object
        if (hit.collider != null)
        {
            Vector3 hitPoint = hit.point;
            float hitDist = Vector3.Distance(hitPoint, origin);
            hitDists[id] = hitDist;

            if (hitDist <= groundMargin + epsilon) // ???
            {
                hitNormals[id] = hit.normal;

                // The wheel needs to be rotated by the normal?
                wheel.transform.localRotation = Quaternion.Euler(hit.normal);

                // vertically translate the wheel from the ground
                hitPoint += transform.up * groundMargin; // 0.2f value is arbitrary, lifts up wheel
                wheel.transform.position = hitPoint;

                return true;
            }
        }

        return false;
    }

    void RaycastWheels(out bool FrontHit, out bool BackHit)
    {
        FrontHit = RaycastWheel(FrontWheel, 0);
        BackHit = RaycastWheel(BackWheel, 1);
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

    void PositionTankByWheels(bool FrontHit, bool BackHit)
    {
        float frontDy = 0;
        float backDy = 0;

        Vector3 FrontWheelPos = FrontWheel.transform.position;
        Vector3 BackWheelPos = BackWheel.transform.position;
        Vector3 tankNewPos = transform.position;
        float wheelDelta = FrontWheelPos.y - BackWheelPos.y;

        if (!FrontHit && !BackHit) // Neither of them are on the ground
        {
            bodyDy = Mathf.Max(-6.0f, bodyDy + -0.03f); // Gravity acceleration

            if (Mathf.Abs(wheelDelta) > 0.01) // Reset tilt
            {
                float frontDirection = wheelDelta > 0 ? -1 : 1;
                frontDy = wheelDelta * 0.5f * -frontDirection;
                backDy = wheelDelta * 0.5f * frontDirection;
            }
        } 
        else
        {
            float maxWheelDist = 0.05f;
            float liftFromGroundBack = Mathf.Max(0, maxWheelDist + groundMargin - hitDists[1]);
            float liftFromGroundFront = Mathf.Max(0, maxWheelDist + groundMargin - hitDists[0]);

            // Tilting effect
            if (FrontHit && !BackHit && Mathf.Abs(wheelDelta) < maxWheelDist) backDy = -maxWheelDist + liftFromGroundBack;
            if (!FrontHit && BackHit && Mathf.Abs(wheelDelta) < maxWheelDist) frontDy = -maxWheelDist + liftFromGroundFront;
            
            bodyDy = 0.0f;
        }

        // let the Slerp handle the tilts?
        FrontWheelPos.y += frontDy;
        BackWheelPos.y += backDy;
        // Gravity falling
        tankNewPos.y += bodyDy * Time.deltaTime;

        FrontWheel.transform.position = FrontWheelPos;
        BackWheel.transform.position = BackWheelPos;
        transform.position = tankNewPos;
    }

    // Function rotates the body so thats forward vector points towards vector from 2 wheels
    void RotateBodyByWheels()
    {
        // Function finds bodyVector, bodyPivot, and bodyNormal
        CalculateBodyProperties();

        // Find the angle between two points (wheels), and then rotates body
        Vector3 hitNormal = Vector3.Normalize(hitNormals[0] + hitNormals[1]);
        Vector3 forward = Quaternion.Euler(0, 90, 0) * bodyVector;
        Vector3 desiredForward = Vector3.ProjectOnPlane(forward, hitNormal).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(desiredForward, hitNormal);

        Body.transform.rotation = Quaternion.Slerp(Body.transform.rotation, targetRotation, Time.deltaTime * 20.0f);

        // Use this to translate based on normal
        Vector3 bodyPosition = bodyPivot;
        bodyPosition += bodyNormal * -0.075f;
        Body.transform.position = bodyPosition;
    }

    void CenterPivotToChildren()
    {
        if (transform.childCount == 0) return;

        Vector3 center = Vector3.zero;
        foreach (Transform child in transform)
        {
            center += child.position;
        }
        center /= transform.childCount;

        Vector3 offset = transform.position - center;

        foreach (Transform child in transform)
        {
            child.position += offset;
        }

        transform.position = center;
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
        bool FrontHit;
        bool BackHit;

        MoveTank();
        CenterPivotToChildren();
        RaycastWheels(out FrontHit, out BackHit);
        PositionTankByWheels(FrontHit, BackHit);
        RotateBodyByWheels();
        // function to re-center the body pivot because otherwise this is lowkey very ugly u feel me
    }
}
