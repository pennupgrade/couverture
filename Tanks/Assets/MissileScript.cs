using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileScript : MonoBehaviour
{
    private float g = -10f;

    public int damage;
    public Vector3 startLocation;
    public Vector3 endLocation;
    public float secondsInAir; //must be positive

    public GameObject missileBody;
    public GameObject targetZone;
    public GameObject damageZone;

    private AudioSource explosionSound;
    private ParticleSystem explosionParticles;

    private bool hitTarget;
    private Vector3 acceleration;
    private Vector3 velocity;

    private Renderer targetZoneRenderer;
    private Color initialTargetZoneColor = new Color(1f, 0, 0, 0);
    private Color finalTargetZoneColor = new Color(1f, 0, 0, 1f);
    private Color flashTargetZoneColor = new Color(1f, 1f, 0, 1f);
    private float colorLerpTime;
    private float flashingTimer;



    // Start is called before the first frame update
    void Start()
    {
        startLocation = gameObject.transform.position;
        endLocation = targetZone.transform.position;
        secondsInAir = Mathf.Abs(secondsInAir); //avoid negative time

        hitTarget = false;
        acceleration = new Vector3(0, g, 0);
        explosionSound = gameObject.GetComponent<AudioSource>();
        explosionParticles = damageZone.GetComponent<ParticleSystem>();

        targetZoneRenderer = targetZone.GetComponent<Renderer>();
        targetZoneRenderer.material.color = initialTargetZoneColor;

        colorLerpTime = 0;
        flashingTimer = 0;
         
        //calculate initial velocity

        float initialVelocityX = (endLocation.x - startLocation.x) / secondsInAir;
        float initialVelocityZ = (endLocation.z - startLocation.z) / secondsInAir;

        float displacementY = endLocation.y - startLocation.y;
        float initialVelocityY = (float) ((displacementY - 0.5 * g * Mathf.Pow(secondsInAir,2)) / secondsInAir);
        velocity = new Vector3(initialVelocityX, initialVelocityY, initialVelocityZ);

        missileBody.transform.rotation = Quaternion.LookRotation(new Vector3(0, 1, 0)); //initially facing upward
    }

    // Update is called once per frame
    void Update()
    {
        if (!hitTarget)
        {
            missileBody.transform.position += velocity * Time.deltaTime;
            velocity += acceleration * Time.deltaTime;
            missileBody.transform.rotation = Quaternion.LookRotation(velocity);
        } else
        {
            velocity = new Vector3(0, 0, 0);
            damageZone.transform.position = endLocation;
        }


        //targetZone reaches full opacity after 0.8 * secondsInAir
        if (colorLerpTime < 1)
        {
            colorLerpTime += Time.deltaTime / (0.8f * secondsInAir);
            targetZoneRenderer.material.color =
                Color.Lerp(initialTargetZoneColor, finalTargetZoneColor, colorLerpTime);
        } else if (flashingTimer < 1)
        {
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

    public void setHitTarget()
    {
        hitTarget = true;
        damageZone.GetComponent<Renderer>().material.color = new Color(1f, 0, 0);
        damageZone.GetComponent<MeshRenderer>().enabled = true;
        explosionSound.Play();
        explosionParticles.Play();
    }

}
