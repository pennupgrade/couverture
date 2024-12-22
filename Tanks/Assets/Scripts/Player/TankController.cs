using System.Collections;
using UnityEngine;

// I'm moving all of the wheel stuff into this class, this should really be separated
// Building this class to reduce redundancies in our state code.
// Should this be renamed y'all? - Anthony

enum WheelID : ushort
{
    Front = 0,
    Back = 1,
    Left = 2,
    Right = 3
}

public class TankController
{
    private Tank tank;

    // Important vectors
    private Vector3 bodyForward;
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
        Vector3 FrontWheelPos = tank.Wheels[(int)WheelID.Front].transform.position;
        Vector3 BackWheelPos = tank.Wheels[(int)WheelID.Back].transform.position;

        // vector representing "forward" vector of the tank
        bodyForward = Vector3.Normalize(FrontWheelPos - BackWheelPos);

        // Position the body in the middle of the two wheels
        float wheelDistances = Vector3.Distance(FrontWheelPos, BackWheelPos);
        bodyPivot = 0.50f * wheelDistances * bodyForward + BackWheelPos;

        // Some vector math to get the body's normal (does Unity have a function for this?)
        bodyNormal = Vector3.Cross(bodyForward, tank.transform.up);
        bodyNormal = Vector3.Normalize(Vector3.Cross(bodyNormal, bodyForward));
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
        FrontHit = RaycastWheel(tank.Wheels[(int)WheelID.Front], 0);
        BackHit = RaycastWheel(tank.Wheels[(int)WheelID.Back], 1);
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
        GameObject wheel = tank.Wheels[id];
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
    private void TransformBody()
    {

        // Find the angle between two points (wheels), and then rotates body
        hitNormal = Vector3.Normalize(hitPoints[0].normal + hitPoints[1].normal);
        forward = Quaternion.AngleAxis(90, hitNormal) * bodyForward;
        desiredForward = Vector3.ProjectOnPlane(forward, hitNormal).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(desiredForward, hitNormal);

        tank.Body.transform.rotation = Quaternion.Slerp(tank.Body.transform.rotation, targetRotation, Time.deltaTime * 20.0f);

        // Use this to translate based on normal
        Vector3 bodyPosition = bodyPivot;
        bodyPosition += bodyNormal * 0.045f;
        tank.Body.transform.position = bodyPosition;
    }

    // Use this to see your vector angles if you're having problems
    public void DebugSomeStuff()
    {
        Debug.DrawRay(bodyPivot, forward, UnityEngine.Color.red);
        Debug.DrawRay(bodyPivot, hitNormal, UnityEngine.Color.cyan);
        Debug.DrawRay(bodyPivot, desiredForward, UnityEngine.Color.magenta);
    }

    public void RotateTank(Vector3 playerInput, Vector3 direction)
    {
        Vector3 tankForward = bodyForward;
        Vector3 tankBackward = -bodyForward;
        Vector3 backDir = -direction;

        // Choose the target direction based on the angle between the player input and the tank's forward/backward vectors
        float angleForward = Vector3.Angle(playerInput, tankForward);
        float angleBackward = Vector3.Angle(playerInput, tankBackward);

        Vector3 targetDir = angleForward < angleBackward ? direction : backDir;
        Transform rotatingObject = tank.transform.Find("Wheels");

        // Rotate the tank towards the target direction
        Quaternion targetRotate = Quaternion.RotateTowards(
                rotatingObject.rotation,
                Quaternion.LookRotation(targetDir),
                playerInput.magnitude * tank.rotSpeed * Time.deltaTime
            );

        tank.transform.rotation = targetRotate;
    }

    // Self explanatory name, using a Vector2 direction for (x,y), we translate the tank based on the projected forward vector
    // Only interacts with the tank transforms -- do not touch the body in anyway
    public void TransformTank(Vector2 dir)
    {
        // Constructing the player input vector
        Vector3 playerInput = new Vector3(-dir.x, 0, -dir.y);
        Vector3 normalizedInput = Vector3.Normalize(playerInput);

        // Constructing the direction and inverse of the input direction and the tank's forward vector
        Vector3 direction = normalizedInput;

        bool rotate = false;
        float thetaFallOff = 1.0f;

        if (rotate) // disable for now, if this is a roomba then it doesn't matter
        {
            RotateTank(playerInput, direction);

            // Move as a function of e^-theta, where theta is the positive dot product between the player and target direction
            // This means the tank will start moving when it's finished rotating
            float theta = Vector3.Dot(direction, bodyForward);
            thetaFallOff = Mathf.Exp(-Mathf.Abs(theta));
        }

        tank.Velocity = tank.moveSpeed * normalizedInput * thetaFallOff;
        tank.transform.position += tank.Velocity * Time.deltaTime;
    }

    // Called in TankMoveState to move the tank
    public void MoveTank(Vector2 dir)
    {
        CalculateBodyProperties();
        TransformTank(dir);
        RaycastWheels();
        PositionWheels();
        TransformBody();

        //DebugSomeStuff();
    }
}
