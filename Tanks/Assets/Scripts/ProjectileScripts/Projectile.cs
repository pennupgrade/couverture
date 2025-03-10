using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected int damage;
    [SerializeField] protected float lifetime;
    [SerializeField] protected GameObject explosionPrefab;
    public float bulletSpeed;
    protected bool destroyed;
    public GameObject parent;

    public float dontDamageOnSpawnDelay=0.1f;

    protected float startLifetime;

    protected virtual void Awake()
    {
        startLifetime = lifetime;
        //StartCoroutine(WaitForParentAndCheckWall());
        
    }

    private IEnumerator WaitForParentAndCheckWall()
    {
        // Wait until the parent is assigned
        while (parent == null)
        {
            Debug.LogWarning("Projectile waiting for parent to be assigned...");
            yield return null; // Wait for the next frame
        }

        // Perform the check after the parent is assigned
        Debug.Log("Parent assigned, checking collision");
        CheckWallCollision();
    }

    protected virtual void CheckWallCollision() {

        // Get the player's position
        Vector3 playerPos = parent.transform.position;
        playerPos.y += 0.1666667f;
        Vector3 bulletPos = transform.position;

        // Define the direction from the player to the bullet's spawn point
        Vector3 direction = (bulletPos - playerPos).normalized;

        transform.position += 12f * direction; // Moves the bullet forward slightly
        //playerPos -= direction;

        // Raycast to check if a wall is in between
        RaycastHit hit;
        int layerMask = ~((1 << 2) | (1 << 7) | (1 << 8) | (1 << 9) | (1 << 11)); // Ignore layers 7, 9 and 11

        Debug.DrawRay(playerPos, 3f * direction * Vector3.Distance(playerPos, bulletPos), Color.red, 2.0f);
        Debug.DrawRay(playerPos, new Vector3(0, 1, 0), Color.red, 1.0f);
        Debug.DrawRay(bulletPos, new Vector3(0, 1, 0), Color.blue, 1.0f);

        if (Physics.Raycast(playerPos, direction, out hit, Vector3.Distance(playerPos, bulletPos), layerMask))
        {
            Debug.Log(hit.collider.gameObject.name);
            // Special check for OneWayWall
            if (hit.collider.CompareTag("OneWay"))
            {
                Vector3 wallNormal = hit.normal; // Get wall's normal
                float dotProduct = Vector3.Dot(direction, wallNormal);
                Debug.Log(dotProduct);

                if (dotProduct < 0) 
                {
                    Debug.Log("Bullet blocked by a One-Way Wall.");
                    Destroy(gameObject);
                } else 
                {
                    Debug.Log("One-Way Wall allows bullet to go through.");
                    transform.position -= 12f * direction;
                }
                // Transform wallTransform = hit.collider.transform;
                // float rotationY = wallTransform.rotation.eulerAngles.y;
                // bool shouldPassThrough = false;

                // // Check if the wall is aligned along the Z-axis
                // if (Mathf.Approximately(rotationY, 0f) || Mathf.Approximately(rotationY, 180f))
                // {
                //     // One-way walls allow shooting in the negative X direction
                //     if (direction.x < 0) 
                //         shouldPassThrough = true;
                // }
                // // Check if the wall is aligned along the X-axis
                // else if (Mathf.Approximately(rotationY, 90f) || Mathf.Approximately(rotationY, 270f))
                // {
                //     // One-way walls allow shooting in the negative Z direction
                //     if (direction.z < 0)
                //         shouldPassThrough = true;
                // } else if (false) {
                //     Debug.Log("rotationY - 180/Mathf.PI * Mathf.Atan(direction.x / direction.z)");
                //     Debug.Log(rotationY - 180/Mathf.PI * Mathf.Atan(direction.x / direction.z));
                //     shouldPassThrough = true;
                // }

                // if (!shouldPassThrough)
                // {
                //     Debug.Log("Bullet blocked by a One-Way Wall.");
                //     Destroy(gameObject);
                // } else {
                //     Debug.Log("One-Way Wall allows bullet to go through.");
                // }
            } else {
                Debug.Log("Bullet spawned on the wrong side of a wall. Destroying...");
                Destroy(gameObject);
                return;
            }
        } else {
            Debug.Log("no wall glitch detected.");
            transform.position -= 12f * direction;
        }
    }

    protected virtual void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime < 0) Destroy(gameObject);
    }

    public virtual void destruction() {
        if (destroyed) return;
        destroyed = true;
        if (explosionPrefab != null) {
            GameObject expl = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(expl, 2);
        }
        //first child should be a trail
        if (transform.childCount != 0) {
            if (transform.GetChild(0).gameObject.TryGetComponent<MeshTrail>(out MeshTrail mt)) {
                mt.kill();
            } else if (transform.GetChild(0).gameObject.TryGetComponent<ParticleSystem>(out ParticleSystem ps)){
                ps.Stop();
                //for rocket
                transform.GetChild(0).localScale = 1 / transform.localScale.x * transform.GetChild(0).localScale;
                Destroy(transform.GetChild(0).gameObject, 2.5f);
            }
            transform.GetChild(0).parent = null;
        }

        removeObjectFromGame();
    }

    protected virtual void removeObjectFromGame()
    {
        Destroy(gameObject);
    }

    protected bool defaultCollisionChecks(Collision collision) {
        if ((startLifetime-lifetime)<dontDamageOnSpawnDelay) {
            Debug.Log("(startLifetime-lifetime)<dontDamageOnSpawnDelay");
            return true;
        }
        if (collision.gameObject.TryGetComponent<IDestroyable>(out IDestroyable d) && (startLifetime-lifetime)>dontDamageOnSpawnDelay) // hit a player
        {
            d.takeDamage(damage);
            destruction();
            return true;
        }

        if (collision.gameObject.tag == "Projectile" || collision.gameObject.tag == "NoBounce") // Parry other projectile or no bounce
        {
            destruction();
            return true;
        }
        return false;
    }

}
