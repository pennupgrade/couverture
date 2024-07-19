using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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



    void Update()
    {
        Vector2 moveDir = controls.TankControls.Move.ReadValue<Vector2>();
        Vector2 gunRot = controls.TankControls.MousePos.ReadValue<Vector2>();

        tankState = tankState.HandleMovement(moveDir);
        tankState = tankState.HandleGunRotation(gunRot);
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

    public virtual TankState HandleGunRotation(Vector2 val) {
        // Debug.Log(Camera.main);
        Ray ray = Camera.main.ScreenPointToRay(val);

        Plane plane = new Plane(Vector3.up, tank.gun.transform.position);

        float dist;
        plane.Raycast(ray, out dist);

        Vector3 point = ray.GetPoint(dist);

        Vector3 offset = (point - tank.gun.transform.position).normalized;
        Vector3 dir = new (offset.x, 0, offset.z);

        float angle = Vector3.SignedAngle(Vector3.right, dir, Vector3.up);

        Debug.Log(angle);

        Debug.DrawRay(tank.gun.transform.position, point - tank.gun.transform.position, UnityEngine.Color.green);

        tank.gun.transform.rotation = Quaternion.Euler(0, angle, 0);

        // Debug.Log(dir);

        return this;
    }

    public abstract TankState HandleShoot();

}