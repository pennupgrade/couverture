using System.Collections;
using UnityEngine;

public struct Wheel
{
    public GameObject obj;
    public RaycastHit hit;
    public bool isHit;
}

public class TankController
{
    private Tank tank;

    // Body Vectors
    private Vector3 bodyForward;
    private Vector3 bodyOrigin;
    private Vector3 bodyNormal;

    private Vector3 prevDir;
    private Vector3 hitNormal;

    int front = 0; // someone better than me at coding can refactor this :)
    int back = 1;

    Wheel[] wheels;

    public TankController(Tank tank)
    {
        this.tank = tank;
        wheels = new Wheel[tank.Wheels.Length];

        for (int i = 0; i < tank.Wheels.Length; i++)
        {
            wheels[i].obj = tank.Wheels[i];
        }
    }

    public void CalculateBodyVectors(Vector3 FrontWheelPos, Vector3 BackWheelPos)
    {
        bodyForward = Vector3.Normalize(FrontWheelPos - BackWheelPos);

        float wheelDistances = Vector3.Distance(FrontWheelPos, BackWheelPos);
        bodyOrigin = 0.50f * wheelDistances * bodyForward + BackWheelPos;

        bodyNormal = Vector3.Cross(bodyForward, tank.transform.up);
        bodyNormal = Vector3.Normalize(Vector3.Cross(bodyNormal, bodyForward));
    }

    public void RaycastWheel(ref Wheel wheel)
    {
        RaycastHit hit;
        Vector3 origin = wheel.obj.transform.position;
        Vector3 direction = -tank.transform.up;

        Physics.Raycast(origin, direction, out hit, tank.wheelMaxDist);

        if (hit.collider != null)
        {
            wheel.hit = hit;
            wheel.isHit = true;
            return;
        }

        wheel.isHit = false;
    }

    public void RaycastWheels()
    {
        RaycastWheel(ref wheels[front]);
        RaycastWheel(ref wheels[back]);
    }

    public bool IsAirborne()
    {
        RaycastWheels();
        return wheels[front].isHit && wheels[back].isHit;
    }

    private void PositionWheelByHit(ref Wheel wheel)
    {
        Vector3 origin = wheel.obj.transform.position;
        Vector3 hitPoint = wheel.hit.point;

        float hitDist = Vector3.Distance(hitPoint, origin);

        // project wheels onto ground, then lift up by gMargin
        if (hitDist <= tank.groundMargin + 0.02f)
        {
            hitPoint += tank.transform.up * tank.groundMargin;
            wheel.obj.transform.position = hitPoint;
        }
    }

    private void PositionWheels()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            if (wheels[i].isHit) PositionWheelByHit(ref wheels[i]);
        }
    }

    private void TransformBody()
    {
        if (wheels[front].isHit || wheels[back].isHit)
        {
            hitNormal = Vector3.Normalize(wheels[front].hit.normal + wheels[back].hit.normal);
        }

        Quaternion targetRotation = Quaternion.LookRotation(bodyForward, hitNormal);
        tank.Body.transform.rotation = Quaternion.Slerp(tank.Body.transform.rotation, targetRotation, Time.deltaTime * 20.0f);

        // Use this to translate based on normal
        Vector3 bodyPosition = bodyOrigin;
        bodyPosition += bodyNormal * 0.045f;
        tank.Body.transform.position = bodyPosition;
    }

    // Use this to see your vector angles if you're having problems
    public void DebugSomeStuff()
    {
        /*Debug.DrawRay(bodyOrigin, forward, UnityEngine.Color.red);
        Debug.DrawRay(bodyOrigin, hitNormal, UnityEngine.Color.cyan);
        Debug.DrawRay(bodyOrigin, desiredForward, UnityEngine.Color.magenta);*/
    }

    public void RotateWheels(Vector3 direction, float magnitude)
    {
        GameObject wheelsRef = tank.WheelsRef;
        Vector3 tankForward = wheelsRef.transform.forward;
        Quaternion targetQuat = Quaternion.LookRotation(direction);

        if (Vector3.Dot(direction, tankForward) < -0.99f) // identical angles
        {
            targetQuat = wheelsRef.transform.rotation;
        }

        // Rotate the tank towards the target direction
        Quaternion targetRotate = Quaternion.RotateTowards(
                wheelsRef.transform.rotation,
                targetQuat,
                magnitude * tank.rotSpeed * Time.deltaTime
            );

        wheelsRef.transform.rotation = targetRotate;
    }

    public void TransformTank(Vector2 dir)
    {
        // Constructing the player input vector
        Vector3 playerInput = new Vector3(-dir.x, 0, -dir.y);
        Vector3 direction = Vector3.Normalize(playerInput);

        float thetaFallOff = 1.0f;
        RotateWheels(direction, playerInput.magnitude);

        // Move as a function of e^-theta, where theta is the positive dot product between the player and target direction
        // This means the tank will start moving when it's finished rotating
        //float theta = Vector3.Dot(direction, bodyForward);
        //thetaFallOff = Mathf.Exp(-Mathf.Abs(theta));

        if (direction != prevDir)
        {
            prevDir = direction;
        }

        tank.Velocity = tank.moveSpeed * direction * thetaFallOff;
        tank.transform.position += tank.Velocity * Time.deltaTime;
    }

    // Called in TankMoveState to move the tank, dir.mag is > 0
    public void MoveTank(Vector2 dir)
    {
        CalculateBodyVectors(
            wheels[front].obj.transform.position, 
            wheels[back].obj.transform.position);
        TransformTank(dir);

        // Project the wheels onto the ground and adjust necessary positions
        RaycastWheels();
        PositionWheels();
        TransformBody();

        DebugSomeStuff();
    }
}
