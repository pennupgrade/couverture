using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Tank : MonoBehaviour, IDestroyable
{
    public const float RELOAD_TIME = 1.8f;
    public const float COOLDOWN_TIME = 0.16f;

    public int LIVES = 5;

    // Base Items
    public Controls controls;
    public TankState tankState;
    public TankController tankController;

    // Necessary Components
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Collider tankCollider;

    // Config Variables
    public float moveSpeed;
    public float rotSpeed;
    public float groundMargin = 0.2f;
    public float wheelMaxDist = 3.0f;
    public bool enableGod = false;

    // Object References
    public GameObject bulletPrefab;
    public GameObject explosionPrefab;
    public GameObject gun;
    public Transform gunShotPos;
    public GameObject FrontWheel;
    public GameObject BackWheel;
    public GameObject Body;

    // Misc
    public int health;
    public int numBullets;
    public bool stunned;
    public Vector3 Velocity;
    private DamageFlash damageFlash;
    public float reloadProgress;
    public float cooldownProgress;

    // Couroutine Garbage
    public Coroutine reloadCoroutine, cooldownCoroutine;

    //--------------------------- HOUSEKEEPING ---------------------------------------------

    private void Awake() {
        controls = new Controls();
        tankState = new TankIdleState(this);
        tankController = new TankController(this);
        damageFlash = new DamageFlash(Body);

        rb = GetComponent<Rigidbody>();
        // tankCollider = GetComponent<BoxCollider>();

        controls.TankControls.Shoot.performed += _ => { tankState = tankState.HandleShoot(); };

        numBullets = 5;
    }


    private void Update() {
        var moveDir = controls.TankControls.Move.ReadValue<Vector2>();
        var gunRot = controls.TankControls.MousePos.ReadValue<Vector2>();

        tankController.DebugSomeStuff();

        tankState = tankState.HandleMovement(moveDir);
        tankState = tankState.HandleGunRotation(gunRot);

        // if (!isReloading && numBullets < 4) {
        //     StartCoroutine(reloadMagazine());
        // }
    }

    private void OnEnable() {
        controls.Enable();
    }

    private void OnDisable() {
        controls.Disable();
    }

    public void takeDamage(int dmg) {
        if (enableGod) return;

        health -= (dmg < 500) ? 100 : dmg;
        damageFlash.CallDamageFlash(this);
        if (health <= 0) {
            if (explosionPrefab != null) {
                var expl = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                Destroy(expl, 2);
            }

            GameManager.Instance.livesManager.LoseLife();

            var respawnTime = GameManager.Instance.livesManager.GetRespawnTime();
            Camera.main!.GetComponent<PlayerCamera>().Kill(respawnTime);

            gameObject.SetActive(false);
        }
    }

    public void Dissolve(float dissolveTime)
    {
        damageFlash.CallDissolve(this, dissolveTime);
    }

    public void incapacitate(float time) {
        StartCoroutine(stunCoroutine(time));
    }

    private IEnumerator stunCoroutine(float time) {
        stunned = true;
        damageFlash.CallElectricity(this, time);
        yield return new WaitForSeconds(time);
        stunned = false;
    }
}

public abstract class TankState
{
    protected Tank tank;

    public TankState(Tank tank) {
        this.tank = tank;
    }

    public abstract TankState HandleMovement(Vector2 dir);

    public virtual TankState HandleGunRotation(Vector2 val) {
        // Debug.Log(Camera.main);
        var ray = Camera.main.ScreenPointToRay(val);

        var plane = new Plane(Vector3.up, tank.gun.transform.position);

        float dist;
        plane.Raycast(ray, out dist);

        var point = ray.GetPoint(dist);

        var offset = (point - tank.gun.transform.position).normalized;
        Vector3 dir = new(offset.x, 0, offset.z);

        var angle = Vector3.SignedAngle(-tank.transform.right, dir, Vector3.up);

        // Debug.Log(angle);

        Debug.DrawRay(tank.gun.transform.position, point - tank.gun.transform.position, Color.green);

        tank.gun.transform.localRotation = Quaternion.Euler(0, angle, 0);

        // Debug.Log(dir);

        return this;
    }

    protected bool spawnInsideWallCheck() {
        return
            Physics.Raycast(tank.gameObject.transform.position,
                            tank.gunShotPos.position - tank.gameObject.transform.position,
                            Vector3.Distance(tank.gameObject.transform.position, tank.gunShotPos.position), 1 << 3);
    }

    public virtual TankState HandleShoot() {
        if (tank.numBullets <= 0 || tank.cooldownCoroutine != null || spawnInsideWallCheck()) return this;

        tank.numBullets--;
        tank.cooldownCoroutine = tank.StartCoroutine(Cooldown());
        var bullet = Object.Instantiate(tank.bulletPrefab, tank.gunShotPos.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(0, Vector3.up)
                                                    * (tank.gun.transform.right *
                                                       -bullet.GetComponent<Projectile>().bulletSpeed);
        bullet.GetComponent<Bullet_Default>().addBounceChange();
        bullet.transform.rotation = Quaternion.LookRotation(bullet.GetComponent<Rigidbody>().velocity);

        // Reload bullets if we're not already doing so
        if (tank.reloadCoroutine == null) tank.reloadCoroutine = tank.StartCoroutine(Reload());
        return this;
    }

    public virtual IEnumerator Cooldown() {
        tank.cooldownProgress = 0f;

        while (tank.cooldownProgress <= Tank.COOLDOWN_TIME) {
            tank.cooldownProgress += Time.deltaTime;
            yield return null;
        }

        tank.cooldownCoroutine = null;
        yield return null;
    }

    public virtual IEnumerator Reload() {
        while (tank.numBullets < 5) {
            tank.reloadProgress = 0f;

            while (tank.reloadProgress <= Tank.RELOAD_TIME) {
                tank.reloadProgress += Time.deltaTime;
                yield return null;
            }

            tank.numBullets++;
        }

        tank.reloadCoroutine = null;
        yield return null;
    }
}