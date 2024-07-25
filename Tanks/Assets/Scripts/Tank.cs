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
    public float groundMargin = 0.2f;
    public float wheelMaxDist = 3.0f;
    public float gunRotSpeed = 3;
    public float bulletSpeed = 3;
    public float shotCooldownTime = 10f;
    public bool enableExperimentalGravity = true;


    // Object References
    public GameObject gun;
    public GameObject bulletPrefab;
    public Transform gunShotPos;
    public GameObject FrontWheel;
    public GameObject BackWheel;
    public GameObject Body;


    // Misc
    [SerializeField] private int health;
    public int numBullets;


    // Couroutine Garbage
    public Coroutine activeBulletCoroutine;


    void Update()
    {
        Vector2 moveDir = controls.TankControls.Move.ReadValue<Vector2>();
        Vector2 gunRot = controls.TankControls.MousePos.ReadValue<Vector2>();

        tankState = tankState.HandleMovement(moveDir);
        tankState = tankState.HandleGunRotation(gunRot);

        // if (!isReloading && numBullets < 4) {
        //     StartCoroutine(reloadMagazine());
        // }
    }

    // private IEnumerator reloadMagazine() {
    //     isReloading = true;
    //     while (numBullets < 4) {
    //         yield return new WaitForSeconds(2);
    //         numBullets++;
    //         if (numBullets == 4) {
    //             isReloading = false;
    //             yield break;
    //         }
    //     }
    // }

    // public void takeDamage(int dmg) {
    //     health -= dmg;
    //     if (health <= 0) {
    //         //Destroy(gameObject);
    //         Debug.Log("You died");
    //     } 
    // }


    //--------------------------- HOUSEKEEPING ---------------------------------------------

    private void Awake() {
        controls = new Controls();
        tankState = new TankIdleState(this);

        rb = GetComponent<Rigidbody>();
        // tankCollider = GetComponent<BoxCollider>();

        controls.TankControls.Shoot.performed += _ => { tankState = tankState.HandleShoot(); };

        numBullets = 5;
        health = 100;
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

        float angle = Vector3.SignedAngle(-tank.transform.right, dir, Vector3.up);

        // Debug.Log(angle);

        Debug.DrawRay(tank.gun.transform.position, point - tank.gun.transform.position, UnityEngine.Color.green);

        tank.gun.transform.localRotation = Quaternion.Euler(0, angle, 0);

        // Debug.Log(dir);

        return this;
    }

    public virtual TankState HandleShoot() { 
        if (tank.numBullets <= 0) {
            return this;
        }

        tank.numBullets--;
        GameObject bullet = Object.Instantiate(tank.bulletPrefab, tank.gunShotPos.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(30 * (Random.value - 0.5f), Vector3.up)
         * (tank.gun.transform.right * tank.bulletSpeed);

        if (tank.activeBulletCoroutine != null) {
            tank.StopCoroutine(tank.activeBulletCoroutine);
        }
        tank.activeBulletCoroutine = tank.StartCoroutine(Reload());

        return this;
    }

    public virtual IEnumerator Reload() {
        Debug.Log("Begun Reload");

        while(tank.numBullets < 5) {
            yield return new WaitForSeconds(1);

            tank.numBullets++;
            Debug.Log("Reloaded to bullets: " + tank.numBullets);
        }

        tank.activeBulletCoroutine = null;
    }

}

interface IDestroyable {
    void takeDamage(int dmg);
}