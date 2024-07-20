using UnityEngine;

public class TankMoveState : TankState
{
    Vector3 bodyNormal;
    Vector3 bodyVector;
    Vector3 bodyPivot;
    Vector3[] hitNormals;

    public TankMoveState(Tank tank) : base(tank) {
        hitNormals = new Vector3[2];
    }

    public override TankState HandleMovement(Vector2 dir)
    {  
        if (dir.magnitude < 0.1f) return new TankIdleState(tank);

        CalculateBodyProperties();

        Vector3 direction = Vector3.ProjectOnPlane(bodyVector, tank.transform.up);
        tank.transform.position += tank.moveSpeed * direction * dir.y * Time.deltaTime;
        tank.transform.RotateAround(bodyPivot, tank.transform.up, tank.rotSpeed * dir.x * Time.deltaTime);

        RaycastWheels();
        RotateBodyByWheels();

        return this;
    }

    public override TankState HandleShoot()
    {
        // Debug.Log("Shoot from MoveState");

        GameObject bullet = Object.Instantiate(tank.bulletPrefab, tank.gunShotPos.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = tank.gun.transform.right * tank.bulletSpeed;

        return new TankShotCooldownState(tank);
    }

    void RaycastWheel(GameObject wheel, int id)
    {
        RaycastHit hit;

        // Raycast args
        Vector3 origin = wheel.transform.position;
        Vector3 direction = -tank.transform.up;
        float maxDist = 3.0f;

        // Perform raycast to find ground
        Physics.Raycast(origin, direction, out hit, maxDist);

        // We hit an object
        if (hit.collider != null)
        {
            Vector3 hitPoint = hit.point;
            hitPoint += tank.transform.up * 0.2f; // 0.2f value is arbitrary, lifts up wheel

            float hitDist = Vector3.Distance(hitPoint, origin);
            Debug.Log("Wheel " + id + " dist: " + hitDist);

            // hit point is within range of falling smoothly
            if (Vector3.Distance(hitPoint, origin) <= 2.0)
            {
                hitNormals[id] = hit.normal;

                // The wheel needs to be rotated by the normal?
                wheel.transform.localRotation = Quaternion.Euler(hit.normal);

                // vertically translate the wheel from the ground
                Vector3 wheelPos = wheel.transform.position;
                wheelPos.y = hitPoint.y;
                wheel.transform.position = wheelPos;
            }
        }

        // Initiate gravity


    }

    void RaycastWheels()
    {
        RaycastWheel(tank.FrontWheel, 0);
        RaycastWheel(tank.BackWheel, 1);
    }

    // Calculates bodyVector, bodyPivot, and bodyNormal
    void CalculateBodyProperties()
    {
        Vector3 FrontWheelPos = tank.FrontWheel.transform.position;
        Vector3 BackWheelPos = tank.BackWheel.transform.position;

        // vector representing "forward" vector of the tank
        bodyVector = Vector3.Normalize(FrontWheelPos - BackWheelPos);

        // Position the body in the middle of the two wheels
        float wheelDistances = Vector3.Distance(FrontWheelPos, BackWheelPos);
        bodyPivot = 0.5f * wheelDistances * bodyVector + BackWheelPos;

        // Some vector math to get the body's normal (does Unity have a function for this?)
        bodyNormal = Vector3.Cross(bodyVector, tank.transform.up);
        bodyNormal = Vector3.Normalize(Vector3.Cross(bodyNormal, bodyVector));
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

        tank.Body.transform.rotation = Quaternion.Slerp(tank.Body.transform.rotation, targetRotation, Time.deltaTime * 20.0f);

        // Use this to translate based on normal
        Vector3 bodyPosition = bodyPivot;
        bodyPosition += bodyNormal * -0.075f;
        tank.Body.transform.position = bodyPosition;
    }


}