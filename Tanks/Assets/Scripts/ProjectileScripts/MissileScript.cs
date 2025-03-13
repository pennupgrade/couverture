using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileScript : MonoBehaviour
{
    public GameObject missileBody;
    public GameObject targetZone;
    public GameObject damageZone;

    private float g = -20f;

    public int damage;
    public Vector3 startLocation;
    public Vector3 endLocation;
    public float secondsInAir; //must be positive

    private bool hitTarget;
    private Vector3 acceleration;
    private Vector3 velocity;


    //visual and auditory effects
    private AudioSource explosionSound;
    private ParticleSystem explosionParticles;
    private Renderer targetZoneRenderer;

    private Color initialTargetZoneColor = new Color(1f, 0, 0, 0);
    private Color finalTargetZoneColor = new Color(1f, 0, 0, 1f);
    private Color flashTargetZoneColor = new Color(1f, 1f, 0, 1f);

    private float colorLerpTime;
    private float flashingTimer;
    private float explosionTime = 1f;

    private bool initialized = false;

    void Start()
    {
        
    }

    public void initialize(Vector3 startPosition, Vector3 endPosition)
    {
        startLocation = startPosition;
        gameObject.transform.position = startPosition;

        endLocation = endPosition;
        targetZone.transform.position = endPosition;
        secondsInAir = Mathf.Abs(secondsInAir); //avoid negative time

        hitTarget = false;
        acceleration = new Vector3(0, g, 0);
        explosionSound = gameObject.GetComponent<AudioSource>();
        explosionParticles = damageZone.GetComponent<ParticleSystem>();

        var particleSystemMain = explosionParticles.main;
        particleSystemMain.startLifetime = explosionTime;

        targetZoneRenderer = targetZone.GetComponent<Renderer>();
        targetZoneRenderer.material.color = initialTargetZoneColor;

        colorLerpTime = 0;
        flashingTimer = 0;

        //calculate initial velocity
        float initialVelocityX = (endLocation.x - startLocation.x) / secondsInAir;
        float initialVelocityZ = (endLocation.z - startLocation.z) / secondsInAir;

        float displacementY = endLocation.y - startLocation.y;
        float initialVelocityY = (float)((displacementY - 0.5 * g * Mathf.Pow(secondsInAir, 2)) / secondsInAir);
        velocity = new Vector3(initialVelocityX, initialVelocityY, initialVelocityZ);

        missileBody.transform.rotation = Quaternion.LookRotation(new Vector3(0, 1, 0)); //initially facing upward

        initialized = true;
    }

    void Update()
    {
        if (!initialized)
        {
            return;
        }

        if (!hitTarget)
        {
            missileBody.transform.position += velocity * Time.deltaTime;
            velocity += acceleration * Time.deltaTime;                          //applies constant downward acceleration
            missileBody.transform.rotation = Quaternion.LookRotation(velocity); //rotates the badminton
        } else
        {
            velocity = new Vector3(0, 0, 0);
            damageZone.transform.position = endLocation;
        }


        //targetZone gradually reaches full opacity after 80% of secondsInAir
        if (colorLerpTime < 1)
        {
            colorLerpTime += Time.deltaTime / (0.8f * secondsInAir);
            targetZoneRenderer.material.color =
                Color.Lerp(initialTargetZoneColor, finalTargetZoneColor, colorLerpTime);
        } else if (flashingTimer < 1)
        {
            //targetZone starts flashing for the last 20% of secondsInAir
            flashingTimer += Time.deltaTime / (0.2f * secondsInAir);
            if (
                (flashingTimer > 0.1 && flashingTimer < 0.2)
                || (flashingTimer > 0.4 && flashingTimer < 0.5)
                || (flashingTimer > 0.6 && flashingTimer < 0.7)
                || (flashingTimer > 0.8 && flashingTimer < 0.85)
                || (flashingTimer > 0.9 && flashingTimer < 0.95)
                || flashingTimer > 0.975
                )
            {
                targetZoneRenderer.material.color = flashTargetZoneColor;
            } else
            {
                targetZoneRenderer.material.color = finalTargetZoneColor;
            }
        }
    }

    public void handleHitTarget()
    {
        hitTarget = true;
        damageZone.GetComponent<Renderer>().material.color = new Color(1f, 0, 0);
        explosionSound.Play();
        explosionParticles.Play();
        StartCoroutine(DestroyObjects());
    }


    /**
     * Handles attack based on collider at secondsInAir seconds
     * after Missile was spawned. Can be used to damage more than just the player.
     * 
     * collider     colliders inside damageZone at time of explosion. This value
     *              is retreived using onTriggerEnter in MissileAttackScript.cs
     */
    public void handleAttack(Collider collider)
    {
        if (collider.tag == "Player")
        {
            Tank player = collider.gameObject.GetComponent<Tank>();
            player.takeDamage(damage);
            player.incapacitate(1);

            damageZone.GetComponent<MeshRenderer>().enabled = false;
            damageZone.GetComponent<SphereCollider>().enabled = false;
        } else if (collider.tag == "Enemy") {
            Enemy e = collider.gameObject.GetComponent<Enemy>();
            e.takeDamage(damage);

            damageZone.GetComponent<MeshRenderer>().enabled = false;
            damageZone.GetComponent<SphereCollider>().enabled = false;
        }
    }

    IEnumerator DestroyObjects()
    {
        Destroy(missileBody);
        Destroy(targetZone);
        yield return new WaitForSeconds(explosionTime);
        Destroy(gameObject);
    }

}
