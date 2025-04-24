using UnityEngine;

public class HomingRocket : Projectile
{
    public GameObject player;
    private Rigidbody rb;

    private bool disabled;

    private float homingStr, Cturn, turnTimer;

    // Start is called before the first frame update
    protected override void Awake() {
        base.Awake();
        bulletSpeed = 3f;
        turnTimer = 0.1f;
        homingStr = 160;
        disabled = false;
        Cturn = Random.value < 0.5f ? 50 : -50;
    }

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    protected override void Update() {
        lifetime -= Time.deltaTime;
        if (lifetime < 0) Destroy(gameObject);

        if (player == null) return;

        if (disabled) {
            bulletSpeed += Time.deltaTime * 2;
        }
        else {
            bulletSpeed += Time.deltaTime * 0.33f;
        }

        if (!disabled && Vector3.Distance(player.transform.position, transform.position) < 1.5f) {
            homingStr = 30;
            disabled = true;
            return;
        }


        if (turnTimer < 0.01f) {
            var v = player.transform.position - transform.position;
            if (Vector3.Dot(transform.right, new Vector3(v.x, 0, v.z).normalized) > 0) {
                Cturn = homingStr;
            }
            else {
                Cturn = -homingStr;
            }

            turnTimer = 0.3f;
        }

        turnTimer = TimerF(turnTimer);
    }

    // Update is called once per frame
    private void FixedUpdate() {
        transform.eulerAngles += Cturn * Time.fixedDeltaTime * Vector3.up;
        rb.MovePosition(rb.position + Time.fixedDeltaTime * bulletSpeed * transform.forward);
    }

    private void OnCollisionEnter(Collision collision) {
        if (defaultCollisionChecks(collision)) return;

        if (collision.gameObject.tag == "Environment" || collision.gameObject.tag == "Untagged") {
            destruction();
            return;
        }

        var wallNormal = collision.contacts[0].normal;
        var bulletDir = rb.velocity.normalized;

        if (collision.gameObject.tag == "OneWay") {
            if (Vector3.Dot(bulletDir, wallNormal) > 0) // Angle check to see if bullet is behind wall
            {
                return;
            }

            destruction();
        }
    }

    protected override void removeObjectFromGame() {
        GetComponent<Animator>().Play("DefaultBulletFadeOut");
        rb.velocity = Vector3.zero;
        GetComponent<Collider>().enabled = false;
        enabled = false;
        audioManager.Stop("Rocket");

        Destroy(gameObject, 0.25f);
    }

    private float TimerF(float val) {
        if (val > 0) {
            val -= Time.deltaTime;
            if (val <= 0) val = 0;
        }

        return val;
    }
}