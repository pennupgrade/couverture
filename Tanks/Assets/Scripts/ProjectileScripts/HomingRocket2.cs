// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using UnityEngine;

public class HomingRocket2 : Projectile
{
    private GameObject player;
    private Rigidbody rb;

    private bool disabled, stopHoming;

    private float homingStr, Cturn;

    // Start is called before the first frame update
    protected override void Awake() {
        base.Awake();
        disabled = true;
        bulletSpeed = 4f;
        homingStr = 30;
        Cturn = 0;
        stopHoming = false;
    }
    public void setPlayer(GameObject p) {
        this.player = p;
    }

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    protected override void Update() {
        lifetime -= Time.deltaTime;
        if (lifetime < 0) Destroy(gameObject);

        if (player == null) {Cturn = 0; return;}

        if (disabled && (Vector3.Distance(player.transform.position, transform.position) < 2.4f ||
                         Vector3.Dot(player.transform.position - transform.position, transform.forward) < 0.1f)) {
            homingStr = 160;
            disabled = false;
        }
        else if (!disabled) {
            bulletSpeed = Mathf.Max(bulletSpeed - 2 * Time.deltaTime, 3);
        }

        if (!stopHoming && Vector3.Distance(player.transform.position, transform.position) < 0.9f) {
            stopHoming = true;
            homingStr = 20;
        }


        var v = player.transform.position - transform.position;
        var dot = Vector3.Dot(transform.right, new Vector3(v.x, 0, v.z).normalized);
        if (dot > 0.04f) {
            Cturn = homingStr;
        }
        else if (dot < -0.04f) {
            Cturn = -homingStr;
        }
        else {
            Cturn = 0;
        }
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
}