using System.Collections;
using UnityEngine;

// I'm moving all of the wheel stuff into this class, this should really be separated
// Building this class to reduce redundancies in our state code.
// Should this be renamed y'all? - Anthony

public class TankController
{
    private Tank tank;

    // Important vectors
    private Vector3 bodyVector;
    private Vector3 bodyPivot;
    private Vector3 bodyNormal;

    // Keep these for debugging
    private Vector3 forward;
    private Vector3 hitNormal;
    private Vector3 desiredForward;

    // Array of size two keeping hitPoints for front (0) and back wheel (1)
    RaycastHit[] hitPoints;

    // Booleans to keep track of hitPoints, used in IsAirborne and other stuff
    private bool FrontHit;
    private bool BackHit;

    // Constructor
    public TankController(Tank tank)
    {
        this.tank = tank;
        hitPoints = new RaycastHit[2];
    }

    // Calculate properties related to the Tank's body orientation
    // Assigns normal of the body pointing up, forward vector of the tank, and the pivot position
    public void CalculateBodyProperties()
    {
        Vector3 FrontWheelPos = tank.FrontWheel.transform.position;
        Vector3 BackWheelPos = tank.BackWheel.transform.position;

        // vector representing "forward" vector of the tank
        bodyVector = Vector3.Normalize(FrontWheelPos - BackWheelPos);

        // Position the body in the middle of the two wheels
        float wheelDistances = Vector3.Distance(FrontWheelPos, BackWheelPos);
        bodyPivot = 0.50f * wheelDistances * bodyVector + BackWheelPos;

        // Some vector math to get the body's normal (does Unity have a function for this?)
        bodyNormal = Vector3.Cross(bodyVector, tank.transform.up);
        bodyNormal = Vector3.Normalize(Vector3.Cross(bodyNormal, bodyVector));
    }

    // Performs a raycast from the wheel onto the ground, returns a boolean as to whether or not there is a hit
    // Assigns hitPoints and returns bool for RaycastWheels()
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
            hitPoints[id] = hit;
            return true;
        }

        return false;
    }

    // Fire raycasts from wheels onto the ground
    // Assigns FrontHit and BackHit true/false
    public void RaycastWheels()
    {
        FrontHit = RaycastWheel(tank.FrontWheel, 0);
        BackHit = RaycastWheel(tank.BackWheel, 1);
    }

    // Checks if front and back wheels raycast hit the ground
    public bool IsAirborne()
    {
        RaycastWheels();
        return !FrontHit && !BackHit;
    }

    // Repositions a wheel based on its hit point from raycast (not physically-accurate suspension)
    private void PositionWheelByHit(int id)
    {
        GameObject wheel = id == 0 ? tank.FrontWheel : tank.BackWheel;
        Vector3 origin = wheel.transform.position;
        Vector3 hitPoint = hitPoints[id].point;

        float hitDist = Vector3.Distance(hitPoint, origin);

        if (hitDist <= tank.groundMargin + 0.02f)
        {
            // vertically translate the wheel from the ground
            hitPoint += tank.transform.up * tank.groundMargin; // 0.2f value is arbitrary, lifts up wheel
            wheel.transform.position = hitPoint;
        }
    }

    // Positions both wheels depending on whether or not they hit the ground
    private void PositionWheels()
    {
        if (FrontHit) PositionWheelByHit(0);
        if (BackHit) PositionWheelByHit(1);
    }

    // Rotates the body based on the forward vector of the Tank, calcualted by the wheels
    private void RotateBodyByWheels()
    {
        // Find the angle between two points (wheels), and then rotates body
        hitNormal = Vector3.Normalize(hitPoints[0].normal + hitPoints[1].normal);
        forward = Quaternion.AngleAxis(90, hitNormal) * bodyVector;
        desiredForward = Vector3.ProjectOnPlane(forward, hitNormal).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(desiredForward, hitNormal);

        tank.Body.transform.rotation = Quaternion.Slerp(tank.Body.transform.rotation, targetRotation, Time.deltaTime * 20.0f);

        // Use this to translate based on normal
        Vector3 bodyPosition = bodyPivot;
        bodyPosition += bodyNormal * -0.045f;
        tank.Body.transform.position = bodyPosition;
    }

    // Use this to see your vector angles if you're having problems
    public void DebugSomeStuff()
    {
        Debug.DrawRay(bodyPivot, forward, UnityEngine.Color.red);
        Debug.DrawRay(bodyPivot, hitNormal, UnityEngine.Color.cyan);
        Debug.DrawRay(bodyPivot, desiredForward, UnityEngine.Color.magenta);
    }

    // Self explanatory name, using a Vector2 direction for (x,y), we translate the tank based on the projected forward vector
    // We only rotate the body to give off the illusion that the tank is angled on the slope
    // When rotating the tank for movement, we rotate the transform such that its rotated angle is always aligned by the vertical axis
    // This simplifies calculation for forward movement later on
    public void TransformTank(Vector2 dir)
    {
        //Debug.DrawRay(bodyPivot, bodyVector * 10.0f, UnityEngine.Color.red);

        float rotSpeed = 200f;
        float moveSpeed = 1f;

        Vector3 playerInput = new Vector3(-dir.x, 0, -dir.y);
        Vector3 tankForward = Vector3.Normalize(bodyVector);
        Vector3 direction = Vector3.Normalize(playerInput);
        direction = Quaternion.AngleAxis(90, Vector3.up) * direction;

        // Rotation transformation
        Debug.DrawRay(bodyPivot, tankForward * 1f, Color.white);
        Debug.DrawRay(bodyPivot, direction * 1f, Color.yellow);

        Quaternion targetRotate = Quaternion.RotateTowards(
                tank.transform.rotation, 
                Quaternion.LookRotation(direction),
                playerInput.magnitude * rotSpeed * Time.deltaTime
            );

        tank.transform.rotation = targetRotate;

        // Translational transformation, move as a function of the angle between the player and target direction.
        // This means the tank won't move until it's finished rotating.
        float theta = Vector3.Dot(direction, tankForward);
        tank.transform.position += moveSpeed * direction * Mathf.Exp(-Mathf.Abs(theta)) * Time.deltaTime;
    }

    // Called in TankMoveState to move the tank
    public void MoveTank(Vector2 dir)
    {
        CalculateBodyProperties();
        TransformTank(dir);
        RaycastWheels();
        PositionWheels();
        RotateBodyByWheels();

        //DebugSomeStuff();
    }
}
