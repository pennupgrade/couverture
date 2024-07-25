using UnityEngine;

public class TankMoveState : TankState
{
    Vector3 bodyNormal;
    Vector3 bodyVector;
    Vector3 bodyPivot;
    Vector3[] hitNormals;

    float bodyDy = 0.0f;
    float groundMargin = 0.2f;
    Vector2 hitDists;

    bool FrontHit;
    bool BackHit;

    public TankMoveState(Tank tank) : base(tank) {
        hitNormals = new Vector3[2];
    }

    public override TankState HandleMovement(Vector2 dir)
    {
        FrontHit = RaycastWheel(tank.FrontWheel, 0);
        BackHit = RaycastWheel(tank.BackWheel, 1);

        if (!tank.enableExperimentalGravity)
        {
            FrontHit = false;
            BackHit = false;
        }

        // If we are in free-fall, we will never return idle, so our
        // bodyDy and frontHit and backHit bools will remain from before.
        if (dir.magnitude < 0.1f && (FrontHit || BackHit)) return new TankIdleState(tank);

        // Tank move functions
        CalculateBodyProperties();
        Vector3 direction = Vector3.ProjectOnPlane(bodyVector, tank.transform.up);
        tank.transform.position += tank.moveSpeed * direction * dir.y * Time.deltaTime;
        tank.transform.RotateAround(bodyPivot, tank.transform.up, tank.rotSpeed * dir.x * Time.deltaTime);

        // After moving, perform a raycast on both wheels for ground detection.
        // PositionTankByWheels checks if we are in free fall and makes tank's y fall.
        CenterPivotToChildren();
        RaycastWheels(out FrontHit, out BackHit);
        //if (tank.enableExperimentalGravity) PositionTankByWheels(FrontHit, BackHit);
        RotateBodyByWheels();

        return this;
    }

    public override TankState HandleShoot()
    {
        //Debug.Log("Shoot from MoveState");
        if (tank.numBullets <= 0) {
            return this;
        }

        tank.numBullets--;
        GameObject bullet = Object.Instantiate(tank.bulletPrefab, tank.gunShotPos.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(30 * (Random.value - 0.5f), Vector3.up)
         * (tank.gun.transform.right * tank.bulletSpeed);

        return new TankShotCooldownState(tank);
    }

    public bool RaycastWheel(GameObject wheel, int id)
    {
        RaycastHit hit;

        // Raycast args
        Vector3 origin = wheel.transform.position;
        Vector3 direction = -tank.transform.up;

        // Perform raycast to find ground
        Physics.Raycast(origin, direction, out hit, tank.wheelMaxDist);

        // We hit an object
        if (hit.collider != null)
        {
            Debug.Log(hit.collider.name);

            Vector3 hitPoint = hit.point;
            float hitDist = Vector3.Distance(hitPoint, origin);
            hitDists[id] = hitDist;

            if (hitDist <= tank.groundMargin + 0.02f || !tank.enableExperimentalGravity) // ???
            {
                hitNormals[id] = hit.normal;

                // The wheel needs to be rotated by the normal?
                wheel.transform.localRotation = Quaternion.Euler(hit.normal);

                // vertically translate the wheel from the ground
                hitPoint += tank.transform.up * tank.groundMargin; // 0.2f value is arbitrary, lifts up wheel
                wheel.transform.position = hitPoint;

                return true;
            }
        }

        return false;
    }

    void RaycastWheels(out bool FrontHit, out bool BackHit)
    {
        FrontHit = RaycastWheel(tank.FrontWheel, 0);
        BackHit = RaycastWheel(tank.BackWheel, 1);
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

    void PositionTankByWheels(bool FrontHit, bool BackHit)
    {
        float frontDy = 0;
        float backDy = 0;

        Vector3 FrontWheelPos = tank.FrontWheel.transform.position;
        Vector3 BackWheelPos = tank.BackWheel.transform.position;
        Vector3 tankNewPos = tank.transform.position;
        float wheelDelta = FrontWheelPos.y - BackWheelPos.y;

        if (!FrontHit && !BackHit) // Neither of them are on the ground
        {
            bodyDy = Mathf.Max(-6.0f, bodyDy + -0.03f); // Gravity acceleration

            //if (Mathf.Abs(wheelDelta) > 0.01) // Reset tilt
            //{
              //  float frontDirection = wheelDelta > 0 ? -1 : 1;
              //  frontDy = wheelDelta * 0.5f * -frontDirection;
              //  backDy = wheelDelta * 0.5f * frontDirection;
            //}
        }
        else
        {
            float maxWheelDist = 0.05f;
            float liftFromGroundBack = Mathf.Max(0, maxWheelDist + groundMargin - hitDists[1]);
            float liftFromGroundFront = Mathf.Max(0, maxWheelDist + groundMargin - hitDists[0]);

            // Tilting effect -- TODO: uncomment this when you figure out the proper method of rotation
            //if (FrontHit && !BackHit && Mathf.Abs(wheelDelta) < maxWheelDist) backDy = -maxWheelDist + liftFromGroundBack;
            //if (!FrontHit && BackHit && Mathf.Abs(wheelDelta) < maxWheelDist) frontDy = -maxWheelDist + liftFromGroundFront;

            bodyDy = 0.0f;
        }

        // let the Slerp handle the tilts?
        FrontWheelPos.y += frontDy;
        BackWheelPos.y += backDy;
        // Gravity falling
        tankNewPos.y += bodyDy * Time.deltaTime;

        tank.FrontWheel.transform.position = FrontWheelPos;
        tank.BackWheel.transform.position = BackWheelPos;
        tank.transform.position = tankNewPos;
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

    // Not my code!
    void CenterPivotToChildren()
    {
        if (tank.transform.childCount == 0) return;

        Vector3 center = Vector3.zero;
        foreach (Transform child in tank.transform)
        {
            center += child.position;
        }
        center /= tank.transform.childCount;

        Vector3 offset = tank.transform.position - center;

        foreach (Transform child in tank.transform)
        {
            child.position += offset;
        }

        tank.transform.position = center;
    }
}