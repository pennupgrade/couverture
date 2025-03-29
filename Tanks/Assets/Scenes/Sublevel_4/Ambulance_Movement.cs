using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ambulance_Movement : MonoBehaviour
{
    [Header("Speed Stats")]
    [SerializeField] private float acceleration = 10f; // Acceleration rate
    [SerializeField] private float maxSpeed = 200f; // Maximum speed
    [SerializeField] private float turnSpeed = 100f; // Steering sensitivity
    [SerializeField] private float brakeForce = 20f; // Braking power
    [SerializeField] private float friction = 0.98f; // Simulated drag
    [SerializeField] private float speedMultiplier = 2f; // Speed boost with Shift

    [Header("Boost")]
    [SerializeField] public float maxBoostFuel = 10f; // Max fuel for boosting
    [SerializeField] private float boostConsumptionRate = 1f; // Fuel usage per second
    [SerializeField] private float boostRefillRate = 5f; // Fuel refill rate at hospital
    public float currentBoostFuel;

    private const float dropoffRadius = 50.0f;

    public float currentSpeed = 0f;
    private float currentTurnSpeed = 0f;
    private Rigidbody rb;

    [Header("Lights")]
    [SerializeField] private GameObject lightSource;
    private float lightInterval = 0.37f;

    [Header("Game info")]
    //[SerializeField] public GameObject game;
    public GameObject destination;

    //private Game_2 gameScript;

    private CharacterController controller;
    private BoxCollider boxCollider;
    public LayerMask obstacleMask; // Set this in the inspector to only include walls

    private Vector3 velocity;
    public float gravity = 32f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask;
    private bool isGrounded;

    void Start()
    {
        //rb = GetComponent<Rigidbody>();
        //gameScript = game.GetComponent<Game_2>();
        //destination = gameScript.destination;
        currentBoostFuel = maxBoostFuel;
        //rb.freezeRotation = true; // Prevent the ambulance from tipping over
        StartCoroutine(Lights());
        controller = GetComponent<CharacterController>();
        boxCollider = GetComponent<BoxCollider>();
        controller.height = 6.5f;
        controller.center = new Vector3(0, 3.25f, 0);
        controller.slopeLimit = 50f;

    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.M)) {
            SceneManager.LoadScene(0);
        }
    }

    void FixedUpdate() {
        HandleMovement();
        RefillFuel();
        AlignWithGround();
        //velocity = controller.velocity;
        // Gravity Handling
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small offset to keep grounded
        }
        else
        {
            velocity.y -= 5 * gravity * Time.deltaTime;
        }

        // Move the ambulance
        //Vector3 move = transform.forward * currentSpeed * Time.deltaTime;
        Vector3 move = new Vector3(0, 0, 0);
        move.y = velocity.y * Time.fixedDeltaTime; // Apply gravity

        controller.Move(move);
    }

    void AlignWithGround()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f; // Small offset to avoid clipping
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, groundCheckDistance + 1f, groundMask))
        {
            // Get rotation aligning up to ground normal
            Quaternion groundTilt = Quaternion.FromToRotation(transform.up, hit.normal);

            // Target rotation while keeping forward direction
            Quaternion targetRotation = groundTilt * transform.rotation;

            // Smoothly transition to target rotation
            float rotationSpeed = 5f; // Adjust for more or less smoothing
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            // Reset rotation when not on a ramp
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, transform.eulerAngles.y, 0), Time.deltaTime * 0.5f);
        }
    }

    private void RefillFuel()
    {
        
    }


    

    void HandleMovement()
    {
        float moveInput = 0f;
        float turnInput = 0f;

        // Forward and backward movement
        if (Input.GetKey(KeyCode.W))
            moveInput = 1f;
        if (Input.GetKey(KeyCode.S))
            moveInput = -1f;

        // Steering logic
        if (Input.GetKey(KeyCode.A))
            turnInput = -1f * moveInput;
        if (Input.GetKey(KeyCode.D))
            turnInput = 1f * moveInput;

        // Boost logic
        bool isBoosting = Input.GetKey(KeyCode.LeftShift) && currentBoostFuel > 0;
        float speedFactor = isBoosting ? speedMultiplier : 1f;

        if (isBoosting)
        {
            currentBoostFuel -= boostConsumptionRate * Time.fixedDeltaTime;
            currentBoostFuel = Mathf.Max(currentBoostFuel, 0);
        }

        // Accelerate and decelerate
        currentSpeed += moveInput * acceleration * Time.fixedDeltaTime;
        currentSpeed *= speedFactor;
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        // Apply friction
        currentSpeed *= friction;

        if (Mathf.Abs(currentSpeed) < 0.05f) currentSpeed = 0f;

        // Braking
        if (Input.GetKey(KeyCode.S) && currentSpeed > 0)
            currentSpeed -= brakeForce * Time.fixedDeltaTime;

        // Steering based on speed (harder to turn at high speeds)
        if (currentSpeed != 0)
            currentTurnSpeed = turnInput * turnSpeed * (Mathf.Clamp01(10f / Mathf.Sqrt(Mathf.Abs(currentSpeed))));

        // Check if moving forward would collide with a wall
        if (!IsColliding(Vector3.forward * currentSpeed * Time.fixedDeltaTime))
        {
            Vector3 moveDirection = transform.forward * currentSpeed;
            controller.Move(moveDirection * Time.deltaTime);
        }

        // Apply rotation
        transform.Rotate(0, currentTurnSpeed * Mathf.Sqrt(speedFactor) * Time.fixedDeltaTime, 0);
    }

    

    // Collision check using BoxCollider
    bool IsColliding(Vector3 movement)
    {
        return Physics.BoxCast(transform.position, boxCollider.size * 0.5f, movement.normalized, Quaternion.identity, movement.magnitude, obstacleMask);
    }

    private IEnumerator Lights() {
        while (true)
        {
            yield return new WaitForSeconds(lightInterval);
            if (lightSource.activeSelf) {
                lightSource.SetActive(false);
            } else {
                lightSource.SetActive(true);
            }
        }
    }
}
