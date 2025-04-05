using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Tank : MonoBehaviour, IDestroyable
{
    public Character character = null;

    public const float RELOAD_TIME = 1.5f;
    public const float COOLDOWN_TIME = 0.18f;

    // Base Items
    public Controls controls;
    public TankState tankState;
    public TankCharacterController tankController;
    public AudioManager audioManager;

    // Necessary Components
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Collider tankCollider;
    [HideInInspector] public CharacterController characterController;

    // Config Variables
    private bool invincible;
    public float moveSpeed;
    public float rotSpeed;
    public float groundMargin;
    public float wheelMaxDist;
    public bool enableGod = false;

    // Object References
    public GameObject explosionPrefab;
    public GameObject gun;
    public Transform gunShotPos;
    public GameObject Body, Roomba;
    public Animator cannonAnimator;

    public const int MAX_BULLETS = 5;

    // Misc
    [HideInInspector] public int maxHealth;
    public int health;
    public int numBullets;
    public bool stunned;
    public Vector3 Velocity;
    private DamageFlash damageFlash;
    public float reloadProgress;
    public float cooldownProgress;
    public float animationProgress;
    public Vector3 forward;
    public Vector3 right;

    private Vector3 currentPos = Vector3.zero;
    private Vector3 previousPos = Vector3.zero;

    public float platformSpeed;

    // Couroutine Garbage
    public Coroutine reloadCoroutine, cooldownCoroutine;

    //effects
    private List<TimedEffect> effects = new();

    //--------------------------- HOUSEKEEPING ---------------------------------------------

    private void Awake() {
        controls = new Controls();
        tankState = new TankIdleState(this);
        tankController = new TankCharacterController(this);
        damageFlash = new DamageFlash(Body);

        forward = transform.forward;
        right = transform.right;

        rb = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
        // tankCollider = GetComponent<BoxCollider>();

        controls.TankControls.Shoot.performed += _ => {
            //If player is moving, bullet speed can be affected
            Vector3 deltaPos = currentPos - previousPos;

            //Debug.Log(platformSpeed);

            tankState = tankState.HandleShoot(platformSpeed * deltaPos / Time.deltaTime);
        };

        numBullets = MAX_BULLETS;
        maxHealth = health;


        //Temporary TimedEffect
        // TimedEffect effect = new(15.0f, 
        //     (tank) => {
        //         tank.moveSpeed *= 5.0f;
        //     },
        //     (tank) => {
        //         tank.moveSpeed /= 5.0f;
        //     }
        // );
        // addEffect(effect);
    }

    public void Freeze() {
        controls.Disable();
        invincible = true;
    }

    public void Unfreeze() {
        invincible = false;
        controls.Enable();
    }

    // Effects
    ~Tank() {
        effects.ForEach(x => x.Kill(this));
    }


    public void addEffect(TimedEffect timedEffect) {    
        effects.Add(timedEffect);
        timedEffect.Start(this);
    }

    // Platform Velocity Funcs
    public void SetPlatformSpeed(float delta)
    {
        platformSpeed = Mathf.Clamp(platformSpeed + delta, 0.0f, 1.1f); // temp clamp
        Debug.Log("Change: platformSpeed: " + (platformSpeed));
    }


    private void Update() {
        var moveDir = controls.TankControls.Move.ReadValue<Vector2>();
        var gunRot = controls.TankControls.MousePos.ReadValue<Vector2>();

        tankController.RayCastTank();
        tankController.GravityFall();
        bool wasIdle = tankState is TankIdleState;
        tankState = tankState.HandleMovement(moveDir);
        if (wasIdle && tankState is TankMoveState) {
            audioManager.Play("Engine");
        } else if (!wasIdle && tankState is TankIdleState){
            audioManager.Stop("Engine");
        }
        tankState = tankState.HandleGunRotation(gunRot);

        for(int i = 0; i < effects.Count; i++) {
            if (!effects[i].enabled) {
                effects.RemoveAt(i);
            }
        }

        previousPos = currentPos;//this gives them a single-tick of delta difference
        currentPos = transform.position;
        
        // FOR TESTING PURPOSES, SHOULD BE REMOVED
        /*if(Input.GetKeyDown(KeyCode.Z)) {
            Debug.Log("Adding Speed");

            //Temporary TimedEffect
            TimedEffect effect = new(15.0f, 
                (tank) => {
                    tank.moveSpeed *= 2.0f;
                },
                (tank) => {
                    tank.moveSpeed /= 2.0f;
                }
            );

            addEffect(effect);
        }*/

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
        if (enableGod || invincible) return;

        health -= (dmg < 600) ? 100 : dmg;
        damageFlash.CallDamageFlash(this);
        if (health <= 0) {
            if (explosionPrefab != null) {
                var expl = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                audioManager.Play("Explosion");
                Destroy(expl, 2);
            }

            Freeze();
            GameManager.Instance.Respawn();

            var respawnTime = GameManager.Instance.GetRespawnTime();
            Camera.main!.GetComponent<PlayerCamera>().Kill(respawnTime);

            gameObject.SetActive(false);
        }
        else if (Random.value < 0.5f) {
            audioManager.Play("Meow");
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

    // Character abilities
    public bool Ability() {
        if (character != null) {
            return character.Ability(this);
        }
        else
        {
            return false;
        }
    }
    public void AbilityUpdate() {
        if (character != null) {
            character.AbilityUpdate(this);
        }
    }

    // spawn a base bullet, override if different base bullet
    public GameObject SpawnBullet() {
        return PoolManager.bulletPool.Get().gameObject;
    }
    public void ResetPosition(Vector3 pos) {
        previousPos = pos;
        currentPos = pos;
        print("RESET" + previousPos + " " + currentPos);
    }
}