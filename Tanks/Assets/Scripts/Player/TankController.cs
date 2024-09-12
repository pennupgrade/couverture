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

        if (hitDist <= tank.groundMargin + 0.02f || !tank.enableExperimentalGravity)
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
        //Debug.DrawRay(bodyPivot, bodyVector, UnityEngine.Color.red);

        //// Find the angle between two points (wheels), and then rotates body
        //hitNormal = Vector3.Normalize(hitPoints[0].normal + hitPoints[1].normal);
        //forward = Quaternion.AngleAxis(90, hitNormal) * bodyVector;
        //desiredForward = Vector3.ProjectOnPlane(forward, hitNormal).normalized;
        //Quaternion targetRotation = Quaternion.LookRotation(desiredForward, hitNormal);

        //tank.Body.transform.rotation = Quaternion.Slerp(tank.Body.transform.rotation, targetRotation, Time.deltaTime * 20.0f);

        //// Use this to translate based on normal
        //Vector3 bodyPosition = bodyPivot;
        //bodyPosition += bodyNormal * -0.045f;
        //tank.Body.transform.position = bodyPosition;
    }

    // Use this to see your vector angles if you're having problems
    public void DebugSomeStuff()
    {
        Debug.DrawRay(bodyPivot, forward, UnityEngine.Color.red);
        Debug.DrawRay(bodyPivot, hitNormal, UnityEngine.Color.cyan);
        Debug.DrawRay(bodyPivot, desiredForward, UnityEngine.Color.magenta);
    }

    // Self explanatory name, using a Vector2 direction for (x,y), we translate the tank based on the projected forward vector
    // We do this because we separate the body from the rest in terms of orientation, wheels should never be rotated along with
    // the tank object itself, only the body. Thus, we only want to translate the tank and "rotate" to TURN in X,Y space only, not Z
    public void TranslateTank(Vector2 dir)
    {
        //Vector3 direction = Vector3.ProjectOnPlane(bodyVector, tank.transform.up);
        //tank.transform.position += tank.moveSpeed * direction * dir.y * Time.deltaTime;
        //tank.Velocity = tank.moveSpeed * direction * dir.y;
        //tank.transform.RotateAround(bodyPivot, tank.transform.up, tank.rotSpeed * dir.x * Time.deltaTime);


        float rotSpeed = 200f;
        float moveSpeed = 7f;

        Vector3 playerInput = new Vector3(-dir.x, 0, -dir.y);
        Vector3 tankForward = -tank.Body.transform.right;


        Debug.DrawRay(bodyPivot, playerInput * 10f, Color.yellow);
        Debug.DrawRay(bodyPivot, tankForward * 10f, Color.magenta);



        // Calculate the angles for both forward and backward directions
        float angleForward = Vector3.Angle(playerInput, tankForward);
        float angleBackward = Vector3.Angle(playerInput, -tankForward);

        // Determine the optimal direction to move towards
        bool faceBackward = angleBackward < angleForward;

        // Calculate the target rotation based on the optimal direction
        Vector3 targetDirection = faceBackward ? -playerInput : playerInput;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        // Rotate towards the target rotation. The speed is a function of the player input magnitude. Light player input still influences movement.
        tank.Body.transform.rotation = Quaternion.RotateTowards(tank.Body.transform.rotation, targetRotation, playerInput.magnitude * rotSpeed * Time.deltaTime);

        // Move as a function of the angle between the player and target direction. This means the tank won't move until it's finished rotating.
        //tank.Body.transform.position += moveSpeed * playerInput * Mathf.Exp(-Mathf.Min(angleForward, angleBackward)) * Time.deltaTime;
    }

    // Called in TankMoveState to move the tank
    public void MoveTank(Vector2 dir)
    {
        CalculateBodyProperties();
        TranslateTank(dir);
        RaycastWheels();
        PositionWheels();
        RotateBodyByWheels();
    }
}
