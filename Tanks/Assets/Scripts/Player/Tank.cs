using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Tank : MonoBehaviour, IDestroyable
{
    public const float RELOAD_TIME = 1.5f;
    public const float COOLDOWN_TIME = 0.18f;

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
    public GameObject[] Wheels;
    public GameObject Body;
    public Animator cannonAnimator;
    public Character charType;

    // Misc
    public int health;
    public int numBullets;
    public bool stunned;
    public Vector3 Velocity;
    private DamageFlash damageFlash;
    public float reloadProgress;
    public float cooldownProgress;
    public float animationProgress;

    // Couroutine Garbage
    public Coroutine reloadCoroutine, cooldownCoroutine;


    //effects
    private List<TimedEffect> effects = new();

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



        //Temporary TimedEffect
        TimedEffect effect = new(15.0f, 
            (tank) => {
                tank.moveSpeed *= 5.0f;
            },
            (tank) => {
                tank.moveSpeed /= 5.0f;
            }
        );

        addEffect(effect);
    }




    ~Tank() {
        effects.ForEach(x => x.Kill(this));
    }


    public void addEffect(TimedEffect timedEffect) {
        effects.Add(timedEffect);
        timedEffect.Start(this);
    }


    private void Update() {
        var moveDir = controls.TankControls.Move.ReadValue<Vector2>();
        var gunRot = controls.TankControls.MousePos.ReadValue<Vector2>();

        tankController.DebugSomeStuff();

        tankState = tankState.HandleMovement(moveDir);
        tankState = tankState.HandleGunRotation(gunRot);

        for(int i = 0; i < effects.Count; i++) {
            if (!effects[i].enabled) {
                effects.RemoveAt(i);
            }
        }

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