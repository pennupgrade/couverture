using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Tank : MonoBehaviour
{

    // Base Items
    Controls controls; 
    public TankState tankState;


    // Necessary Components
    [HideInInspector]
    public Rigidbody rb;

    [HideInInspector]
    public Collider tankCollider;


    // Config Variables
    public float moveSpeed = 4;
    public float rotSpeed = 0.3f;
    public float gunRotSpeed = 3;
    public float bulletSpeed = 3;
    public float shotCooldownTime = 10f;


    // Object References
    public GameObject gun;
    public GameObject bulletPrefab;
    public Transform gunShotPos;
    public GameObject FrontWheel;
    public GameObject BackWheel;
    public GameObject Body;




    void Update()
    {
        Vector2 moveDir = controls.TankControls.Move.ReadValue<Vector2>();
        float gunRot = controls.TankControls.RotateGun.ReadValue<float>();

        tankState = tankState.HandleMovement(moveDir);
        tankState = tankState.HandleGunRotation(gunRot);

        Debug.Log("TankState: " + tankState.GetType());
    }



    //--------------------------- HOUSEKEEPING ---------------------------------------------
    
    private void Awake() {
        controls = new Controls();
        tankState = new TankIdleState(this);

        rb = GetComponent<Rigidbody>();
        tankCollider = GetComponent<BoxCollider>();

        controls.TankControls.Shoot.performed += _ => { tankState = tankState.HandleShoot(); };
    }

    private void OnEnable() {
        controls.Enable();
    }

    private void OnDisable() {
        controls.Disable();
    }

}




public abstract class TankState {
    
    protected Tank tank;

    public TankState(Tank tank) {
        this.tank = tank;
    }

    public abstract TankState HandleMovement(Vector2 dir);

    public abstract TankState HandleGunRotation(float val);

    public abstract TankState HandleShoot();

}